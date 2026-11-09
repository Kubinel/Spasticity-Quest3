using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Spasticity.Quiz;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spasticity.Tests
{
    public sealed class SceneImportTests
    {
        [Test]
        public void BuildStartsAtMenuAndIncludesBothDestinations()
        {
            CollectionAssert.AreEqual(new[] { "Menu", "Preview", "Quiz" }, EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path)).ToArray());
        }

        [TestCase("Menu")]
        [TestCase("Preview")]
        [TestCase("Quiz")]
        public void SceneImportsWithItsContentBindings(string name)
        {
            string path = $"Assets/Scenes/{name}.unity";
            Scene scene = SceneManager.GetSceneByPath(path);
            bool openedForTest = !scene.isLoaded;
            if (openedForTest)
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            try
            {
                GameObject[] roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                    Assert.That(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject),
                        Is.Zero, transform.name);

                var controller = roots.SelectMany(root => root.GetComponentsInChildren<QuestScene>(true)).Single();
                var serialized = new SerializedObject(controller);
                Assert.That(serialized.FindProperty("lineMaterial").objectReferenceValue, Is.Not.Null);
                if (name == "Menu")
                    return;

                Assert.That(serialized.FindProperty("muscleCatalog").objectReferenceValue, Is.Not.Null);
                var model = (AnatomyModel)serialized.FindProperty("model").objectReferenceValue;
                Assert.That(model, Is.Not.Null);
                var modelFields = new SerializedObject(model);
                var focus = (GameObject)modelFields.FindProperty("focusModel").objectReferenceValue;
                Assert.That(focus, Is.Not.Null);
                Muscle[] muscles = focus.GetComponentsInChildren<Muscle>();
                Assert.That(muscles.Length, Is.EqualTo(31));
                Assert.That(muscles.All(muscle => muscle.Surface != null), Is.True);
                var ids = new HashSet<int>(muscles.Select(muscle => muscle.Id));
                Assert.That(ids.Count, Is.EqualTo(muscles.Length));

                if (name == "Preview")
                {
                    Assert.That(muscles.All(muscle => muscle.transform.childCount >= 2), Is.True);
                    Assert.That(modelFields.FindProperty("layerModel").objectReferenceValue, Is.Not.Null);
                    Assert.That(modelFields.FindProperty("skinMaterial").objectReferenceValue, Is.Not.Null);
                    var layers = modelFields.FindProperty("layers");
                    Assert.That(layers.arraySize, Is.EqualTo(7));
                    for (int i = 0; i < layers.arraySize; i++)
                        Assert.That(layers.GetArrayElementAtIndex(i).objectReferenceValue, Is.Not.Null);
                }
                else
                {
                    var illustrations = serialized.FindProperty("scenarioImages");
                    Assert.That(illustrations.arraySize, Is.EqualTo(QuizSession.Scenarios.Count));
                    for (int i = 0; i < illustrations.arraySize; i++)
                        Assert.That(illustrations.GetArrayElementAtIndex(i).objectReferenceValue, Is.Not.Null);
                    Assert.That(QuizSession.Choices.SelectMany(choice => choice.MuscleIds).All(ids.Contains), Is.True);
                }
            }
            finally
            {
                if (openedForTest)
                    EditorSceneManager.CloseScene(scene, true);
            }
        }
    }
}
