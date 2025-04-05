# 🛠️ Unity-EasierAnimatorTransitionCopier
![Unity](https://img.shields.io/badge/Unity-2020%2B-blue.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

**Unity-EasierAnimatorTransitionCopier** is a Unity Editor tool that allows you to copy transitions from the Animator and paste them by selecting both the source and target states or sub-state machines — in a very simple and intuitive way.

---

## 📚 Table of Contents
- [🚀 Features](#-features)
- [🛠️ Installation](#️-installation)
- [📸 Screenshots](#-screenshots)
- [🎮 Usage](#-usage)
- [❓ FAQ](#-faq)
- [📝 Requirements](#-requirements)
- [✨ Credits](#-credits)
- [📜 License](#-license)

---

## 🚀 Features
- ✅ Copy and paste multiple transitions at once.
- ✅ Supports copying transitions from an animation state or sub-state machine to another state or sub-state machine.
- ✅ Compatible with Entry, Exit, and AnyState nodes.
- ✅ Intuitive and easy to use.

## 🛠️ Installation
### Option 1 – Zip Download
1. Download the `.zip` of this repository.
2. Extract it into your Unity project's `Assets` folder.

### Option 2 – Unity Package Manager (via Git URL)
1. Open `Window > Package Manager`.
2. Click the `+` icon > `Add package from Git URL...`.
3. Paste: https://github.com/TakNof/Unity-EasierAnimatorTransitionCopier.git

## 📸 Screenshots

## 📦 Import the package using Unity Package Manager
Easily install the tool by pasting the GitHub URL into Unity’s Package Manager.
![Importing](ShowcaseGifs/Import.gif)

### 🔍 Open the tool and load your Animator
Access the tool from the top menu and select the Animator you want to work with.
![Opening the tool](ShowcaseGifs/OpenAndLoad.gif)

### 🎯 Select transitions and paste them wherever you want!
Copy multiple transitions and paste them into any state or sub-state machine with just a few clicks.
![Enjoy!](ShowcaseGifs/Enjoy.gif)

## 🎮 Usage
1. Install the tool.
2. Go to the top menu bar and search for the option: **Taknof Tools**.
3. Open the **Unity-EasierAnimatorTransitionCopier** window.
4. Load the Animator you want to work with.
5. Select one or multiple transitions to copy.
6. Press the **Copy Selected Transitions** button.
7. Select the source animation state or sub-state machine.
8. While presing shift, select the target animation state or sub-state machine.
9. Press the **Paste Transitions** button.
10. Wait a few moments for the operation to complete (sometimes Unity requires interaction with the Animator window to display the pasted transitions visually).
11. Enjoy! :D

---

## ❓ FAQ

### 💬 Why are the transitions not showing up immediately after pasting?
It’s a Unity refresh behavior. Sometimes you need to click or interact with the Animator window for the pasted transitions to be visually updated.

### 🧩 Can I paste transitions from a sub-state machine into a regular animation state (and vice versa)?
Yes! You can mix and match sub-state machines and animation states freely.

### 🔄 Does this tool overwrite existing transitions?
No. It only adds new transitions. Existing ones will remain unchanged.

### 🛠️ Is this tool safe to use in production projects?
Yes! It only modifies the Animator Controller you’re working on and doesn’t touch your animations or prefabs.

---

## 📝 Requirements
- Unity 2020 or newer.

## ✨ Credits
Developed by **TakNof**.
If you find this tool useful, consider giving it a ⭐ on GitHub or buying me a coffee!
https://buymeacoffee.com/taknof


## 📜 License
This project is licensed under the [MIT License](LICENSE).
