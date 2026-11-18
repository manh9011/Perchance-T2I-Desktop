<script setup lang="ts">
import { ref, computed } from 'vue';
import { useGeneratorStore, type PromptGroup } from '../stores/generatorStore';
import Button from 'primevue/button';
import SelectButton from 'primevue/selectbutton';
import InputText from 'primevue/inputtext';
import { useConfirm } from 'primevue/useconfirm';
import { useModelsStore } from '../stores/modelsStore';

const store = useGeneratorStore();
const modelsStore = useModelsStore();
const confirm = useConfirm();

const sortBy = ref<'date' | 'alpha' | 'likes'>('date');
const sortOptions = [
    { icon: 'pi pi-calendar', value: 'date' },
    { icon: 'pi pi-sort-alpha-down', value: 'alpha' },
    { icon: 'pi pi-heart-fill', value: 'likes', color: 'var(--p-red-500)' }
];

const searchQuery = ref('');
const isSearchVisible = ref(false);
const searchInput = ref<any>(null);

const showSearch = () => {
    isSearchVisible.value = true;
    setTimeout(() => {
        const el = searchInput.value?.$el || searchInput.value;
        if (el) {
            const input = el.tagName === 'INPUT' ? el : el.querySelector('input');
            input?.focus();
        }
    }, 50);
};

const handleBlur = () => {
    isSearchVisible.value = false;
};

const clearSearch = (event: Event) => {
    event.stopPropagation();
    searchQuery.value = '';
    isSearchVisible.value = false;
};

// Get total likes for a group
const getGroupLikes = (group: PromptGroup): number => {
    return group.images.reduce((sum, img) => sum + (img.likes || 0), 0);
};

// Get model name for a group
const getModelName = (group: PromptGroup): string => {
    return modelsStore.getModelName(group.config.model);
};

const groups = computed(() => {
    let allGroups = [...store.promptGroups];

    if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase();
        allGroups = allGroups.filter(g =>
            g.config.prompt.toLowerCase().includes(query) ||
            getModelName(g).toLowerCase().includes(query) ||
            g.config.artStyle.toLowerCase().includes(query)
        );
    }

    if (sortBy.value === 'alpha') {
        return allGroups.sort((a, b) => a.config.prompt.localeCompare(b.config.prompt));
    }
    if (sortBy.value === 'likes') {
        return allGroups.sort((a, b) => getGroupLikes(b) - getGroupLikes(a));
    }
    return allGroups.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
});

const selectGroup = (group: PromptGroup) => {
    store.selectGroup(group);
};

const deleteGroup = (group: PromptGroup, event: Event) => {
    event.stopPropagation();
    confirm.require({
        message: `Delete all images with this prompt?`,
        header: 'Delete Prompt Group',
        icon: 'pi pi-exclamation-triangle',
        acceptClass: 'p-button-danger',
        accept: () => {
            store.deletePromptGroup(group.id);
        }
    });
};

const getThumbnail = (group: PromptGroup): string => {
    const thumb = store.getGroupThumbnail(group);
    return thumb?.path || '';
};

const truncatePrompt = (text: string, maxLen: number = 50): string => {
    return text.length > maxLen ? text.substring(0, maxLen) + '...' : text;
};
</script>

<template>
    <div class="prompt-list">
        <div class="prompt-list-header" :class="{ 'search-active': isSearchVisible }">
            <template v-if="!isSearchVisible">
                <div class="header-main">
                    <span class="title">Prompts</span>
                    <div class="filter-btn-container">
                        <Button icon="pi pi-search" size="small" severity="secondary" text @click="showSearch"
                            class="filter-toggle-btn" v-tooltip.top="'Search prompts'" />
                        <div v-if="searchQuery" class="clear-badge" @click="clearSearch" v-tooltip.top="'Clear search'">
                            <i class="pi pi-times" style="font-size: 0.4rem;"></i>
                        </div>
                    </div>
                </div>
                <SelectButton v-model="sortBy" :options="sortOptions" optionLabel="value" optionValue="value"
                    :allowEmpty="false" size="small" class="order-select">
                    <template #option="slotProps">
                        <i :class="slotProps.option.icon" :style="{ color: slotProps.option.color }"></i>
                    </template>
                </SelectButton>
            </template>
            <template v-else>
                <div class="search-wrapper">
                    <InputText ref="searchInput" v-model="searchQuery" placeholder="Search prompts..." size="small"
                        fluid class="search-input" @blur="handleBlur" />
                </div>
            </template>
        </div>

        <div class="prompt-list-content">
            <div v-for="group in groups" :key="group.id" class="prompt-item"
                :class="{ selected: store.selectedGroup?.id === group.id }" @click="selectGroup(group)">
                <div class="prompt-thumb">
                    <img v-if="getThumbnail(group)" :src="getThumbnail(group)" alt="thumbnail" />
                    <div v-else class="thumb-placeholder">
                        <i class="pi pi-image"></i>
                    </div>
                </div>
                <div class="prompt-info">
                    <div class="prompt-text">{{ truncatePrompt(group.config.prompt) }}</div>
                    <div class="prompt-meta">
                        <div class="meta-row">
                            <span class="meta-item">
                                <i class="pi pi-image"></i>
                                {{ group.images.length }}
                            </span>
                            <span v-if="getGroupLikes(group) > 0" class="likes-badge meta-item">
                                <i class="pi pi-heart-fill"></i>
                                {{ getGroupLikes(group) }}
                            </span>
                            <span class="meta-item truncate">{{ group.config.artStyle }} • CFG {{
                                group.config.guidanceScale
                            }}</span>
                        </div>
                    </div>
                    <div class="prompt-footer">
                        <div class="model-name">{{ getModelName(group) }}</div>
                        <div class="footer-meta">
                            <span class="dims">{{ group.config.width }}×{{ group.config.height }}</span>
                            <img :src="`https://flagcdn.com/w20/${group.config.language === 'en' ? 'us' : 'vn'}.png`"
                                class="flag-icon-mini"
                                v-tooltip="group.config.language === 'en' ? 'English' : 'Vietnamese'" />
                        </div>
                    </div>
                </div>
                <Button icon="pi pi-trash" size="small" severity="danger" text class="delete-btn"
                    @click="deleteGroup(group, $event)" v-tooltip.left="'Delete all'" />
            </div>

            <div v-if="groups.length === 0" class="empty-state">
                <i class="pi pi-images"></i>
                <span>No prompts yet</span>
            </div>
        </div>
    </div>
