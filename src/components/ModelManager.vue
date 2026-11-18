<script setup lang="ts">
import { ref, computed } from 'vue';
import {
    useModelsStore,
    MODEL_PRESETS,
    type CustomModel,
    type ModelPreset,
    type ResponseOutputType,
} from '../stores/modelsStore';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Textarea from 'primevue/textarea';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import TabView from 'primevue/tabview';
import TabPanel from 'primevue/tabpanel';
import Select from 'primevue/select';
import SelectButton from 'primevue/selectbutton';
import ToggleSwitch from 'primevue/toggleswitch';
import Tag from 'primevue/tag';
import { useConfirm } from 'primevue/useconfirm';
import { useToast } from 'primevue/usetoast';

const props = defineProps<{
    visible: boolean;
}>();

const emit = defineEmits<{
    (e: 'update:visible', value: boolean): void;
}>();

const modelsStore = useModelsStore();
const confirm = useConfirm();
const toast = useToast();

const createEmptyEditingModel = (): Partial<CustomModel> => ({
    name: '',
    url: '',
    method: 'POST',
    headers: '',
    payload: '',
    responseType: 'json',
    imageField: '',
    errorField: '',
    cfgMultiplier: 1,
    enabled: true,
});

const editDialogVisible = ref(false);
const editingModel = ref<Partial<CustomModel>>(createEmptyEditingModel());
const isNewModel = ref(false);
const payloadTextareaRef = ref<any>(null);

const customModels = computed(() => modelsStore.customModels);
const builtInModels = computed(() => modelsStore.builtInModels);

const methodOptions: ('POST' | 'GET')[] = ['POST', 'GET'];
const responseTypeOptions: { label: string; value: ResponseOutputType }[] = [
    { label: 'JSON', value: 'json' },
    { label: 'Direct Image', value: 'image' },
];

const placeholderTokens = [
    '{prompt}',
    '{negative}',
    '{cfg}',
    '{seed}',
    '{ratio}',
    '{width}',
    '{height}',
    '{size}',
];

const openAddDialog = () => {
    const defaultPreset = MODEL_PRESETS[3].model; // OpenAI Image Gen preset as clean starter
    editingModel.value = {
        ...defaultPreset,
        id: undefined,
        name: '',
        cfgMultiplier: 1,
    };
    isNewModel.value = true;
    editDialogVisible.value = true;
};

const openEditDialog = (model: CustomModel) => {
    editingModel.value = {
        ...model,
        cfgMultiplier: model.cfgMultiplier ?? 1,
    };
    isNewModel.value = false;
    editDialogVisible.value = true;
};

const applyPreset = (preset: ModelPreset) => {
    if (!editingModel.value) return;
    const preservedId = isNewModel.value ? undefined : editingModel.value.id;
    editingModel.value = {
        ...preset.model,
        id: preservedId,
    };
};

const insertPlaceholder = (token: string) => {
    if (!editingModel.value) return;
    const el: HTMLTextAreaElement | undefined = payloadTextareaRef.value?.$el;
    const current = editingModel.value.payload || '';
    if (el && typeof el.selectionStart === 'number' && typeof el.selectionEnd === 'number') {
        const start = el.selectionStart;
        const end = el.selectionEnd;
        editingModel.value.payload = current.slice(0, start) + token + current.slice(end);
        requestAnimationFrame(() => {
            el.focus();
            el.setSelectionRange(start + token.length, start + token.length);
        });
    } else {
        editingModel.value.payload = current + token;
    }
};

