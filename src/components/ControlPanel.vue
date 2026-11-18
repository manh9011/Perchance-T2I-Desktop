<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useGeneratorStore } from '../stores/generatorStore';
import { useModelsStore } from '../stores/modelsStore';
import { artStyles } from '../constants/artStyles';
import * as components from '../constants/promptComponents';
import Textarea from 'primevue/textarea';
import Select from 'primevue/select';
import Slider from 'primevue/slider';
import InputNumber from 'primevue/inputnumber';
import Button from 'primevue/button';
import Tag from 'primevue/tag';
import Checkbox from 'primevue/checkbox';
import Dialog from 'primevue/dialog';
import StyleManager from './StyleManager.vue';
import ModelManager from './ModelManager.vue';
import StylePicker from './StylePicker.vue';
import Popover from 'primevue/popover';

const stylePickerPopover = ref();
const dialogStylePickerPopover = ref();
const languagePopover = ref();

const toggleLanguagePopover = (event: Event) => {
    languagePopover.value.toggle(event);
};

const selectLanguage = (lang: string) => {
    store.config.language = lang;
    languagePopover.value.hide();
};

const toggleStylePicker = (event: Event) => {
    stylePickerPopover.value.toggle(event);
};

const toggleDialogStylePicker = (event: Event) => {
    dialogStylePickerPopover.value.toggle(event);
};

const onStyleSelect = () => {
    stylePickerPopover.value.hide();
};

const onDialogStyleSelect = () => {
    dialogStylePickerPopover.value.hide();
};

const store = useGeneratorStore();
const modelsStore = useModelsStore();
const styleManagerVisible = ref(false);
const modelManagerVisible = ref(false);
const promptDialogVisible = ref(false);
const isRandomizing = ref(false);

const modelOptions = computed(() => modelsStore.availableModels.map(m => ({
    label: m.name,
    value: m.id,
    isNew: m.isNew,
    isCustom: !m.isBuiltIn
})));

watch(
    () => modelsStore.availableModels,
    (options) => {
        if (options.length > 0 && !options.some(o => o.id === store.config.model)) {
            store.config.model = options[0].id;
        }
    },
    { immediate: true }
);

const languageOptions = [
    { label: 'English', code: 'en', flag: 'us' },
    { label: 'Vietnamese', code: 'vi', flag: 'vn' },
    { label: 'Chinese', code: 'zh-CN', flag: 'cn' },
    { label: 'Spanish', code: 'es', flag: 'es' },
    { label: 'French', code: 'fr', flag: 'fr' },
    { label: 'German', code: 'de', flag: 'de' },
    { label: 'Japanese', code: 'ja', flag: 'jp' },
    { label: 'Korean', code: 'ko', flag: 'kr' },
    { label: 'Russian', code: 'ru', flag: 'ru' },
    { label: 'Portuguese', code: 'pt', flag: 'pt' },
    { label: 'Italian', code: 'it', flag: 'it' },
    { label: 'Thai', code: 'th', flag: 'th' }
];

const currentLanguage = computed(() => {
    return languageOptions.find(l => l.code === store.config.language) || languageOptions[0];
});

const generate = async () => {
    await store.generate();
};

