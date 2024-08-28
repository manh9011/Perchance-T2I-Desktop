# Perchance Text to Image Desktop Application
![.NET Desktop](https://github.com/manh9011/Perchance-T2I-Desktop/actions/workflows/dotnet-desktop.yml/badge.svg)

## Overview

The **Perchance Text to Image Desktop Application** is a powerful tool that allows users to generate images from textual descriptions using the Perchance API. This desktop version provides a user-friendly interface, making it easier for users to interact with the API, generate images, and save them directly to their local storage.

## Features

- **Text Input**: Users can input any descriptive text to generate an image.
- **API Integration**: Seamless integration with the Perchance API to convert text into images.
- **Image Preview**: View the generated image directly within the application.
- **Save & Export**: Save the generated images to your local storage.
- **Customizable Settings**: Adjust parameters like style, image resolution, guidance scale.
- **User-Friendly Interface**: Easy-to-navigate GUI designed for both beginners and advanced users.

## Installation

### Prerequisites

- **Operating System**: Windows 10 or higher.
- **WebView2 Runtime**: Ensure that the Microsoft Edge WebView2 Runtime is installed. You can download it from the [Microsoft website](https://developer.microsoft.com/en-us/microsoft-edge/webview2/).

### Download

1. Download the latest release from the [Releases]([https://github.com/manh9011/PerchanceT2I-Desktop/releases) page.
2. Extract the downloaded ZIP file to your desired location.

### Running the Application

1. Navigate to the extracted folder.
2. Double-click the `Perchance.exe`.

## Usage

1. **Enter Text**: Type in a description of the image you want to generate.
2. **Configure Settings**: (Optional) Adjust the settings to customize the output.
3. **Generate Image**: Click the "Generate" button to create the image.
4. **Preview & Save**: Preview the image and save it to your local storage.
5. **View History**: Access the `History` panel to view a list of previously generated images along with their corresponding text inputs. You can re-generate or save these images again directly from the history.

## Self build from code

This guide provides instructions on how to build the `Perchance.csproj` project using the .NET command line interface (CLI).

### Prerequisites

- .NET SDK 8.0.* must be installed on your system. You can check if it's installed and its version by running:

    ```bash
    dotnet --version
    ```

- If the .NET SDK is not installed, download and install it from the official .NET website: [dotnet.microsoft.com](https://dotnet.microsoft.com/).

### Build Instructions

1. **Navigate to the project directory:**

    Open your terminal or command prompt and change the current directory to the location of your `Perchance.csproj` file. For example:

    ```bash
    cd path/to/your/project
    ```

2. **Restore dependencies:**

    Before building the project, you need to restore the project's dependencies. Run the following command:

    ```bash
    dotnet restore
    ```

    This command will download all necessary dependencies specified in the project file (`Perchance.csproj`).

3. **Publish the project:**

    To publish the project for deployment, use:

    ```bash
    dotnet publish /p:PublishProfile=Properties\PublishProfiles\FolderProfile.pubxml
    ```

    This command will publish the project to the `.\bin\Release\net8.0-windows\publish\win-x86` directory by profile configuration.

## Screenshot
![main ui](https://github.com/manh9011/PerchanceT2I-Desktop/blob/master/assets/main_ui.PNG?raw=true)
