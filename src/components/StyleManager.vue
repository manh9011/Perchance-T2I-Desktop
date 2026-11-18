<script setup lang="ts">
import { ref, computed } from 'vue';
import { useStylesStore, type CustomStyle } from '../stores/stylesStore';
import { artStyles } from '../constants/artStyles';
import { getBuiltInStyleThumbnail } from '../utils/styleThumbnails';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import Textarea from 'primevue/textarea';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import TabView from 'primevue/tabview';
import TabPanel from 'primevue/tabpanel';
import { useConfirm } from 'primevue/useconfirm';
import { useToast } from 'primevue/usetoast';

const props = defineProps<{
    visible: boolean;
}>();

const emit = defineEmits<{
    (e: 'update:visible', value: boolean): void;
}>();

const stylesStore = useStylesStore();
const confirm = useConfirm();
const toast = useToast();

// Edit dialog state
const editDialogVisible = ref(false);
const editingStyle = ref<Partial<CustomStyle> | null>(null);
const isNewStyle = ref(false);

// Built-in styles list with bundled static thumbnails
const builtInStyles = computed(() =>
    Object.entries(artStyles).map(([name]) => ({
        name,
        isBuiltIn: true,
        thumbnail: name === 'No Style' ? undefined : getBuiltInStyleThumbnail(name),
    }))
);

// Custom styles from store
const customStyles = computed(() => stylesStore.customStyles);

const openAddDialog = () => {
    editingStyle.value = {
        name: '',
        promptTemplate: '{prompt}',
        negativeTemplate: '{negative}'
    };
    isNewStyle.value = true;
    editDialogVisible.value = true;
};

const openEditDialog = (style: CustomStyle) => {
    editingStyle.value = { ...style };
    isNewStyle.value = false;
    editDialogVisible.value = true;
};

const duplicateBuiltIn = (name: string) => {
    const style = artStyles[name];
    if (style) {
        // Convert function to template (showing the pattern)
        const promptTemplate = style.prompt({ prompt: '{prompt}', negative: '{negative}' } as any);
        const negativeTemplate = style.negative({ prompt: '{prompt}', negative: '{negative}' } as any);

        editingStyle.value = {
            name: `${name} (Custom)`,
            promptTemplate,
            negativeTemplate
        };
        isNewStyle.value = true;
        editDialogVisible.value = true;
    }
};

const saveStyle = () => {
    if (!editingStyle.value?.name?.trim()) {
        toast.add({ severity: 'error', summary: 'Error', detail: 'Style name is required', life: 2000 });
        return;
    }

    if (isNewStyle.value) {
        stylesStore.addCustomStyle({
            name: editingStyle.value.name,
            promptTemplate: editingStyle.value.promptTemplate || '{prompt}',
            negativeTemplate: editingStyle.value.negativeTemplate || '{negative}'
        });
        toast.add({ severity: 'success', summary: 'Created', detail: 'Style created successfully', life: 2000 });
    } else if (editingStyle.value.id) {
        stylesStore.updateCustomStyle(editingStyle.value.id, {
            name: editingStyle.value.name,
            promptTemplate: editingStyle.value.promptTemplate,
            negativeTemplate: editingStyle.value.negativeTemplate
        });
        toast.add({ severity: 'success', summary: 'Updated', detail: 'Style updated successfully', life: 2000 });
    }

    editDialogVisible.value = false;
    editingStyle.value = null;
};

const deleteStyle = (style: CustomStyle) => {
    confirm.require({
        message: `Delete style "${style.name}"?`,
        header: 'Confirm Delete',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: () => {
            stylesStore.deleteCustomStyle(style.id);
            toast.add({ severity: 'success', summary: 'Deleted', detail: 'Style deleted', life: 2000 });
        }
    });
};
</script>