</template>

<style scoped>
.prompt-list {
    display: flex;
    flex-direction: column;
    height: 100%;
    min-height: 0;
}

.prompt-list-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.5rem;
    border-bottom: 1px solid var(--p-surface-700);
    min-height: 2.75rem;
}

.header-main {
    display: flex;
    align-items: center;
    gap: 0.25rem;
    flex: 1;
}

.filter-btn-container {
    position: relative;
    display: flex;
    align-items: center;
}

.clear-badge {
    position: absolute;
    top: -1px;
    right: -1px;
    width: 12px;
    height: 12px;
    background: #ef4444;
    /* red-500 */
    color: white;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 6px;
    /* Smaller x */
    cursor: pointer;
    border: 1px solid var(--p-surface-900);
    z-index: 2;
    box-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
    transition: all 0.1s;
}

.clear-badge:hover {
    background: #dc2626;
}

.search-wrapper {
    width: 100%;
}

.search-input {
    width: 100%;
}

.search-active {
    padding-left: 0.5rem;
    padding-right: 0.5rem;
}

.title {
    font-weight: 600;
    font-size: 0.875rem;
}

.filter-toggle-btn {
    width: 2rem !important;
    height: 2rem !important;
}

.order-select {
    flex-shrink: 0;
}

:deep(.p-selectbutton-button) {
    height: 1.75rem;
    padding: 0 0.5rem;
}

.prompt-list-content {
    flex: 1;
    overflow-y: auto;
    padding: 0.75rem;
}

.prompt-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem;
    border-radius: 0.5rem;
    cursor: pointer;
    transition: background-color 0.15s;
}

.prompt-item:hover {
    background-color: var(--p-surface-700);
}

.prompt-item.selected {
    background-color: var(--p-surface-600);
}

.prompt-thumb {
    width: 3rem;
    height: 3rem;
    flex-shrink: 0;
    border-radius: 0.25rem;
    overflow: hidden;
    background: var(--p-surface-700);
}

.prompt-thumb img {
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
    color: var(--p-surface-400);
}

.prompt-info {
    flex: 1;
    min-width: 0;
    overflow: hidden;
    display: flex;
    flex-direction: column;
    gap: 0.15rem;
}

.prompt-text {
    font-size: 0.8rem;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.prompt-meta {
    display: flex;
    flex-direction: column;
    gap: 0.1rem;
    font-size: 0.65rem;
    color: var(--p-surface-400);
}

.meta-row {
    display: flex;
    align-items: center;
    gap: 0.4rem;
}

.prompt-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: auto;
    padding-top: 0.2rem;
}

.footer-meta {
    display: flex;
    align-items: center;
    gap: 0.4rem;
}

.meta-item {
    display: flex;
    align-items: center;
    gap: 0.2rem;
}

.meta-item i {
    font-size: 0.6rem;
}

.dims {
    opacity: 0.7;
    font-size: 0.6rem;
}

.truncate {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.likes-badge {
    display: inline-flex;
    align-items: center;
    gap: 0.15rem;
    color: var(--p-red-400);
}

.model-name {
    color: var(--p-primary-color);
    font-weight: 700;
    font-size: 0.65rem;
    text-transform: uppercase;
    letter-spacing: 0.02rem;
    opacity: 0.8;
}

.flag-icon-mini {
    width: 0.75rem;
    height: 0.55rem;
    object-fit: cover;
    border-radius: 1px;
    flex-shrink: 0;
}

.delete-btn {
    opacity: 0;
    transition: opacity 0.15s;
}

.prompt-item:hover .delete-btn {
    opacity: 1;
}

.empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 2rem;
    color: var(--p-surface-400);
    gap: 0.5rem;
}

.empty-state i {
    font-size: 1.5rem;
}
</style>
