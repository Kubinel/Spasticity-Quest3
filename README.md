# Spasticity and Upper Limb Anatomy

A Unity C# application for exploring upper-limb anatomy and studying ten example spasticity patterns. Jakub Gombala originally developed the application for Microsoft HoloLens 2. The original HoloLens build worked according to the author. The first GitHub revision captured a partial Quest migration. This branch restores the three-scene application with controller input through Unity OpenXR and XR Interaction Toolkit.

**Status** · The source port is ready for local validation. This branch has no verified APK or Quest 3 test result. See the [validation record](docs/quest3-validation.md) and [asset and content review](docs/asset-and-content-review.md).

## Features

| Area | What you can do |
| --- | --- |
| Main menu | Open anatomy study or the quiz. Return from either scene or switch directly between them. |
| Muscle study | Point at the arm and pull the trigger, or select a muscle by name. The app highlights the mesh and shows its name, a leader line, and a short description. |
| Anatomy layers | Toggle arteries, lymphatic structures, muscles, nerves, skeleton, skin, and veins. Adjust skin opacity from 0% to 100%. |
| Model manipulation | Aim at the model or bring a controller near it and hold grip. Move or turn the controller to reposition or rotate the model. While gripping, move the thumbstick up or down to scale. Reset restores the scene's initial placement. |
| Spasticity quiz | Review the original illustration and select from 22 choices. The app checks the complete answer set, shows the expected choices and extra selections, and advances only when requested. All ten scenarios appear once per shuffled cycle. |

## Run in Unity

1. Install [Unity 2022.3.62f3](https://unity.com/releases/editor/whats-new/2022.3.62f3) through Unity Hub. Include **Android Build Support**, **Android SDK & NDK Tools**, and **OpenJDK**.
2. Clone this repository and open its root in Hub. Allow Unity to resolve packages and import the FBX, textures, and TextMesh Pro assets.
3. Open `Assets/Scenes/Menu.unity`. Press **Play** and click the panels with a mouse to check scene navigation, muscle study, layers, and the quiz.
4. Open **Window → General → Test Runner → EditMode** and run all tests. Save any open scene edits before running the scene-binding tests.

## Build and install on Quest 3

1. In **File → Build Settings**, switch to **Android**. Confirm **Menu**, **Preview**, and **Quiz** are enabled in that order and **Build App Bundle** is off.
2. In Android **XR Plug-in Management**, confirm **Initialize XR on Startup** and **OpenXR**. Under **OpenXR**, confirm **Meta Quest Support** targeting **Quest 3**, the **Meta Quest Touch Plus Controller Profile**, and the **Oculus Touch Controller Profile**. Review Android **Project Validation** before building.
3. Confirm **IL2CPP**, **ARM64**, **Vulkan**, and **Input System Package (New)** in Player settings. The Android package identifier is `com.kubinel.spasticityanatomy`.
4. Build a development APK, for example `Builds/SpasticityAnatomy.apk`. With developer mode enabled and USB debugging authorized on the headset, select **Build And Run** or install with `adb install -r Builds/SpasticityAnatomy.apk`.
5. Launch the app on Quest 3 and follow the [hardware checklist](docs/quest3-validation.md). Record the headset OS version and Unity build log with your results.

## Architecture

| Area | Responsibility |
| --- | --- |
| `Assets/Scenes` | Scene lighting and authored bindings to the arm, anatomy layers, label anchors, catalog, and quiz illustrations. |
| `Assets/Scripts/XR` | Creates one XR Origin, tracked camera, two controller rays, and an XR UI event system per scene. Places the world-space panels when the scene opens. |
| `Assets/Scripts/Anatomy` | Owns model placement, layer state, skin opacity, muscle selection, and material highlights. The original serialized muscle IDs still index `Assets/Svaly.json`. |
| `Assets/Scripts/Quiz` | Engine-independent quiz data, grouped choices, shuffling, and exact answer checking. |
| `Assets/Tests/EditMode` | Quiz regression tests and scene import checks for active content bindings. |

The port uses the Built-in Render Pipeline to preserve the original materials. The old MRTK and Meta building-block scenes and prefabs are available in the base commit. See the [technical audit](docs/technical-audit.md) for the migration decisions and the original failures.

## Known limits

- The Quest target uses controllers. Hand tracking and mixed-reality passthrough are deferred.
- The original quiz keys and illustrations are retained. Two combined scenarios need a content review before classroom use.
- Asset redistribution rights are unconfirmed. Check the original model purchase terms and illustration provenance before sharing the source, an APK, or captures. The [asset and content review](docs/asset-and-content-review.md) records the candidate listings and open questions.

## Portfolio capture plan

After the device checklist and rights review, capture three in-headset screenshots showing a selected muscle and its label, the arm with multiple layers and translucent skin, and a quiz result with correct and extra selections. A 45–60 second demo can show the route from the menu through a muscle selection, a grip and scale gesture, a skin opacity change, and one submitted quiz scenario. Include the tested Unity and headset OS versions with the video.
