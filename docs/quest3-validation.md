# Quest 3 validation record

## Recorded results

| Check | Result |
| --- | --- |
| Quiz behavior | Five regression tests passed in a standalone C# harness using the Unity editor’s bundled Mono and the pinned NUnit assembly. They cover all ten original keys, grouping, short and extra answers, submission locking, and resetting selections. |
| Unity C# API check | Application and test assemblies compiled with Roslyn against Unity 2022.3.62f3 managed assemblies and the pinned XR package sources. Android and editor code paths compiled. |
| Serialized content | Static checks cover scene roots, local references, scene and prefab GUIDs, catalog IDs, quiz illustrations, layer targets, and unchanged original assets. |
| Unity Edit Mode and scene import | Blocked at editor startup. The installed 2022.3.62f3 editor exited with code 199 because it could not connect to its Licensing Client IPC channel. The Licensing Client also reported no licenses. No Unity Test Runner XML or imported-scene result was produced. |
| Android APK | Pending an activated editor with Android Build Support, SDK, NDK, and OpenJDK. No APK was produced. |
| Physical Quest 3 | Pending. Headset tracking, controller input, rendering, and performance require a device run. |

## Local editor and build checklist

- [ ] Open in an activated Unity 2022.3.62f3 editor with Android modules. Confirm Unity Package Manager resolves the lock and the Console has no compile or missing-script errors.
- [ ] Run the full Edit Mode suite. The scene tests check the three build entries, original FBX muscle bindings and renderers, layer and skin material references, and ten quiz sprites.
- [ ] Open Menu in Play Mode. Navigate through both experiences using the mouse and back to Menu. Confirm both arm views render with the original textures. Inspect the Console for import, shader, and physics-cooking warnings.
- [ ] Switch to Android, check Android XR Project Validation, and build a development APK. Check the merged manifest for the Quest VR launch category, Quest 3 device target, and no leftover Meta hand, scene, or anchor permissions.
- [ ] Install on Quest 3 in developer mode. Record headset OS, editor version, branch commit, APK name, and the build report.

## Physical headset checklist

| Area | Test and expected result |
| --- | --- |
| Launch and tracking | Cold-launch from the headset library. The menu appears in stereo in front of the initial gaze, with stable position when looking or leaning. Repeat seated, standing, and after recentering. |
| Controllers and UI | Test both Touch Plus controllers independently. The ray follows the hand, disappears when tracking is lost, and returns on reacquisition. One trigger click activates each button once. Verify Menu → Preview → Quiz → Menu and repeat after suspend/resume. |
| Anatomy | Select muscles from shoulder, forearm, and hand by ray and by name. Highlight, label, and leader agree. Selecting a second muscle clears the first highlight. Verify label positioning while rotating the model and after switching layer modes. |
| Layers and opacity | Toggle each of the seven layers individually, then enable them together. Compare skin opacity at 0%, roughly 50%, and 100%. Check for missing materials, sorting problems, and geometry that disappears in one eye. |
| Manipulation | Aim at or reach toward the model and hold grip with either controller. Move and rotate it, scale in both directions while gripping, release, and reset. Verify UI rays still work and controller tracking loss releases the model. |
| Quiz | Confirm all ten illustrations appear over a full cycle. Submit empty, incomplete, exact, and overcomplete selections. Confirm expected answers are shown in green and extra choices in red, controls lock after submission, and Next clears all prior feedback. |
| Device performance | Enable all layers, rotate and scale the arm, switch scenes repeatedly, and watch frame timing and memory. Capture `adb logcat` for Unity, OpenXR, and Android errors. |

Keep the PR in draft until the import, APK, and physical headset checks have recorded results.
