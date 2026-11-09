using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spasticity.Quiz;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class QuestScene : MonoBehaviour
{
    [SerializeField] private AnatomyModel model;
    [SerializeField] private TextAsset muscleCatalog;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Sprite[] scenarioImages;

    private QuestRig rig;
    private WorldPanel panel;
    private MuscleCatalog catalog;
    private Muscle selection;
    private TextMeshPro label;
    private LineRenderer leader;
    private TMP_Text details;
    private bool isPreview;
    private readonly List<RaycastResult> mouseHits = new List<RaycastResult>();

    private IEnumerator Start()
    {
        var rigObject = new GameObject("Quest XR Origin");
        rigObject.transform.SetParent(transform, false);
        rigObject.SetActive(false);
        rig = rigObject.AddComponent<QuestRig>();
        rig.Initialize(lineMaterial);
        rigObject.SetActive(true);

        // The content stays world-locked once positioned from the first tracked view.
#if UNITY_ANDROID && !UNITY_EDITOR
        float deadline = Time.realtimeSinceStartup + 3f;
        while (!rig.IsHeadTracked && Time.realtimeSinceStartup < deadline)
            yield return null;
        if (!rig.IsHeadTracked)
            Debug.LogWarning("Head tracking was unavailable at scene entry. Placing the scene at the origin.");
#endif
        yield return null;
        string sceneName = gameObject.scene.name;
        isPreview = sceneName == "Preview";
        bool isMenu = sceneName == "Menu";

        Vector3 forward = Vector3.ProjectOnPlane(rig.View.transform.forward, Vector3.up);
        Quaternion facing = forward.sqrMagnitude > 0.001f ? Quaternion.LookRotation(forward) : Quaternion.identity;
        Vector3 head = rig.View.transform.position;
        panel = new WorldPanel(rig.View, isMenu ? 680 : 850, isMenu ? 405 : 690);
        panel.Root.SetParent(transform, false);
        panel.Root.SetPositionAndRotation(head + facing * (isMenu
            ? new Vector3(0f, -0.02f, 1.35f)
            : new Vector3(0.48f, -0.06f, 1.5f)), facing);

        if (isMenu)
        {
            BuildMenu();
            yield break;
        }

        catalog = new MuscleCatalog(muscleCatalog);
        model.Initialize(isPreview, SelectMuscle);
        model.Place(head + facing * new Vector3(-0.33f, -0.12f, 1.27f), facing);
        rig.Model = model;
        if (isPreview)
            BuildPreview();
        else
            BuildQuiz();
    }

    private void BuildMenu()
    {
        panel.Heading("Spasticity and Upper Limb Anatomy");
        panel.Text("Explore the anatomical layers or test your knowledge of ten upper-limb patterns.",
            23, 28, 80, 620, 65);
        panel.Button("Explore anatomy", GameManager.StartPreview, 28, 166, 624, 58);
        panel.Button("Start quiz", GameManager.StartQuiz, 28, 237, 624, 58);
        panel.Button("Quit", GameManager.QuitApp, 28, 316, 148, 48);
    }

    private void BuildPreview()
    {
        panel.Heading("Explore anatomy");
        BuildNavigation("Quiz", GameManager.StartQuiz);
        panel.Text("Point and pull the trigger to select. Grip the model to move or rotate it. Use the thumbstick while gripping to scale.",
            21, 28, 127, 794, 66);

        RectTransform muscles = panel.Group("Muscle controls");
        RectTransform layers = panel.Group("Layer controls");
        layers.gameObject.SetActive(false);
        panel.Button("Muscle study", () =>
        {
            model.SetLayerMode(false);
            layers.gameObject.SetActive(false);
            muscles.gameObject.SetActive(true);
        }, 28, 207, 380, 48);
        panel.Button("Anatomical layers", () =>
        {
            ClearMuscle();
            model.SetLayerMode(true);
            muscles.gameObject.SetActive(false);
            layers.gameObject.SetActive(true);
        }, 442, 207, 380, 48);

        details = panel.Text("Select a muscle on the model or choose its name below.", 23, 28, 267, 794, 100, muscles);
        details.alignment = TextAlignmentOptions.TopLeft;
        panel.Button("Clear", ClearMuscle, 28, 614, 180, 46, muscles);
        BuildMusclePicker(muscles);
        BuildLayerControls(layers);
    }

    private void BuildMusclePicker(RectTransform parent)
    {
        const int pageSize = 8;
        int[] ids = model.MuscleIds.ToArray();
        int page = 0;
        var buttons = new Button[pageSize];
        for (int i = 0; i < buttons.Length; i++)
        {
            int slot = i;
            buttons[i] = panel.Button("", () => SelectMuscle(model.GetMuscle(ids[page * pageSize + slot])),
                28 + i % 2 * 406, 373 + i / 2 * 55, 388, 47, parent, 21);
        }

        TMP_Text counter = panel.Text("", 22, 430, 614, 201, 46, parent);
        Button previous = panel.Button("Previous", null, 240, 614, 180, 46, parent);
        Button next = panel.Button("Next", null, 642, 614, 180, 46, parent);
        previous.onClick.AddListener(() => { page--; Refresh(); });
        next.onClick.AddListener(() => { page++; Refresh(); });
        counter.alignment = TextAlignmentOptions.Center;

        void Refresh()
        {
            int pages = (ids.Length + pageSize - 1) / pageSize;
            previous.interactable = page > 0;
            next.interactable = page + 1 < pages;
            counter.SetText($"{page + 1} / {pages}");
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = page * pageSize + i;
                buttons[i].gameObject.SetActive(index < ids.Length);
                if (index < ids.Length)
                    WorldPanel.SetText(buttons[i], catalog.Name(ids[index]));
            }
        }
        Refresh();
    }

    private void BuildLayerControls(RectTransform parent)
    {
        for (int i = 0; i < AnatomyModel.LayerNames.Count; i++)
        {
            int index = i;
            Button button = null;
            button = panel.Button("", () =>
            {
                model.SetLayer(index, !model.IsLayerVisible(index));
                UpdateLayer(button, index);
            }, 28 + i % 2 * 406, 276 + i / 2 * 58, 388, 49, parent);
            UpdateLayer(button, index);
        }

        TMP_Text opacity = panel.Text("", 23, 28, 526, 794, 43, parent);
        void ChangeOpacity(float change)
        {
            model.SetSkinOpacity(model.SkinOpacity + change);
            opacity.SetText($"Skin opacity  {Mathf.RoundToInt(model.SkinOpacity * 100f)}%");
        }
        ChangeOpacity(0f);
        panel.Button("Less opaque", () => ChangeOpacity(-0.1f), 28, 579, 388, 55, parent);
        panel.Button("More opaque", () => ChangeOpacity(0.1f), 434, 579, 388, 55, parent);
    }

    private void UpdateLayer(Button button, int index)
    {
        bool visible = model.IsLayerVisible(index);
        WorldPanel.SetText(button, $"{AnatomyModel.LayerNames[index]}  [{(visible ? "On" : "Off")}]");
        WorldPanel.Tint(button, visible ? AnatomyModel.SelectedColor : (Color?)null);
    }

    private void SelectMuscle(Muscle muscle)
    {
        if (model.LayersVisible || muscle == selection)
            return;

        ClearMuscle();
        selection = muscle;
        selection.SetHighlight(AnatomyModel.SelectedColor);
        string title = catalog.Name(muscle.Id);
        details.SetText(title + "\n" + catalog.Description(muscle.Id));
        if (label == null)
        {
            label = new GameObject("Selected muscle label").AddComponent<TextMeshPro>();
            label.transform.SetParent(transform, false);
            label.fontSize = 0.22f;
            label.alignment = TextAlignmentOptions.Center;
            label.rectTransform.sizeDelta = new Vector2(0.35f, 0.06f);
            label.enableWordWrapping = false;
            label.color = Color.white;

            leader = new GameObject("Muscle leader line").AddComponent<LineRenderer>();
            leader.transform.SetParent(transform, false);
            leader.sharedMaterial = lineMaterial;
            leader.useWorldSpace = true;
            leader.positionCount = 2;
            leader.startWidth = leader.endWidth = 0.0025f;
            leader.startColor = leader.endColor = AnatomyModel.SelectedColor;
        }
        label.SetText(title);
        label.gameObject.SetActive(true);
        leader.gameObject.SetActive(true);
    }

    private void ClearMuscle()
    {
        if (selection != null)
            selection.SetHighlight(null);
        selection = null;

        if (details != null)
            details.SetText("Select a muscle on the model or choose its name below.");
        if (label != null)
        {
            label.gameObject.SetActive(false);
            leader.gameObject.SetActive(false);
        }
    }

    private void BuildQuiz()
    {
        var session = new QuizSession(new System.Random());
        panel.Heading("Spasticity quiz");
        BuildNavigation("Explore", GameManager.StartPreview);
        Image image = panel.AddImage("Scenario illustration", 28, 133, 168, 128);
        image.preserveAspect = true;
        image.raycastTarget = false;
        TMP_Text pattern = panel.Text("", 22, 215, 131, 602, 131);
        pattern.alignment = TextAlignmentOptions.TopLeft;
        TMP_Text status = panel.Text("", 20, 28, 264, 794, 41);
        var buttons = new Button[QuizSession.Choices.Count];
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            QuizChoice choice = QuizSession.Choices[i];
            buttons[i] = panel.Button(choice.Label ?? catalog.Name(choice.MuscleIds[0]), () =>
            {
                session.Toggle(index);
                RefreshAnswers();
            }, 28 + i % 2 * 405, 311 + i / 2 * 28, 390, 26, fontSize: 20);
        }

        Button submit = panel.Button("Check answers", null, 298, 630, 254, 44);
        Button clear = panel.Button("Clear", null, 28, 630, 254, 44);
        Button next = panel.Button("Next scenario", null, 568, 630, 254, 44);
        submit.onClick.AddListener(() =>
        {
            switch (session.Submit())
            {
                case QuizResult.Empty:
                    status.SetText("Select at least one answer before checking.");
                    return;
                case QuizResult.Correct:
                    status.SetText("Correct. Green marks the expected answers.");
                    break;
                case QuizResult.Incorrect:
                    status.SetText("Incorrect. Green marks expected answers. Red marks extra choices.");
                    break;
                default:
                    return;
            }

            RefreshAnswers();
            UpdateButtons();
        });

        clear.onClick.AddListener(() =>
        {
            session.Clear();
            status.SetText(Prompt());
            RefreshAnswers();
        });

        next.onClick.AddListener(() =>
        {
            session.Next();
            RefreshScenario();
        });

        string Prompt() => $"Select {session.ExpectedChoiceCount} answer choices.";

        void UpdateButtons()
        {
            submit.interactable = !session.Submitted;
            clear.interactable = !session.Submitted;
            next.interactable = session.Submitted;
        }

        void RefreshAnswers()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                QuizChoice choice = QuizSession.Choices[i];
                bool selected = session.IsSelected(i);
                Color? tint = selected ? AnatomyModel.SelectedColor : (Color?)null;
                if (session.Submitted)
                {
                    tint = session.IsCorrectChoice(choice) ? AnatomyModel.CorrectColor :
                        selected ? AnatomyModel.IncorrectColor : (Color?)null;
                }

                buttons[i].interactable = !session.Submitted;
                WorldPanel.Tint(buttons[i], tint);
                foreach (int id in choice.MuscleIds)
                    model.GetMuscle(id).SetHighlight(tint);
            }
        }

        void RefreshScenario()
        {
            pattern.SetText(session.Scenario.Pattern);
            image.sprite = scenarioImages[session.ScenarioIndex];
            status.SetText(Prompt());
            RefreshAnswers();
            UpdateButtons();
        }

        RefreshScenario();
    }

    private void BuildNavigation(string otherScene, Action navigate)
    {
        panel.Button("Menu", GameManager.ReturnToMenu, 28, 77, 250, 44);
        panel.Button(otherScene, navigate, 300, 77, 250, 44);
        panel.Button("Reset model", rig.ResetModel, 572, 77, 250, 44);
    }

    private void LateUpdate()
    {
        if (selection != null)
        {
            Vector3 position = selection.LabelPoint;
            Vector3 facing = position - rig.View.transform.position;
            if (facing.sqrMagnitude > 0.0001f)
                label.transform.SetPositionAndRotation(position, Quaternion.LookRotation(facing));
            leader.SetPosition(0, position);
            leader.SetPosition(1, selection.LeaderPoint);
        }

#if UNITY_EDITOR
        if (!isPreview || panel == null || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mouse = Mouse.current.position.ReadValue();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = mouse }, mouseHits);
        if (mouseHits.Count == 0 && Physics.Raycast(rig.View.ScreenPointToRay(mouse), out RaycastHit hit, 4f,
                1 << AnatomyModel.MuscleRayLayer) && hit.collider.TryGetComponent(out Muscle muscle))
            SelectMuscle(muscle);
#endif
    }
}
