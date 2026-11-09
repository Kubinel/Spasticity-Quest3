using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

internal sealed class QuestRig : MonoBehaviour
{
    private const float RayLength = 4f;

    private sealed class Hand
    {
        public XRRayInteractor Ray;
        public XRInteractorLineVisual Visual;
        public InputAction Tracked;
        public InputAction Grip;
        public InputAction Stick;
    }

    private readonly List<InputAction> ownedActions = new List<InputAction>();
    private readonly List<Hand> hands = new List<Hand>();
    private InputAction headTracked;
    private Hand grabbingHand;
    private Vector3 grabOffset;
    private Quaternion grabRotation;

    public Camera View { get; private set; }
    public bool IsHeadTracked => headTracked.IsPressed();
    public AnatomyModel Model { private get; set; }

    public void Initialize(Material lineMaterial)
    {
        var origin = gameObject.AddComponent<XROrigin>();
        var offset = new GameObject("Camera and controller offset");
        offset.transform.SetParent(transform, false);
        origin.CameraFloorOffsetObject = offset;
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
        origin.CameraYOffset = 0f;

        var cameraObject = new GameObject("XR Camera");
        cameraObject.transform.SetParent(offset.transform, false);
        cameraObject.tag = "MainCamera";
        View = cameraObject.AddComponent<Camera>();
        View.nearClipPlane = 0.05f;
        View.farClipPlane = 30f;
        View.clearFlags = CameraClearFlags.SolidColor;
        View.backgroundColor = new Color(0.037f, 0.062f, 0.105f);
        cameraObject.AddComponent<AudioListener>();
        origin.Camera = View;

        var head = cameraObject.AddComponent<TrackedPoseDriver>();
        head.positionInput = new InputActionProperty(Bind("<XRHMD>/centerEyePosition", "Vector3"));
        head.rotationInput = new InputActionProperty(Bind("<XRHMD>/centerEyeRotation", "Quaternion"));
        head.trackingStateInput = new InputActionProperty(Bind("<XRHMD>/trackingState", "Integer"));
        headTracked = Bind("<XRHMD>/isTracked", "Button");

        var interactionManager = gameObject.AddComponent<XRInteractionManager>();
        var eventSystem = new GameObject("XR Event System");
        eventSystem.transform.SetParent(transform, false);
        eventSystem.AddComponent<EventSystem>();
        var module = eventSystem.AddComponent<XRUIInputModule>();
        module.activeInputMode = XRUIInputModule.ActiveInputMode.InputSystemActions;
        module.enableMouseInput = Application.isEditor;

        CreateHand("LeftHand", offset.transform, interactionManager, lineMaterial);
        CreateHand("RightHand", offset.transform, interactionManager, lineMaterial);
    }

    private void CreateHand(string usage, Transform parent, XRInteractionManager manager, Material material)
    {
        var handObject = new GameObject(usage + " controller ray");
        handObject.transform.SetParent(parent, false);
        string path = "<XRController>{" + usage + "}/";

        var controller = handObject.AddComponent<ActionBasedController>();
        var tracking = Bind(path + "isTracked", "Button");
        controller.isTrackedAction = new InputActionProperty(tracking);
        controller.trackingStateAction = new InputActionProperty(Bind(path + "trackingState", "Integer"));
        controller.positionAction = new InputActionProperty(Bind(path + "pointerPosition", "Vector3"));
        controller.rotationAction = new InputActionProperty(Bind(path + "pointerRotation", "Quaternion"));
        controller.selectAction = new InputActionProperty(Bind(path + "triggerPressed", "Button"));
        controller.uiPressAction = new InputActionProperty(Bind(path + "triggerPressed", "Button"));

        var ray = handObject.AddComponent<XRRayInteractor>();
        ray.interactionManager = manager;
        ray.rayOriginTransform = handObject.transform;
        ray.maxRaycastDistance = RayLength;
        ray.raycastMask = 1 << AnatomyModel.MuscleRayLayer;
        ray.hitClosestOnly = true;
        ray.allowAnchorControl = false;
        ray.enableUIInteraction = true;
        ray.enabled = false;

        var line = handObject.AddComponent<LineRenderer>();
        line.sharedMaterial = material;
        line.enabled = false;
        var visual = handObject.AddComponent<XRInteractorLineVisual>();
        visual.lineWidth = 0.003f;
        visual.lineLength = RayLength;
        visual.stopLineAtFirstRaycastHit = true;
        visual.enabled = false;

        hands.Add(new Hand
        {
            Ray = ray,
            Visual = visual,
            Tracked = tracking,
            Grip = Bind(path + "gripPressed", "Button"),
            Stick = Bind(path + "thumbstick", "Vector2")
        });
    }

    private InputAction Bind(string binding, string layout)
    {
        var type = layout == "Button" ? InputActionType.Button : InputActionType.Value;
        var action = new InputAction(type: type, binding: binding, expectedControlType: layout);
        ownedActions.Add(action);
        return action;
    }

    private void OnEnable()
    {
        foreach (var action in ownedActions)
            action.Enable();
    }

    private void OnDisable()
    {
        grabbingHand = null;
        foreach (var action in ownedActions)
            action.Disable();
    }

    private void OnDestroy()
    {
        foreach (var action in ownedActions)
            action.Dispose();
    }

    private void LateUpdate()
    {
        foreach (var hand in hands)
        {
            bool tracked = hand.Tracked.IsPressed();
            if (hand.Ray.enabled != tracked)
            {
                hand.Ray.enabled = tracked;
                hand.Visual.enabled = tracked;
            }
        }

        if (Model == null)
            return;

        if (grabbingHand != null && (!grabbingHand.Ray.enabled || !grabbingHand.Grip.IsPressed()))
            grabbingHand = null;

        if (grabbingHand == null)
        {
            foreach (var hand in hands)
            {
                if (!hand.Ray.enabled || !hand.Grip.WasPressedThisFrame())
                    continue;

                var aim = hand.Ray.rayOriginTransform;
                Physics.SyncTransforms();
                if (hand.Ray.TryGetCurrentUIRaycastResult(out _) ||
                    !Model.CanGrab(new Ray(aim.position, aim.forward)))
                    continue;

                grabbingHand = hand;
                grabOffset = Quaternion.Inverse(aim.rotation) * (Model.Pivot.position - aim.position);
                grabRotation = Quaternion.Inverse(aim.rotation) * Model.Pivot.rotation;
                break;
            }
        }

        if (grabbingHand == null)
            return;

        Transform controller = grabbingHand.Ray.rayOriginTransform;
        Model.Pivot.SetPositionAndRotation(controller.position + controller.rotation * grabOffset,
            controller.rotation * grabRotation);

        float zoom = grabbingHand.Stick.ReadValue<Vector2>().y;
        if (Mathf.Abs(zoom) > 0.2f)
        {
            Model.Scale(Mathf.Exp(zoom * Time.unscaledDeltaTime));
            grabOffset = Quaternion.Inverse(controller.rotation) * (Model.Pivot.position - controller.position);
        }
    }

    public void ResetModel()
    {
        grabbingHand = null;
        Model.ResetPose();
    }
}