const validatePayloadTemplate = (payload: string): string | null => {
    const trimmed = payload.trim();
    if (!trimmed) return null;
    if (!trimmed.startsWith('{') && !trimmed.startsWith('[')) return null;

    const sampleReplaced = trimmed
        .replace(
            /\{(cfg|guidance_scale)\s*([+\-*/]\s*\d+(?:\.\d+)?(?:\s*[+\-*/]\s*\d+(?:\.\d+)?)*)\}/gi,
            '7'
        )
        .replace(
            /\{(prompt|positive_prompt|negative|negative_prompt|cfg|guidance_scale|seed|ratio|aspect_ratio|width|height|size)\}/gi,
            (_, key: string) => {
                const k = key.toLowerCase();
                if (k === 'cfg' || k === 'guidance_scale') return '7';
                if (k === 'seed') return '42';
                if (k === 'width' || k === 'height') return '1024';
                if (k === 'ratio' || k === 'aspect_ratio') return '1:1';
                if (k === 'size') return '1024x1024';
                return 'sample';
            }
        );

    try {
        JSON.parse(sampleReplaced);
        return null;
    } catch (e: any) {
        return e?.message || 'Invalid JSON syntax';
    }
};

const validateHeadersTemplate = (headers: string): string | null => {
    const trimmed = headers.trim();
    if (!trimmed || !trimmed.startsWith('{')) return null;

    const sampleReplaced = trimmed.replace(
        /\{(prompt|positive_prompt|negative|negative_prompt|cfg|guidance_scale|seed|ratio|aspect_ratio|width|height|size)\}/gi,
        'sample'
    );
    try {
        const parsed = JSON.parse(sampleReplaced);
        if (!parsed || typeof parsed !== 'object' || Array.isArray(parsed)) {
            return 'Headers JSON must be a key-value object';
        }
        return null;
    } catch (e: any) {
        return e?.message || 'Invalid Headers JSON syntax';
    }
};

const saveModel = () => {
    const m = editingModel.value;
    if (!m?.name?.trim()) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Model name is required', life: 2500 });
        return;
    }
    if (!m?.url?.trim()) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Endpoint URL is required', life: 2500 });
        return;
    }

    const headerErr = validateHeadersTemplate(m.headers || '');
    if (headerErr) {
        toast.add({ severity: 'error', summary: 'Invalid Headers', detail: headerErr, life: 3000 });
        return;
    }

    if ((m.method || 'POST') !== 'GET') {
        const payloadErr = validatePayloadTemplate(m.payload || '');
        if (payloadErr) {
            toast.add({ severity: 'error', summary: 'Invalid Payload JSON', detail: payloadErr, life: 3500 });
            return;
        }
    }

    if (isNewModel.value) {
        modelsStore.addCustomModel({
            name: m.name.trim(),
            url: m.url.trim(),
            method: m.method || 'POST',
            headers: m.headers || '',
            payload: m.payload || '',
            responseType: m.responseType || 'json',
            imageField: (m.imageField || '').trim(),
            errorField: (m.errorField || '').trim(),
            cfgMultiplier: typeof m.cfgMultiplier === 'number' && m.cfgMultiplier > 0 ? m.cfgMultiplier : 1,
            enabled: m.enabled ?? true,
        });
        toast.add({ severity: 'success', summary: 'Created', detail: 'Custom model added', life: 2000 });
    } else if (m.id) {
        modelsStore.updateCustomModel(m.id, {
            name: m.name.trim(),
            url: m.url.trim(),
            method: m.method || 'POST',
            headers: m.headers || '',
            payload: m.payload || '',
            responseType: m.responseType || 'json',
            imageField: (m.imageField || '').trim(),
            errorField: (m.errorField || '').trim(),
            cfgMultiplier: typeof m.cfgMultiplier === 'number' && m.cfgMultiplier > 0 ? m.cfgMultiplier : 1,
            enabled: m.enabled ?? true,
        });
        toast.add({ severity: 'success', summary: 'Updated', detail: 'Custom model updated', life: 2000 });
    }

    editDialogVisible.value = false;
    editingModel.value = createEmptyEditingModel();
};

const duplicateModel = (model: CustomModel) => {
    modelsStore.duplicateCustomModel(model);
    toast.add({ severity: 'success', summary: 'Duplicated', detail: `Copied "${model.name}"`, life: 2000 });
};

const deleteModel = (model: CustomModel) => {
    confirm.require({
        message: `Delete model "${model.name}"?`,
        header: 'Confirm Delete',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: () => {
            modelsStore.deleteCustomModel(model.id);
            toast.add({ severity: 'success', summary: 'Deleted', detail: 'Model deleted', life: 2000 });
        },
    });
};

