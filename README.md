# Perchance T2I Desktop

[![Tauri Build](https://github.com/manh9011/Perchance-T2I-Desktop/actions/workflows/tauri-build.yml/badge.svg)](https://github.com/manh9011/Perchance-T2I-Desktop/actions)
![License](https://img.shields.io/github/license/manh9011/Perchance-T2I-Desktop)
![Rust](https://img.shields.io/badge/rust-v1.75+-orange.svg)
![Vue](https://img.shields.io/badge/vue-v3.5+-green.svg)

A powerful, cross-platform desktop application for generating high-quality images from text descriptions using the Perchance API. Built with **Tauri**, **Rust**, and **Vue 3**, it provides a lightning-fast and premium user experience.

![Main UI](https://github.com/manh9011/Perchance-T2I-Desktop/blob/master/assets/main_ui.PNG?raw=true)

## ✨ Features

- 🎨 **Powerful Generation**: Full integration with Perchance AI for diverse image generation.
- 🚀 **Hardware Upscaling**: Intelligent image upscaling using **WebSR** for crisp, high-resolution results.
- 🎭 **Style Management**: 
    - Extensive **Style Picker** with categorized prompt templates.
    - **Style Manager** to create, edit, and organize your own custom styles.
- 📜 **Advanced History**: 
    - Automatically saves every generation.
    - Sorting by Date, Seed, or Likes.
    - Favorite system to keep track of your best results.
- 🔍 **Interactive Preview**: High-performance image viewer with smooth zoom and pan.
- 🌓 **Modern Aesthetics**: Sleek dark mode interface built with **PrimeVue** and custom CSS.
- 🌍 **Internationalization**: Support for multiple languages with prompt translation indicators.

## 🛠️ Tech Stack

- **Framework**: [Tauri](https://tauri.app/) (Desktop Bridge)
- **Frontend**: [Vue.js 3](https://vuejs.org/) (Composition API)
- **State Management**: [Pinia](https://pinia.vuejs.org/)
- **UI Components**: [PrimeVue](https://primevue.org/)
- **Styling**: Vanilla CSS & PrimeVue Passthrough (PT)
- **Language**: Rust & TypeScript

## 🚀 Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (v18+)
- [Rust](https://www.rust-lang.org/tools/install) (stable)
- [WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (Windows only)

### Development

1. **Clone the repository**:
   ```bash
   git clone https://github.com/manh9011/Perchance-T2I-Desktop.git
   cd Perchance-T2I-Desktop
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Run in development mode**:
   ```bash
   npm run tauri dev
   ```

### Build

To create a production-ready installer:
```bash
npm run tauri build
```
The output will be located in `src-tauri/target/release/bundle`.

## 📸 Screenshots
![Main UI](https://github.com/manh9011/Perchance-T2I-Desktop/blob/master/assets/main_ui.PNG?raw=true)
![Style Manager UI](https://github.com/manh9011/Perchance-T2I-Desktop/blob/master/assets/stylemgr_ui.png?raw=true)
![Upscale UI](https://github.com/manh9011/Perchance-T2I-Desktop/blob/master/assets/upscale_ui.png?raw=true)
![Prompt UI](https://github.com/manh9011/Perchance-T2I-Desktop/blob/master/assets/prompt_ui.png?raw=true)

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## Credit
- Idea by [Perchance](https://perchance.org/ai-text-to-image-generator)
- Model Z-Image Turbo by [Pollination](https://pollinations.ai)
- Model SXDL & Flux Schnell by [Cloudflare AI](https://www.cloudflare.com)
- Static Web Hosting [Vercel](https://vercel.com)
- Api Gateway [Cloudflare](https://www.cloudflare.com)

Developed with ❤️ by [manh9011](https://github.com/manh9011).

