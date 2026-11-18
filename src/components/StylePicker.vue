<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useGeneratorStore } from '../stores/generatorStore';
import { useModelsStore } from '../stores/modelsStore';
import { useStylesStore } from '../stores/stylesStore';
import { styleList } from '../constants/artStyles';
import { generateImage } from '../utils/textToImageApi';
import { saveToCache, loadFromCache } from '../utils/imageCache';
import { getBuiltInStyleThumbnail, getStyleThumbSlug } from '../utils/styleThumbnails';
import ProgressSpinner from 'primevue/progressspinner';
import InputText from 'primevue/inputtext';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';

defineProps<{
    modelValue: string;
}>();

const emit = defineEmits<{
    (e: 'update:modelValue', value: string): void;
    (e: 'select', value: string): void;
}>();

const stylesStore = useStylesStore();
const generatorStore = useGeneratorStore();
const modelsStore = useModelsStore();

const searchQuery = ref('');
const thumbnails = ref<Record<string, string>>({});
const isLoadingThumbnails = ref(false);

const allStyles = computed(() => {
    const builtIn = styleList.map(name => ({ name, isBuiltIn: true }));
    const custom = stylesStore.customStyles.map(s => ({ name: s.name, isBuiltIn: false }));
    return [...builtIn, ...custom];
});

const filteredStyles = computed(() => {
    if (!searchQuery.value) return allStyles.value;
    const query = searchQuery.value.toLowerCase();
    return allStyles.value.filter(s => s.name.toLowerCase().includes(query));
});

const generateThumbnailId = (styleName: string) => `style-thumb-${getStyleThumbSlug(styleName)}`;

const getThumbnailPrompt = (styleName: string) => {
    const lower = styleName.toLowerCase();
    if (lower.includes('photo') || lower.includes('cinematic') || lower.includes('film')) {
        return `A stunning portrait of a young woman with lighting, ${styleName} style, masterpiece, 8k, high detailed`;
    }
    return `A masterpiece representation of ${styleName}, beautiful scenery or character, detailed`;
};

const generateAndCacheThumbnail = async (styleName: string) => {
    const id = generateThumbnailId(styleName);
    try {
        const thumbnailModel = modelsStore.getCustomModelById(generatorStore.config.model)
            ? 'zimage'
            : generatorStore.config.model;
        const blob = await generateImage({
            prompt: getThumbnailPrompt(styleName),
            negative: "camera, lens, holding camera, photographer, (worst quality, low quality, normal quality, lowres, low details, oversaturated, undersaturated, overexposed, underexposed, grayscale, bw, bad photo, bad photography, bad art:1.4), (watermark, signature, text font, username, error, logo, words, letters, digits, autograph, trademark, name:1.2), (blur, blurry, grainy), morbid, ugly, asymmetrical, mutated malformed, mutilated, poorly lit, bad shadow, draft, cropped, out of frame, cut off, censored, jpeg artifacts, out of focus, glitch, duplicate, (airbrushed, cartoon, anime, semi-realistic, cgi, render, blender, digital art, manga, sketch, 3d, dual exposure, bad transition, watermark, face, distortion:0.5)",
            artStyle: styleName,
            model: thumbnailModel,
            guidanceScale: 5,
            width: 256,
            height: 256,
            count: 1,
            seed: 42, // Consistent seed for same style
            useSeed: true,
            language: 'en'
        }, { throwOnError: true });

        if (blob) {
            await saveToCache(id, blob);
            thumbnails.value[styleName] = URL.createObjectURL(blob);
        }
    } catch (e) {
        console.error(`Failed to generate thumbnail for ${styleName}`, e);
    }
};

