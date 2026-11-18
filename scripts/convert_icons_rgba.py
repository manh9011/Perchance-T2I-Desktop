"""Convert PNG icons in src-tauri/icons to RGBA mode for Tauri builds."""

from pathlib import Path
import sys
from PIL import Image


def convert_pngs_to_rgba(icons_dir: Path) -> int:
    """Convert any non-RGBA PNG files in icons_dir to RGBA in place.

    Returns the number of files converted.
    """
    if not icons_dir.is_dir():
        raise FileNotFoundError(f"Icons directory not found: {icons_dir}")

    converted = 0
    for png_path in sorted(icons_dir.glob("*.png")):
        with Image.open(png_path) as img:
            if img.mode == "RGBA":
                print(f"[OK] {png_path.name}: already RGBA ({img.size[0]}x{img.size[1]})")
                continue

            old_mode = img.mode
            rgba_img = img.convert("RGBA")

        rgba_img.save(png_path, format="PNG")
        converted += 1
        print(f"[CONVERTED] {png_path.name}: {old_mode} -> RGBA ({rgba_img.size[0]}x{rgba_img.size[1]})")

    return converted


def main() -> int:
    default_dir = Path(__file__).resolve().parent.parent / "src-tauri" / "icons"
    target_dir = Path(sys.argv[1]) if len(sys.argv) > 1 else default_dir
    converted = convert_pngs_to_rgba(target_dir)
    print(f"Done. Converted {converted} file(s).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
