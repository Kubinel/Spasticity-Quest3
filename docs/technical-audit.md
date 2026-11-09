# Technical audit and migration decisions

Base revision `c61859f5ec6525c48d9d7450faf35949b82cbc89` · One commit was present before the port. The HoloLens source history and a working HoloLens build were unavailable in that revision.

## Original project

| Area | Findings |
| --- | --- |
| Scene flow | Only Preview was enabled in Build Settings. Calls to load Menu and Quiz could fail because those scenes were absent from the player. Menu and Quiz still serialized MRTK cameras, input components, and interaction events. MRTK was absent from the package manifest. |
| Preview | Two instances of the same arm FBX represented focused muscle study and layer exploration. Both had serialized model and presentation overrides. Preview added a Meta camera rig, controller and hand building blocks, passthrough, and grab components. Application UI still carried legacy input components and had no consistent ray-to-button path. The focus menu canvas started inactive and the alternate layer model carried its own controls. |
| Quiz | The scene had 22 answer buttons, a filtered arm model, ten sprite references, and question text. It still used the old camera, grab scripts, and canvas event components. Its catalog reference pointed to an absent GUID. Two components relied on unspecified `Start` ordering to register muscles, generate a scenario, and populate choices. |
| Sample scenes | `SampleScene` tested an OVR rig variant alongside the arm. `SampleSceneOld` contained a desktop camera and model. Neither was a playable entry point in the build. Both remain in Git history. |
| Vendor examples | TextMesh Pro includes 28 demonstration scenes and three demonstration prefabs outside the build. A GUID scan found unresolved references within the bundled sample materials and scenes. The port leaves vendor samples outside the application flow. |
| Prefabs | `Answer` and `Muscle text` referenced legacy input components. The former carried two input paths into the same toggle. The latter relied on the old head-camera reference and a separately instantiated line. `Choice (1)` was a title/background widget. `OVRCameraRig Variant` inherited the v72 Meta prefab and nested hand/controller assets. `PointForTitle` is still used as a label anchor and now contains just a transform. The original line prefab is retained in the project. |
| Anatomical data | One FBX with fourteen original PNG textures. Scene overrides identify 64 stable catalog IDs and 31 active study meshes. The layer view has a separate authored muscle subgroup and seven layer targets. Preview pointed to the existing 64-entry catalog. The original descriptions were development notes and were never presented as vetted anatomy. |
| Materials | The skin has a Built-in Standard material configured for transparency. The previous opacity script expected 0–255 despite Unity's normalized alpha, wrote the shared material and a renderer copy, and polled a slider every frame. Highlighting instanced renderer materials and reset colors to white. Four muscle and skeleton texture maps are 4096×4096. The Android memory and frame time cost needs device profiling. |
| Quiz behavior | The original grader sorted the user's selection in place and indexed it without a length check. A short answer could throw and a matching prefix with extra answers could pass. Position-dependent grouping was coupled to the number and sort order of active meshes. Independent feedback coroutines unlocked the UI while the correct answers were still being shown. |
| Model placement | Static position, scale, and rotation were written from per-object updates. The default quaternion was not normalized, and both model variants could overwrite shared state as their active state changed. Old highlight and label code assumed renderers, anchors, a valid catalog index, and a valid head reference. |
| Android | The project already set IL2CPP, ARM64, Linear color, and Vulkan. The Android loader list contained OpenXR, while the Unity Meta Quest feature was disabled and the Meta SDK's OpenXR features were enabled. Oculus loader/settings assets coexisted with that configuration. A custom manifest declared Quest devices, passthrough, scene access, anchors, and hand tracking. The SDK and manifest paths needed consolidation. |
| Packages | Unity 2022.3.7f1, Meta XR SDK All 72.0.0, and Unity OpenXR 1.8.2. Meta XR Core, Interaction SDK, MR Utility Kit, platform, voice, audio, and simulator were pulled into the lock. MRTK serialized components could not resolve from the dependency graph. TextMesh Pro examples and third-party notices were present. |

The model, layer overrides, original quiz assets and keys, and Preview anchor positions were recoverable from serialized files. The original build, headset tracking, UI, and grab behavior were unverified.

## Restored architecture

- The three active scenes retain their scene IDs. They reference the same original FBX and use the original focused-muscle overrides, layer model, and label anchors. Archived sample scenes and obsolete XR prefabs remain in Git history. The FBX, PNGs, and original catalog were left byte-for-byte intact.
- Unity OpenXR owns Android tracking and the Quest manifest integration. Android enables the Unity Meta Quest feature and Touch Plus plus Oculus Touch profiles. There is one XR provider and one rig per scene. XRI handles controller trigger input to uGUI and mesh interactables. A separate grip gesture moves, rotates, and scales the shared model pivot.
- World-space panels replace the old input-bound canvases. Anatomy study displays one active label to keep the arm readable and includes a name picker for occluded geometry. Preview can switch between the original study and layers variants without moving the arm's pivot. Color and opacity updates use material property blocks.
- Quiz scenarios use their original stable mesh IDs. Choice groups explicitly encode the old 22-button grouping. Exact-set grading and a submission lock replace timed feedback. The current question stays visible until the learner chooses Next. The shuffle visits every scenario before reshuffling.
- Android keeps IL2CPP, ARM64, Vulkan, and Linear color. The Quest support feature supplies launch metadata. The Meta-generated manifest and dormant Meta resources were removed from the active project.

## Version decision

| Option | Decision |
| --- | --- |
| Unity 2022.3.62f3 with OpenXR 1.14.3 and XRI 2.5.4 | Use for this recovery branch. Staying on the same editor line preserves importer and serialization behavior as much as possible. The patch is newer than Unity's 2022.3 security fix. Registry package manifests declare compatibility with this editor generation. |
| Unity 6.3 LTS | Use as the long-term migration target after recording a Quest baseline. Meta currently documents a Unity 6 path for its latest SDK. Upgrading the editor, XR package generations, and optional mixed-reality features together would add variables before the first headset test. |
| Retain Meta SDK All or restore MRTK | The source contains enough authored data to restore the features with a smaller controller stack. Neither dependency is required for the selected Quest architecture. |

| Locked package | Version |
| --- | --- |
| Unity OpenXR | 1.14.3 |
| XR Interaction Toolkit | 2.5.4 |
| XR Plug-in Management | 4.4.0 |
| XR Core Utilities | 2.3.0 |
| Input System | 1.7.0 |
| TextMesh Pro | 3.0.6 |
| Unity Test Framework | 1.1.33 |

Official references include the [Unity editor release](https://unity.com/releases/editor/whats-new/2022.3.62f3), [security advisory](https://unity.com/security/sept-2025-01), [Unity 6 support calendar](https://unity.com/releases/unity-6/support), [Meta provider guidance](https://developers.meta.com/vr/documentation/unity/unity-xr-plugin/), [Unity OpenXR Meta Quest feature](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.14/manual/features/metaquest.html), and [XRI UI setup](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.5/manual/ui-setup.html).