const loadThumbnails = async () => {
    if (isLoadingThumbnails.value) return;
    isLoadingThumbnails.value = true;

    try {
        // Load bundled static assets for all built-in styles without calling API
        for (const style of allStyles.value) {
            if (style.isBuiltIn && style.name !== 'No Style' && !thumbnails.value[style.name]) {
                const assetUrl = getBuiltInStyleThumbnail(style.name);
                if (assetUrl) {
                    thumbnails.value[style.name] = assetUrl;
                }
            }
        }

        const customStylesToProcess = allStyles.value.filter(
            s => !s.isBuiltIn && !thumbnails.value[s.name]
        );

        // 1. Parallel cache check for custom styles
        const missingStyles: string[] = [];
        await Promise.all(customStylesToProcess.map(async (style) => {
            const id = generateThumbnailId(style.name);
            const cachedUrl = await loadFromCache(id);
            if (cachedUrl) {
                thumbnails.value[style.name] = cachedUrl;
            } else {
                missingStyles.push(style.name);
            }
        }));

        // 2. Generate missing custom style thumbnails in parallel batches
        if (missingStyles.length > 0) {
            const BATCH_SIZE = 6;
            for (let i = 0; i < missingStyles.length; i += BATCH_SIZE) {
                const batch = missingStyles.slice(i, i + BATCH_SIZE);
                await Promise.all(batch.map(styleName => generateAndCacheThumbnail(styleName)));
            }
        }
    } finally {
        isLoadingThumbnails.value = false;
    }
};

onMounted(() => {
    loadThumbnails();
});

// Re-load if custom styles change
watch(() => stylesStore.customStyles, () => {
    loadThumbnails();
}, { deep: true });

const selectStyle = (name: string) => {
    emit('update:modelValue', name);
    emit('select', name);
};
</script>

<template>
    <div class="style-picker">
        <div class="search-wrapper">
            <IconField class="w-full">
                <InputIcon class="pi pi-search" />
                <InputText v-model="searchQuery" placeholder="Search styles..." fluid size="small" />
            </IconField>
        </div>

        <div class="style-grid">
            <div v-for="style in filteredStyles" :key="style.name" class="style-item"
                :class="{ 'active': style.name === modelValue }" @click="selectStyle(style.name)">
                <div class="style-thumb-wrapper">
                    <div v-if="style.name === 'No Style'" class="thumb-placeholder no-style">
                        <i class="pi pi-ban"></i>
                    </div>
                    <img v-else-if="thumbnails[style.name]" :src="thumbnails[style.name]" :alt="style.name"
                        loading="lazy" />
                    <div v-else class="thumb-placeholder">
                        <ProgressSpinner style="width: 20px; height: 20px" strokeWidth="4" />
                    </div>

                    <div class="style-badge" v-if="!style.isBuiltIn">
                        Custom
                    </div>

                    <div class="active-overlay" v-if="style.name === modelValue">
                        <i class="pi pi-check-circle"></i>
                    </div>
                </div>
                <div class="style-info">
                    <span class="style-name">{{ style.name }}</span>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.style-picker {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    max-height: 400px;
    width: 100%;
}

.search-wrapper {
    padding: 2px;
}

.style-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
    gap: 0.75rem;
    overflow-y: auto;
    padding: 2px;
    scrollbar-width: thin;
}

.style-item {
    display: flex;
    flex-direction: column;
    gap: 0.4rem;
    cursor: pointer;
    transition: transform 0.1s;
}

.style-item:hover {
    transform: translateY(-2px);
}

.style-thumb-wrapper {
    width: 100%;
    aspect-ratio: 1;
    border-radius: 8px;
    overflow: hidden;
    position: relative;
    background: var(--p-surface-800);
    border: 2px solid transparent;
    transition: all 0.2s;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.style-item.active .style-thumb-wrapper {
    border-color: var(--p-primary-500);
    box-shadow: 0 0 8px var(--p-primary-500);
}

.style-thumb-wrapper img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.thumb-placeholder {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: var(--p-surface-700);
}

.no-style {
    background: var(--p-surface-600);
}

.no-style i {
    font-size: 2rem;
    color: var(--p-surface-400);
}

.style-info {
    text-align: center;
}

.style-name {
    font-size: 0.75rem;
    font-weight: 500;
    color: var(--p-surface-100);
    display: -webkit-box;
    -webkit-line-clamp: 2;
    line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
    line-height: 1.2;
}

.style-badge {
    position: absolute;
    top: 4px;
    right: 4px;
    background: rgba(0, 0, 0, 0.6);
    color: white;
    font-size: 0.6rem;
    padding: 1px 4px;
    border-radius: 4px;
    backdrop-filter: blur(2px);
}

.active-overlay {
    position: absolute;
    top: 4px;
    left: 4px;
    color: var(--p-primary-400);
    font-size: 1rem;
    filter: drop-shadow(0 0 2px rgba(0, 0, 0, 0.5));
}

@media (max-width: 480px) {
    .style-grid {
        grid-template-columns: repeat(3, 1fr);
    }
}
</style>
