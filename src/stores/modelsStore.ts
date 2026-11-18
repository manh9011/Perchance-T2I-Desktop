import { defineStore } from 'pinia';
import { models as builtInModelList, type ModelInfo } from '../constants/models';

export type ResponseOutputType = 'json' | 'image';

export interface CustomModel {
    id: string;
    name: string;
    url: string;
    method: 'POST' | 'GET';
    headers: string;
    payload: string;
    responseType: ResponseOutputType;
    imageField: string;
    errorField: string;
    cfgMultiplier?: number;
    enabled: boolean;
    isBuiltIn: false;
}

export interface ModelPreset {
    id: string;
    label: string;
    model: Omit<CustomModel, 'isBuiltIn'>;
}

export interface SelectableModel {
    id: string;
    name: string;
    isBuiltIn: boolean;
    isNew?: boolean;
}

export const MODEL_PRESETS: ModelPreset[] = [
    {
        id: 'colab-zimage-turbo',
        label: 'Colab Z-Image-Turbo (:2504)',
        model: {
            id: 'colab-zimage-turbo',
            name: 'Colab Z-Image-Turbo',
            url: 'http://127.0.0.1:2504/api/generate',
            method: 'POST',
            headers: JSON.stringify(
                {
                    'Content-Type': 'application/json',
                },
                null,
                2
            ),
            payload: JSON.stringify(
                {
                    positive_prompt: '{prompt}',
                    negative_prompt: '{negative}',
                    aspect_ratio: '{ratio}',
                    steps: 8,
                    cfg: 1.0,
                    seed: '{seed_raw}',
                    variant: 'Turbo',
                    quant: 'Q6',
                    response_format: 'b64_json',
                },
                null,
                2
            ).replace('"{seed_raw}"', '{seed}'),
            responseType: 'json',
            imageField: 'b64_json',
            errorField: 'detail',
            enabled: true,
        },
    },
    {
        id: 'colab-qwen-image-2-1',
        label: 'Colab Qwen-Image-2.1 (:2505)',
        model: {
            id: 'colab-qwen-image-2-1',
            name: 'Colab Qwen-Image-2.1',
            url: 'http://127.0.0.1:2505/api/generate',
            method: 'POST',
            headers: JSON.stringify(
                {
                    'Content-Type': 'application/json',
                },
                null,
                2
            ),
            payload: JSON.stringify(
                {
                    positive_prompt: '{prompt}',
                    negative_prompt: '{negative}',
                    aspect_ratio: '{ratio}',
                    steps: 30,
                    cfg: 6.0,
                    seed: '{seed_raw}',
                    quant: 'Q6',
                    encoder_quant: 'UD-Q4_K_XL',
                    response_format: 'b64_json',
                },
                null,
                2
            ).replace('"{seed_raw}"', '{seed}'),
            responseType: 'json',
            imageField: 'b64_json',
            errorField: 'detail',
            enabled: true,
        },
    },
    {
        id: 'colab-minimax-h3-image',
        label: 'Colab MiniMax H3-Image (:2506)',
        model: {
            id: 'colab-minimax-h3-image',
            name: 'Colab MiniMax H3-Image',
            url: 'http://127.0.0.1:2506/api/generate',
            method: 'POST',
            headers: JSON.stringify(
                {
                    'Content-Type': 'application/json',
                },
                null,
                2
            ),
            payload: JSON.stringify(
                {
                    positive_prompt: '{prompt}',
                    negative_prompt: '{negative}',
                    aspect_ratio: '{ratio}',
                    steps: 6,
                    cfg: 1.0,
                    seed: '{seed_raw}',
                    quant: 'Q4',
                    encoder: 'compact_4b_int8',
                    response_format: 'b64_json',
                },
                null,
                2
            ).replace('"{seed_raw}"', '{seed}'),
            responseType: 'json',
            imageField: 'b64_json',
            errorField: 'detail',
            enabled: true,
        },
    },
    {
        id: 'openai-images-preset',
        label: 'OpenAI /v1/images/generations',
        model: {
            id: 'openai-images-preset',
            name: 'OpenAI Image Gen',
            url: 'http://127.0.0.1:2504/v1/images/generations',
            method: 'POST',
            headers: JSON.stringify(
                {
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer YOUR_API_KEY',
                },
                null,
                2
            ),
            payload: JSON.stringify(
                {
                    model: 'z-image-turbo',
                    prompt: '{prompt}',
                    negative_prompt: '{negative}',
                    size: '{width}x{height}',
                    steps: 8,
                    cfg: '{cfg_raw}',
                    seed: '{seed_raw}',
                    response_format: 'b64_json',
                },
                null,
                2
            ).replace('"{cfg_raw}"', '{cfg}').replace('"{seed_raw}"', '{seed}'),
            responseType: 'json',
            imageField: 'data[0].b64_json',
            errorField: 'error.message',
            enabled: true,
        },
    },
    {
        id: 'direct-image-preset',
        label: 'Direct Image Stream',
        model: {
            id: 'direct-image-preset',
            name: 'Colab Direct Image',
            url: 'http://127.0.0.1:2504/api/generate',
            method: 'POST',
            headers: JSON.stringify(
                {
                    'Content-Type': 'application/json',
                },
                null,
                2
            ),
            payload: JSON.stringify(
                {
                    positive_prompt: '{prompt}',
                    negative_prompt: '{negative}',
                    aspect_ratio: '{ratio}',
                    steps: 8,
                    cfg: '{cfg_raw}',
                    seed: '{seed_raw}',
                    response_format: 'image',
                },
                null,
                2
            ).replace('"{cfg_raw}"', '{cfg}').replace('"{seed_raw}"', '{seed}'),
            responseType: 'image',
            imageField: '',
            errorField: 'detail',
            enabled: true,
        },
    },
];