const toggleCustomModel = (model: CustomModel, enabled: boolean) => {
    modelsStore.updateCustomModel(model.id, { enabled });
};

const toggleBuiltInModel = (id: string, enabled: boolean) => {
    modelsStore.setBuiltInEnabled(id, enabled);
};

const restoreColabModels = () => {
    const added = modelsStore.restoreColabPresets();
    if (added > 0) {
        toast.add({
            severity: 'success',
            summary: 'Restored',
            detail: `Added ${added} ColabImageAPI model(s)`,
            life: 2500,
        });
    } else {
        toast.add({
            severity: 'info',
            summary: 'Already Present',
            detail: 'All 3 ColabImageAPI models are already configured',
            life: 2500,
        });
    }
};
</script>

<template>
    <Dialog :visible="props.visible" @update:visible="emit('update:visible', $event)" header="Manage Models" modal
        maximizable :style="{ width: '880px', maxWidth: '100vw' }">
        <TabView>
            <!-- Tab 1: Custom Models -->
            <TabPanel value="custom" header="Custom Models">
                <div class="toolbar mb-3">
                    <Button label="New Model" icon="pi pi-plus" size="small" @click="openAddDialog" />
                    <Button label="Restore ColabImageAPI Models" icon="pi pi-refresh" size="small" severity="secondary"
                        outlined @click="restoreColabModels" />
                </div>

                <DataTable :value="customModels" size="small" v-if="customModels.length > 0" scrollable>
                    <Column header="Active" style="width: 4.5rem;">
                        <template #body="{ data }">
                            <ToggleSwitch :modelValue="data.enabled"
                                @update:modelValue="(val: boolean) => toggleCustomModel(data, val)" />
                        </template>
                    </Column>
                    <Column field="name" header="Name" style="min-width: 11rem;">
                        <template #body="{ data }">
                            <span class="model-name-cell">{{ data.name }}</span>
                        </template>
                    </Column>
                    <Column header="Endpoint" style="min-width: 15rem;">
                        <template #body="{ data }">
                            <div class="endpoint-cell">
                                <Tag :value="data.method" severity="info" class="mini-tag" />
                                <span class="url-text" :title="data.url">{{ data.url }}</span>
                            </div>
                        </template>
                    </Column>
                    <Column header="Output" style="min-width: 11rem;">
                        <template #body="{ data }">
                            <div class="output-cell">
                                <Tag :value="data.responseType === 'image' ? 'Direct Image' : 'JSON'"
                                    :severity="data.responseType === 'image' ? 'warn' : 'success'" class="mini-tag" />
                                <span class="field-hint" v-if="data.responseType === 'json'">
                                    img: <code>{{ data.imageField || 'auto' }}</code>
                                </span>
                                <span class="field-hint" v-if="data.errorField">
                                    err: <code>{{ data.errorField }}</code>
                                </span>
                            </div>
                        </template>
                    </Column>
                    <Column header="Actions" style="width: 8.5rem;">
                        <template #body="{ data }">
                            <div class="action-buttons">
                                <Button icon="pi pi-copy" size="small" severity="secondary" text
                                    @click="duplicateModel(data)" v-tooltip.top="'Duplicate'" />
                                <Button icon="pi pi-pencil" size="small" severity="secondary" text
                                    @click="openEditDialog(data)" v-tooltip.top="'Edit'" />
                                <Button icon="pi pi-trash" size="small" severity="danger" text
                                    @click="deleteModel(data)" v-tooltip.top="'Delete'" />
                            </div>
                        </template>
                    </Column>
                </DataTable>

                <div v-else class="empty-state">
                    <i class="pi pi-server"></i>
                    <p>No custom models yet. Create one or restore the ColabImageAPI presets.</p>
                </div>
            </TabPanel>

            <!-- Tab 2: Built-in Models -->
            <TabPanel value="builtin" header="Built-in Models">
                <p class="builtin-intro">
                    Enable or disable the 3 built-in models displayed in the model selector.
                </p>

                <DataTable :value="builtInModels" size="small">
                    <Column header="Active" style="width: 5rem;">
                        <template #body="{ data }">
                            <ToggleSwitch :modelValue="data.enabled"
                                @update:modelValue="(val: boolean) => toggleBuiltInModel(data.id, val)" />
                        </template>
                    </Column>
                    <Column field="name" header="Model Name" />
                    <Column header="ID">
                        <template #body="{ data }">
                            <code>{{ data.id }}</code>
                        </template>
                    </Column>
                    <Column header="Status" style="width: 8rem;">
                        <template #body="{ data }">
                            <Tag :value="data.enabled ? 'Enabled' : 'Disabled'"
                                :severity="data.enabled ? 'success' : 'secondary'" />
                        </template>
                    </Column>
                </DataTable>
            </TabPanel>
        </TabView>
    </Dialog>

    <!-- Edit/Add Custom Model Dialog -->
    <Dialog v-model:visible="editDialogVisible" :header="isNewModel ? 'New Custom Model' : 'Edit Custom Model'" modal
        maximizable class="model-edit-dialog">
        <div class="edit-form">
            <div class="form-field">
                <label>Quick Presets (ColabImageAPI &amp; OpenAI Compatible)</label>
                <div class="preset-chips">
                    <Button v-for="preset in MODEL_PRESETS" :key="preset.id" :label="preset.label" size="small"
                        severity="secondary" outlined class="preset-btn" @click="applyPreset(preset)" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field flex-1">
                    <label>Model Name</label>
                    <InputText v-model="editingModel.name" fluid placeholder="e.g. Colab Z-Image-Turbo" />
                </div>
                <div class="form-field cfg-mult-field">
                    <label>CFG Multiplier</label>
                    <InputNumber v-model="editingModel.cfgMultiplier" :min="0.001" :max="100" :step="0.1"
                        :minFractionDigits="0" :maxFractionDigits="4" fluid />
                    <small class="hint">
                        UI 1–20 → {{ Math.round(1 * (editingModel.cfgMultiplier || 1) * 100) / 100 }}–{{
                            Math.round(20 * (editingModel.cfgMultiplier || 1) * 100) / 100 }}
                    </small>
                </div>
                <div class="form-field enabled-field">
                    <label>Enabled</label>
                    <ToggleSwitch v-model="editingModel.enabled" />
                </div>
            </div>

            <div class="form-row">
                <div class="form-field method-field">
                    <label>Method</label>
                    <Select v-model="editingModel.method" :options="methodOptions" fluid />
                </div>
                <div class="form-field flex-1">
                    <label>Endpoint URL</label>
                    <InputText v-model="editingModel.url" fluid
                        placeholder="http://127.0.0.1:2504/api/generate or /v1/images/generations" />
                </div>
            </div>

            <div class="form-field">
                <label>HTTP Headers (JSON object or Key: Value lines)</label>
                <Textarea v-model="editingModel.headers" rows="3" fluid class="mono-input"
                    placeholder='{"Content-Type": "application/json", "Authorization": "Bearer ..."}' />
            </div>

            <div class="form-field">
                <div class="label-with-tools">
                    <label>Request Payload Template (JSON)</label>
                    <div class="token-list">
                        <span v-for="token in placeholderTokens" :key="token" class="token-chip"
                            @click="insertPlaceholder(token)" title="Click to insert into payload">
                            {{ token }}
                        </span>
                    </div>
                </div>
                <Textarea ref="payloadTextareaRef" v-model="editingModel.payload" rows="7" fluid class="mono-input"
                    :disabled="editingModel.method === 'GET'"
                    placeholder='{"prompt": "{prompt}", "negative_prompt": "{negative}", "cfg": {cfg}, "seed": {seed}, "aspect_ratio": "{ratio}"}' />
                <small class="hint">
                    Supported variables: {prompt}, {negative}, {cfg}, {seed}, {ratio} (1:1, 16:9, 9:16), {width},
                    {height}, {size}
                </small>
            </div>

            <div class="form-row">
                <div class="form-field">
                    <label>Output Format</label>
                    <SelectButton v-model="editingModel.responseType" :options="responseTypeOptions"
                        optionLabel="label" optionValue="value" :allowEmpty="false" size="small" />
                </div>
                <div class="form-field flex-1">
                    <label>Image Field (JSON Path)</label>
                    <InputText v-model="editingModel.imageField" fluid
                        :disabled="editingModel.responseType === 'image'"
                        placeholder="b64_json, data[0].b64_json, data[0].url, local_path" />
                    <small class="hint">Supports URL (auto-download), Base64, Data URL, or local file path</small>
                </div>
                <div class="form-field flex-1">
                    <label>Error Message Field (JSON Path)</label>
                    <InputText v-model="editingModel.errorField" fluid placeholder="detail, error.message, message" />
                    <small class="hint">Extracted into an error SVG card if generation fails</small>
                </div>
            </div>
        </div>

        <template #footer>
            <Button label="Cancel" severity="secondary" @click="editDialogVisible = false" />
            <Button label="Save" icon="pi pi-check" @click="saveModel" />
        </template>
    </Dialog>
