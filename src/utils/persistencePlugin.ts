import { PiniaPluginContext } from 'pinia';

/**
 * Custom Pinia plugin to persist store state to localStorage.
 * Replaces pinia-plugin-persistedstate.
 */
export function localStoragePlugin(context: PiniaPluginContext) {
    const { store } = context;

    // List of store IDs that should be persisted
    const persistedStores = ['styles', 'generator', 'models'];

    if (!persistedStores.includes(store.$id)) {
        return;
    }

    // Load state from localStorage on initialization
    const savedState = localStorage.getItem(store.$id);
    if (savedState) {
        try {
            store.$patch(JSON.parse(savedState));
        } catch (error) {
            console.error(`Failed to load state for store ${store.$id}:`, error);
        }
    }

    // Subscribe to store changes and save to localStorage
    store.$subscribe((_mutation, state) => {
        try {
            // specifically for generator store, we might want to ensure we don't save excessive data
            // but the previous plugin saved everything, so we'll stick to that for now to maintain behavior
            localStorage.setItem(store.$id, JSON.stringify(state));
        } catch (error) {
            console.error(`Failed to save state for store ${store.$id}:`, error);
        }
    });
}