const COLAB_PRESET_IDS = [
    'colab-zimage-turbo',
    'colab-qwen-image-2-1',
    'colab-minimax-h3-image',
];

function createDefaultColabModels(): CustomModel[] {
    return MODEL_PRESETS
        .filter(p => COLAB_PRESET_IDS.includes(p.id))
        .map(p => ({
            ...p.model,
            isBuiltIn: false as const,
        }));
}

interface ModelsState {
    builtInEnabled: Record<string, boolean>;
    customModels: CustomModel[];
}

export const useModelsStore = defineStore('models', {
    state: (): ModelsState => ({
        builtInEnabled: Object.fromEntries(builtInModelList.map(m => [m.id, true])),
        customModels: createDefaultColabModels(),
    }),

    getters: {
        builtInModels: (state): (ModelInfo & { enabled: boolean })[] => {
            return builtInModelList.map(m => ({
                ...m,
                enabled: state.builtInEnabled[m.id] !== false,
            }));
        },

        availableModels(state): SelectableModel[] {
            const enabledBuiltIn: SelectableModel[] = builtInModelList
                .filter(m => state.builtInEnabled[m.id] !== false)
                .map(m => ({
                    id: m.id,
                    name: m.name,
                    isBuiltIn: true,
                    isNew: m.isNew,
                }));

            const enabledCustom: SelectableModel[] = state.customModels
                .filter(m => m.enabled)
                .map(m => ({
                    id: m.id,
                    name: m.name,
                    isBuiltIn: false,
                }));

            return [...enabledBuiltIn, ...enabledCustom];
        },

        getCustomModelById: (state) => (id: string): CustomModel | undefined => {
            return state.customModels.find(m => m.id === id);
        },

        getModelName: (state) => (id: string): string => {
            const builtIn = builtInModelList.find(m => m.id === id);
            if (builtIn) return builtIn.name;
            const custom = state.customModels.find(m => m.id === id);
            if (custom) return custom.name;
            return id;
        },
    },

    actions: {
        setBuiltInEnabled(id: string, enabled: boolean) {
            this.builtInEnabled = {
                ...this.builtInEnabled,
                [id]: enabled,
            };
        },

        addCustomModel(model: Omit<CustomModel, 'id' | 'isBuiltIn'> & { id?: string }): CustomModel {
            const newModel: CustomModel = {
                id: model.id || `custom-${crypto.randomUUID()}`,
                name: model.name.trim(),
                url: model.url.trim(),
                method: model.method || 'POST',
                headers: model.headers || '',
                payload: model.payload || '',
                responseType: model.responseType || 'json',
                imageField: model.imageField ?? '',
                errorField: model.errorField ?? '',
                cfgMultiplier: typeof model.cfgMultiplier === 'number' && model.cfgMultiplier > 0 ? model.cfgMultiplier : 1,
                enabled: model.enabled ?? true,
                isBuiltIn: false,
            };
            this.customModels.push(newModel);
            return newModel;
        },

        updateCustomModel(id: string, updates: Partial<Omit<CustomModel, 'id' | 'isBuiltIn'>>) {
            const index = this.customModels.findIndex(m => m.id === id);
            if (index !== -1) {
                this.customModels[index] = {
                    ...this.customModels[index],
                    ...updates,
                };
            }
        },

        deleteCustomModel(id: string) {
            const index = this.customModels.findIndex(m => m.id === id);
            if (index !== -1) {
                this.customModels.splice(index, 1);
            }
        },

        duplicateCustomModel(model: CustomModel): CustomModel {
            return this.addCustomModel({
                name: `${model.name} (Copy)`,
                url: model.url,
                method: model.method,
                headers: model.headers,
                payload: model.payload,
                responseType: model.responseType,
                imageField: model.imageField,
                errorField: model.errorField,
                cfgMultiplier: model.cfgMultiplier,
                enabled: model.enabled,
            });
        },

        restoreColabPresets(): number {
            let updated = 0;
            for (const preset of MODEL_PRESETS) {
                if (!COLAB_PRESET_IDS.includes(preset.id)) continue;
                const existing = this.customModels.find(m => m.id === preset.model.id);
                if (!existing) {
                    this.addCustomModel({ ...preset.model });
                    updated++;
                } else if (existing.payload.includes('"cfg": {cfg}')) {
                    const defaultCfg = preset.id === 'colab-qwen-image-2-1' ? '6.0' : '1.0';
                    existing.payload = existing.payload.replace('"cfg": {cfg}', `"cfg": ${defaultCfg}`);
                    updated++;
                }
            }
            return updated;
        },
    },
});