</template>

<style scoped>
.model-edit-dialog {
    width: 48rem;
    max-width: 95vw;
}

.toolbar {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
}

.mb-3 {
    margin-bottom: 1rem;
}

.builtin-intro {
    font-size: 0.875rem;
    color: var(--p-surface-400);
    margin: 0 0 0.75rem 0;
}

.model-name-cell {
    font-weight: 600;
}

.endpoint-cell {
    display: flex;
    align-items: center;
    gap: 0.4rem;
    max-width: 17rem;
}

.url-text {
    font-family: monospace;
    font-size: 0.75rem;
    color: var(--p-surface-300);
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.output-cell {
    display: flex;
    flex-direction: column;
    gap: 0.2rem;
    align-items: flex-start;
}

.field-hint {
    font-size: 0.7rem;
    color: var(--p-surface-400);
}

.mini-tag {
    font-size: 0.65rem;
    padding: 0.1rem 0.4rem;
}

.action-buttons {
    display: flex;
    gap: 0.2rem;
}

.empty-state {
    text-align: center;
    padding: 2rem;
    color: var(--p-surface-400);
}

.empty-state i {
    font-size: 2rem;
    margin-bottom: 0.5rem;
}

.edit-form {
    display: flex;
    flex-direction: column;
    gap: 0.9rem;
}

.form-row {
    display: flex;
    gap: 0.75rem;
    align-items: flex-start;
    flex-wrap: wrap;
}

.flex-1 {
    flex: 1;
    min-width: 12rem;
}

.method-field {
    width: 7rem;
}

.enabled-field {
    align-items: center;
    min-width: 4.5rem;
}

.cfg-mult-field {
    width: 9.5rem;
}

.form-field {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.form-field label {
    font-weight: 500;
    font-size: 0.85rem;
}

.mono-input {
    font-family: monospace;
    font-size: 0.82rem;
}

.hint {
    font-size: 0.73rem;
    color: var(--p-surface-400);
}

.preset-chips {
    display: flex;
    flex-wrap: wrap;
    gap: 0.4rem;
}

.preset-btn {
    font-size: 0.75rem !important;
    padding: 0.25rem 0.55rem !important;
}

.label-with-tools {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 0.5rem;
    flex-wrap: wrap;
}

.token-list {
    display: flex;
    flex-wrap: wrap;
    gap: 0.25rem;
}

.token-chip {
    font-family: monospace;
    font-size: 0.72rem;
    padding: 0.1rem 0.4rem;
    border-radius: 4px;
    background: var(--p-surface-700);
    color: var(--p-primary-300);
    cursor: pointer;
    user-select: none;
    transition: background-color 0.15s;
}

.token-chip:hover {
    background: var(--p-surface-600);
}
</style>