const generateRandomPrompt = async () => {
    if (isRandomizing.value) return;
    isRandomizing.value = true;
    try {
        const { promptSubjects, promptActions, promptSettings, promptMoods, promptLighting, promptStyles } = components;

        const subject = promptSubjects[Math.floor(Math.random() * promptSubjects.length)];
        const action = promptActions[Math.floor(Math.random() * promptActions.length)];
        const setting = promptSettings[Math.floor(Math.random() * promptSettings.length)];
        const mood = promptMoods[Math.floor(Math.random() * promptMoods.length)];
        const lighting = promptLighting[Math.floor(Math.random() * promptLighting.length)];
        const style = promptStyles[Math.floor(Math.random() * promptStyles.length)];

        const topic = `${subject} ${action} ${setting}, ${mood}, ${lighting}, ${style}`;

        let query = '';
        if (store.config.language !== 'en') {
            const langName = currentLanguage.value.label;
            query = `Tạo một mô tả prompt stable diffusion chi tiết dựa trên các yếu tố sau: Chủ thể "${subject}", Hành động "${action}", Bối cảnh "${setting}", Tâm trạng "${mood}", Ánh sáng "${lighting}", Phong cách "${style}". Chỉ trả về nội dung prompt bằng ${langName}, tuyệt đối không bao gồm lời dẫn, giải thích hay câu hỏi nào thêm. Thêm một chút chi tiết ngẫu nhiên để tạo sự khác biệt.`;
        } else {
            query = `Generate a detailed stable diffusion prompt based on: ${topic}. Be creative and add some unique details. Provide only the prompt text and nothing else. Do not ask questions.`;
        }

        // Cache buster to avoid output caching by the API
        const cacheBuster = Date.now();
        const response = await fetch(`https://t2t.manh9011.workers.dev/?prompt=${encodeURIComponent(query)}&_t=${cacheBuster}`);
        if (!response.ok) throw new Error('API request failed');

        let text = await response.text();

        text = text.split('\n')
            .filter(line => !line.toLowerCase().includes('hint image') && !line.toLowerCase().includes('pollen'))
            .join(' ')
            .trim();

        if (text) {
            store.config.prompt = text;
            await generate();
        }
    } catch (e) {
        console.error("Failed to generate random prompt", e);
    } finally {
        isRandomizing.value = false;
    }
};

const promptInputRef = ref();
const negativeInputRef = ref();
const focusTarget = ref<'prompt' | 'negative'>('prompt');

const openPromptDialog = (target: 'prompt' | 'negative') => {
    focusTarget.value = target;
    promptDialogVisible.value = true;
};

const onDialogShow = () => {
    setTimeout(() => {
        if (focusTarget.value === 'prompt' && promptInputRef.value?.$el) {
            const textarea = promptInputRef.value.$el;
            textarea.focus();
            // Try to set cursor to end
            const len = textarea.value.length;
            textarea.setSelectionRange(len, len);
        } else if (focusTarget.value === 'negative' && negativeInputRef.value?.$el) {
            const textarea = negativeInputRef.value.$el;
            textarea.focus();
            const len = textarea.value.length;
            textarea.setSelectionRange(len, len);
        }
    }, 100);
};

const previewPrompt = computed(() => {
    const style = artStyles[store.config.artStyle] || artStyles["No Style"];
    // Simulate config
    return style.prompt(store.config);
});

const previewNegative = computed(() => {
    const style = artStyles[store.config.artStyle] || artStyles["No Style"];
    return style.negative(store.config);
});

