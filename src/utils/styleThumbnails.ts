const builtInThumbModules = import.meta.glob('../assets/style-thumbs/*.{png,jpg,jpeg,webp}', {
    eager: true,
    import: 'default',
}) as Record<string, string>;

const builtInThumbBySlug: Record<string, string> = {};

for (const [modulePath, assetUrl] of Object.entries(builtInThumbModules)) {
    const fileName = modulePath.split('/').pop() || '';
    const slug = fileName.replace(/\.[^.]+$/, '').toLowerCase();
    if (slug) {
        builtInThumbBySlug[slug] = assetUrl;
    }
}

export function getStyleThumbSlug(styleName: string): string {
    return styleName.replace(/\s+/g, '-').toLowerCase();
}

export function getBuiltInStyleThumbnail(styleName: string): string | undefined {
    return builtInThumbBySlug[getStyleThumbSlug(styleName)];
}