<template>
    <Dialog :visible="props.visible" @update:visible="emit('update:visible', $event)" header="Manage Art Styles" modal
        maximizable :style="{ width: '800px', maxWidth: '100vw' }">
        <TabView>
            <TabPanel value="custom" header="Custom Styles">
                <div class="mb-3">
                    <Button label="New Style" icon="pi pi-plus" size="small" @click="openAddDialog" />
                </div>

                <DataTable :value="customStyles" size="small" v-if="customStyles.length > 0">
                    <Column field="name" header="Name" />
                    <Column header="Actions" style="width: 8rem;">
                        <template #body="{ data }">
                            <div class="action-buttons">
                                <Button icon="pi pi-pencil" size="small" severity="secondary" text
                                    @click="openEditDialog(data)" v-tooltip="'Edit'" />
                                <Button icon="pi pi-trash" size="small" severity="danger" text
                                    @click="deleteStyle(data)" v-tooltip="'Delete'" />
                            </div>
                        </template>
                    </Column>
                </DataTable>

                <div v-else class="empty-state">
                    <i class="pi pi-palette"></i>
                    <p>No custom styles yet. Create one or duplicate a built-in style.</p>
                </div>
            </TabPanel>

            <TabPanel value="builtin" header="Built-in Styles">
                <div class="style-grid">
                    <div v-for="style in builtInStyles" :key="style.name" class="style-card">
                        <div class="style-image-wrapper">
                            <div v-if="style.name === 'No Style'" class="image-placeholder no-style">
                                <i class="pi pi-ban"></i>
                            </div>
                            <img v-else-if="style.thumbnail" :src="style.thumbnail" :alt="style.name" loading="lazy" />
                            <div v-else class="image-placeholder no-style">
                                <i class="pi pi-image"></i>
                            </div>
                            <div class="style-overlay" v-if="style.name !== 'No Style'">
                                <Button icon="pi pi-copy" rounded severity="contrast"
                                    @click="duplicateBuiltIn(style.name)" v-tooltip.top="'Duplicate'"
                                    class="overlay-btn" />
                            </div>
                        </div>
                        <div class="style-name">{{ style.name }}</div>
                    </div>
                </div>
            </TabPanel>
        </TabView>
    </Dialog>

    <!-- Edit/Add Style Dialog -->
    <Dialog v-model:visible="editDialogVisible" :header="isNewStyle ? 'New Style' : 'Edit Style'" modal maximizable
        class="style-edit-dialog">
        <div class="edit-form" v-if="editingStyle">
            <div class="form-field">
                <label>Name</label>
                <InputText v-model="editingStyle.name" fluid placeholder="Style name" />
            </div>
            <div class="form-field">
                <label>Prompt Template</label>
                <Textarea v-model="editingStyle.promptTemplate" rows="4" fluid
                    placeholder="Use {prompt} and {negative} as placeholders" />
                <small class="hint">Use {prompt} where user's prompt should appear</small>
            </div>
            <div class="form-field">
                <label>Negative Template</label>
                <Textarea v-model="editingStyle.negativeTemplate" rows="3" fluid
                    placeholder="Use {prompt} and {negative} as placeholders" />
                <small class="hint">Use {negative} where user's negative prompt should appear</small>
            </div>
        </div>

        <template #footer>
            <Button label="Cancel" severity="secondary" @click="editDialogVisible = false" />
            <Button label="Save" icon="pi pi-check" @click="saveStyle" />
        </template>
    </Dialog>
</template>

<style scoped>
.style-edit-dialog {
    width: 40rem;
    max-width: 90vw;
}

.mb-3 {
    margin-bottom: 1rem;
}

.action-buttons {
    display: flex;
    gap: 0.25rem;
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
    gap: 1rem;
}

.form-field {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.form-field label {
    font-weight: 500;
    font-size: 0.875rem;
}

.hint {
    font-size: 0.75rem;
    color: var(--p-surface-400);
}

/* Grid Layout */
.style-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(110px, 128px));
    gap: 0.75rem;
    padding: 0.5rem;
}

.style-card {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    align-items: center;
}

.style-image-wrapper {
    width: 100%;
    aspect-ratio: 1;
    border-radius: 8px;
    overflow: hidden;
    position: relative;
    background: var(--p-surface-100);
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.style-image-wrapper img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    transition: transform 0.2s;
}

.image-placeholder {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: var(--p-surface-100);
}

.style-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.3);
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    opacity: 0;
    transition: opacity 0.2s;
}

.style-image-wrapper:hover .style-overlay {
    opacity: 1;
}

.style-image-wrapper:hover img {
    transform: scale(1.05);
}

.overlay-btn {
    width: 2.5rem !important;
    height: 2.5rem !important;
    backdrop-filter: blur(4px);
}

.style-name {
    font-size: 0.875rem;
    text-align: center;
    font-weight: 500;
    /* Truncate nicely */
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    line-clamp: 2;
    -webkit-box-orient: vertical;
}

.no-style {
    background-color: var(--p-surface-200);
}

.no-style i {
    font-size: 3rem;
    color: var(--p-surface-400);
}
</style>