const escapeHtml = (text: string) => {
    if (!text) return '';
    const map: Record<string, string> = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.toString().replace(/[&<>"']/g, (m) => map[m]);
};

const highlightText = (fullText: string, fragment: string) => {
    if (!fullText) return '';
    const escapedFull = escapeHtml(fullText);
    if (!fragment) return escapedFull;

    const escapedFragment = escapeHtml(fragment);
    if (!escapedFragment) return escapedFull;

    // Split by the fragment and join with the highlighted version
    // This handles multiple occurrences
    return escapedFull.split(escapedFragment).join(`<span class="highlight">${escapedFragment}</span>`);
};

const formattedPreviewPrompt = computed(() => {
    return highlightText(previewPrompt.value, store.config.prompt);
});

const formattedPreviewNegative = computed(() => {
    return highlightText(previewNegative.value, store.config.negative);
});
</script>

<template>
    <div class="control-panel-content">
        <!-- Model + Language -->
        <div class="row">
            <Select v-model="store.config.model" :options="modelOptions" optionLabel="label" optionValue="value"
                placeholder="Model" class="flex-1">
                <template #option="slotProps">
                    <div class="model-option">
                        <span>{{ slotProps.option.label }}</span>
                        <div class="model-meta">
                            <Tag v-if="slotProps.option.isNew" value="NEW" severity="success" class="new-tag" />
                            <Tag v-if="slotProps.option.isCustom" value="CUSTOM" severity="info" class="new-tag" />
                            <span class="price-hint">{{ slotProps.option.priceHint }}</span>
                        </div>
                    </div>
                </template>
            </Select>
            <Button icon="pi pi-cog" v-tooltip.top="'Manage Models'" severity="secondary" size="small"
                @click="modelManagerVisible = true" />
            <Button @click="toggleLanguagePopover" severity="secondary" text class="language-trigger-btn"
                v-tooltip.top="'Prompt language'">
                <img :src="`https://flagcdn.com/w40/${currentLanguage.flag}.png`" :alt="currentLanguage.label"
                    class="current-flag-icon" />
            </Button>
            <Popover ref="languagePopover">
                <div class="language-options-grid">
                    <div v-for="lang in languageOptions" :key="lang.code" class="lang-option"
                        :class="{ active: store.config.language === lang.code }" @click="selectLanguage(lang.code)">
                        <img :src="`https://flagcdn.com/w20/${lang.flag}.png`" :alt="lang.label"
                            class="flag-icon-small" />
                        <span>{{ lang.label }}</span>
                    </div>
                </div>
            </Popover>
        </div>

        <!-- Prompts -->
        <div class="prompt-row">
            <Textarea v-model="store.config.prompt" rows="3" fluid class="prompt-input"
                :placeholder="store.config.language !== 'en' ? `Prompt (auto translate ${currentLanguage.label} to EN)` : 'Prompt'" />
            <div class="side-actions">
                <Button icon="pi pi-expand" severity="secondary" size="small" v-tooltip.top="'Expand'"
                    @click="openPromptDialog('prompt')" class="action-btn" />
                <Button v-if="store.config.prompt" icon="pi pi-times" severity="secondary" text size="small"
                    class="action-btn clear-btn" @click="store.config.prompt = ''" />
            </div>
        </div>

        <!-- Negative -->
        <div class="prompt-row">
            <Textarea v-model="store.config.negative" rows="3" autoResize fluid placeholder="Negative"
                class="prompt-input" />
            <div class="side-actions">
                <Button icon="pi pi-expand" severity="secondary" size="small" v-tooltip.top="'Expand'"
                    @click="openPromptDialog('negative')" class="action-btn" />
                <Button v-if="store.config.negative" icon="pi pi-times" severity="secondary" text size="small"
                    class="action-btn clear-btn" @click="store.config.negative = ''" />
            </div>
        </div>

        <!-- Style -->
        <div class="row">
            <Button :label="store.config.artStyle" icon="pi pi-palette" severity="secondary"
                class="flex-1 style-trigger-btn" @click="toggleStylePicker" />
            <Popover ref="stylePickerPopover">
                <div class="style-picker-container">
                    <StylePicker v-model="store.config.artStyle" @select="onStyleSelect" />
                </div>
            </Popover>
            <Button icon="pi pi-cog" v-tooltip.top="'Manage Styles'" severity="secondary" size="small"
                @click="styleManagerVisible = true" />
        </div>

        <!-- CFG -->
        <div class="row">
            <span class="hint">CFG {{ store.config.guidanceScale }}</span>
            <Slider v-model="store.config.guidanceScale" :min="1" :max="20" :step="0.5" class="flex-1" />
        </div>

        <!-- Count -->
        <div class="row">
            <label for="count" class="hint" style="cursor: pointer; min-width: 3rem;">Count {{ store.config.count
                }}</label>
            <Slider v-model="store.config.count" :min="1" :max="30" :step="1" class="flex-1" />
        </div>


        <!-- Seed -->
        <div class="row">
            <Checkbox v-model="store.config.useSeed" :binary="true" inputId="useSeed" />
            <label for="useSeed" class="hint" style="cursor: pointer; min-width: 2rem;">Seed</label>
            <InputNumber v-model="store.config.seed" :disabled="!store.config.useSeed" :useGrouping="false"
                :placeholder="store.config.useSeed ? '' : 'Random'" class="flex-1" />
        </div>

        <!-- Dimensions & Generate -->
        <div class="row">
            <div class="button-group">
                <Button icon="pi pi-mobile" @click="store.config.width = 768; store.config.height = 1024"
                    v-tooltip.top="'Portrait'" severity="secondary" size="small"
                    :outlined="!(store.config.width === 768 && store.config.height === 1024)" />
                <Button icon="pi pi-stop" @click="store.config.width = 1024; store.config.height = 1024"
                    v-tooltip.top="'Square'" severity="secondary" size="small"
                    :outlined="!(store.config.width === 1024 && store.config.height === 1024)" />
                <Button icon="pi pi-tablet" @click="store.config.width = 1024; store.config.height = 768"
                    v-tooltip.top="'Landscape'" severity="secondary" size="small"
                    :outlined="!(store.config.width === 1024 && store.config.height === 768)" />
            </div>
            <div class="flex-1" style="display: flex; gap: 0.5rem">
                <Button icon="pi pi-lightbulb" severity="warn" v-tooltip.top="'Random Prompt'" :loading="isRandomizing"
                    @click="generateRandomPrompt" />
                <Button label="Generate" icon="pi pi-bolt" @click="generate" :loading="store.isGenerating" fluid
                    class="flex-1" />
            </div>
        </div>

        <!-- Style Manager Dialog -->
        <StyleManager v-model:visible="styleManagerVisible" />

        <!-- Model Manager Dialog -->
        <ModelManager v-model:visible="modelManagerVisible" />

        <!-- Prompt Expand Dialog -->
        <Dialog v-model:visible="promptDialogVisible" header="Edit Prompt & Preview" modal maximizable
            class="prompt-edit-dialog" @show="onDialogShow"
            :contentStyle="{ width: '80vw', height: '80vh', display: 'flex', flexDirection: 'column' }">
            <template #header>
                <div class="dialog-header-content">
                    <span class="p-dialog-title">Edit Prompt & Preview</span>
                    <div class="header-style-selector">
                        <Button :label="store.config.artStyle" icon="pi pi-palette" severity="secondary" size="small"
                            class="style-trigger-btn header-btn" @click="toggleDialogStylePicker" />
                        <Popover ref="dialogStylePickerPopover">
                            <div class="style-picker-container">
                                <StylePicker v-model="store.config.artStyle" @select="onDialogStyleSelect" />
                            </div>
                        </Popover>
                    </div>
                </div>
            </template>

            <div class="dialog-grid">
                <!-- Row 1: Headers -->
                <div class="grid-col-left dialog-label">Prompt</div>
                <div class="grid-col-right dialog-label preview-label-text">Final Prompt Preview</div>

                <!-- Row 2: Prompt Content -->
                <div class="grid-col-left">
                    <Textarea ref="promptInputRef" v-model="store.config.prompt" class="expanded-textarea" fluid
                        :placeholder="store.config.language !== 'en' ? `Prompt (tự dịch từ ${currentLanguage.label} sang EN)` : 'Enter your prompt here...'" />
                </div>
                <div class="grid-col-right">
                    <div class="preview-box" v-html="formattedPreviewPrompt"></div>
                </div>

                <!-- Row 3: Headers -->
                <div class="grid-col-left dialog-label">Negative</div>
                <div class="grid-col-right dialog-label preview-label-text">Final Negative Preview</div>

                <!-- Row 4: Negative Content -->
                <div class="grid-col-left">
                    <Textarea ref="negativeInputRef" v-model="store.config.negative" class="expanded-textarea small"
                        fluid placeholder="Negative prompt..." />
                </div>
                <div class="grid-col-right">
                    <div class="preview-box small" v-html="formattedPreviewNegative"></div>
                </div>
            </div>

            <template #footer>
                <Button label="Done" icon="pi pi-check" @click="promptDialogVisible = false" />
            </template>
        </Dialog>
    </div>
</template>

<style scoped>
.control-panel-content {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.row {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.flex-1 {
    flex: 1;
}

.hint {
    font-size: 0.75rem;
    color: var(--p-surface-400);
}

.model-option {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
}

.model-meta {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.new-tag {
    font-size: 0.5rem;
    padding: 0.1rem 0.3rem;
}

.price-hint {
    font-size: 0.7rem;
    color: var(--p-surface-400);
}

.cfg-control {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    min-width: 5rem;
}

.cfg-slider {
    width: 100%;
}

.dim-text {
    font-size: 0.875rem;
    color: var(--p-surface-300);
}

.count-input {
    width: 4rem;
}

.prompt-row {
    display: flex;
    gap: 0.25rem;
    align-items: flex-start;
}

.prompt-input {
    flex: 1;
    max-height: 4.5rem;
    overflow-y: auto !important;
    resize: none !important;
}

.side-actions {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    flex-shrink: 0;
}

.action-btn {
    width: 2rem !important;
    height: 1.5rem !important;
    min-width: 0 !important;
    padding: 0 !important;
}

.clear-btn {
    color: var(--p-surface-400) !important;
}

.clear-btn:hover {
    color: var(--p-red-400) !important;
    background: transparent !important;
}

.language-selector {
    flex-shrink: 0;
}

.current-flag-icon {
    width: 1.75rem;
    height: auto;
    display: block;
    border-radius: 2px;
}

.language-trigger-btn {
    padding: 0.25rem 0.5rem !important;
}

.language-options-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 0.25rem;
    padding: 0.25rem;
    min-width: 250px;
}

.lang-option {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 0.5rem 0.75rem;
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.2s;
    font-size: 0.9rem;
}

.lang-option:hover {
    background: var(--p-surface-700);
}

.lang-option.active {
    background: var(--p-primary-900);
    color: var(--p-primary-100);
}

.flag-icon-small {
    width: 1.25rem;
    height: auto;
    border-radius: 2px;
}

.prompt-edit-dialog {
    width: 80vw;
    max-width: 1400px;
    height: 80vh;
}

/* Dialog Header Customization */
.dialog-header-content {
    display: flex;
    align-items: center;
    width: 100%;
    position: relative;
    padding-right: 2rem;
    /* Make space for close button which is absolute usually, or just to be safe */
}

.header-style-selector {
    position: absolute;
    left: 50%;
    transform: translateX(-50%);
    z-index: 10;
}

.header-btn {
    font-size: 0.85rem;
    padding: 0.25rem 0.75rem !important;
}

/* Dialog Grid Layout */
.dialog-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-template-rows: auto 1fr auto auto;
    /* Removed the first 'auto' row */
    gap: 0.5rem 1rem;
    flex: 1;
    min-height: 0;
    overflow-y: auto;
    padding-right: 0.5rem;
}

.grid-full-width {
    grid-column: 1 / -1;
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1rem;
}

.style-select-row {
    max-width: 400px;
}

.dialog-style-btn {
    width: 100%;
}

.style-picker-container {
    width: 600px;
    padding: 0.5rem;
}

.style-trigger-btn {
    justify-content: flex-start;
    text-align: left;
    overflow: hidden;
    text-overflow: ellipsis;
    border: 1px solid var(--p-surface-700) !important;
    background: var(--p-surface-800) !important;
    padding: 0.5rem 0.75rem !important;
}

.style-trigger-btn:hover {
    border-color: var(--p-primary-500) !important;
}

.grid-col-left {
    grid-column: 1;
}

.grid-col-right {
    grid-column: 2;
}

.dialog-label {
    font-size: 0.85rem;
    font-weight: 600;
    color: var(--p-surface-400);
    margin-bottom: 0.25rem;
}

.preview-label-text {
    color: var(--p-primary-400);
}

/* Textareas and Boxes */
.expanded-textarea {
    height: 100% !important;
    resize: none;
    font-family: monospace;
}

.expanded-textarea.small {
    min-height: 8rem;
    height: auto !important;
}

.preview-box {
    background: var(--p-surface-800);
    color: var(--p-surface-200);
    border: 1px solid var(--p-surface-700);
    padding: 0.75rem;
    border-radius: 6px;
    font-family: monospace;
    font-size: 0.85rem;
    line-height: 1.4;
    white-space: pre-wrap;
    word-break: break-word;
    height: 100%;
    min-height: 100px;
    overflow-y: auto;
}

.preview-box.small {
    min-height: 8rem;
    height: auto;
}

@media (max-width: 768px) {
    .dialog-grid {
        grid-template-columns: 1fr;
        grid-template-rows: auto;
        display: flex;
        flex-direction: column;
        gap: 1rem;
    }

    .grid-col-left,
    .grid-col-right {
        grid-column: 1;
    }

    .expanded-textarea {
        height: 15rem !important;
    }

    .header-style-selector {
        position: static;
        transform: none;
        margin-left: auto;
    }
}

:deep(.highlight) {
    color: var(--p-primary-400);
    font-weight: bold;
    background-color: color-mix(in srgb, var(--p-primary-400), transparent 85%);
    border-radius: 4px;
    padding: 0 4px;
}
</style>
