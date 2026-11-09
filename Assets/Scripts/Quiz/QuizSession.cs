using System;
using System.Collections.Generic;
using System.Linq;

namespace Spasticity.Quiz
{
    public enum QuizResult
    {
        Empty,
        Correct,
        Incorrect,
        AlreadySubmitted
    }

    public sealed class QuizScenario
    {
        internal QuizScenario(string pattern, params int[] muscleIds)
        {
            Pattern = pattern;
            MuscleIds = Array.AsReadOnly(muscleIds);
        }

        public string Pattern { get; }
        public IReadOnlyList<int> MuscleIds { get; }
    }

    public sealed class QuizChoice
    {
        internal QuizChoice(string label, params int[] muscleIds)
        {
            Label = label;
            MuscleIds = Array.AsReadOnly(muscleIds);
        }

        public string Label { get; }
        public IReadOnlyList<int> MuscleIds { get; }
    }

    public sealed class QuizSession
    {
        public static IReadOnlyList<QuizScenario> Scenarios { get; } = Array.AsReadOnly(new[]
        {
            new QuizScenario("Abducted shoulder\nInternal rotation", 48, 56, 59),
            new QuizScenario("Flexed elbow", 5, 6, 8, 9),
            new QuizScenario("Pronated forearm", 50, 51),
            new QuizScenario("Flexed wrist", 28, 29),
            new QuizScenario("Flexed fingers", 31, 32),
            new QuizScenario("Thumb in palm", 3, 33, 34),
            new QuizScenario("Clenched fist", 3, 31, 32, 33, 34),
            new QuizScenario("Abducted shoulder\nFlexed elbow", 5, 6, 8, 9, 48, 56, 59),
            new QuizScenario("Abducted shoulder\nFlexed elbow\nSupinated forearm\nFlexed wrist",
                5, 6, 8, 9, 28, 29, 48, 56, 59),
            new QuizScenario("Abducted shoulder\nFlexed elbow\nPronated forearm\nFlexed wrist",
                5, 6, 8, 9, 31, 32, 48, 50, 51, 56, 59)
        });

        // The original 22 buttons combined mesh IDs at hard-coded indices in an alphabetical list.
        public static IReadOnlyList<QuizChoice> Choices { get; } = Array.AsReadOnly(new[]
        {
            new QuizChoice(null, 1),
            new QuizChoice(null, 2),
            new QuizChoice(null, 3),
            new QuizChoice("Biceps", 5, 6),
            new QuizChoice(null, 8),
            new QuizChoice(null, 9),
            new QuizChoice("Extensor carpi", 19, 20, 21),
            new QuizChoice(null, 22),
            new QuizChoice(null, 28),
            new QuizChoice(null, 29),
            new QuizChoice(null, 31),
            new QuizChoice(null, 32),
            new QuizChoice(null, 33),
            new QuizChoice(null, 34),
            new QuizChoice("Lumbricals", 37, 38, 39, 40),
            new QuizChoice(null, 42),
            new QuizChoice(null, 48),
            new QuizChoice(null, 50),
            new QuizChoice(null, 51),
            new QuizChoice(null, 56),
            new QuizChoice(null, 59),
            new QuizChoice("Triceps", 61, 62)
        });

        private readonly Random random;
        private readonly int[] order = Enumerable.Range(0, Scenarios.Count).ToArray();
        private readonly HashSet<int> selected = new HashSet<int>();
        private int orderIndex = -1;

        public QuizSession(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            Next();
        }

        public int ScenarioIndex => order[orderIndex];
        public QuizScenario Scenario => Scenarios[ScenarioIndex];
        public bool Submitted { get; private set; }
        public int ExpectedChoiceCount => Choices.Count(IsCorrectChoice);

        public bool IsSelected(int choiceIndex) => selected.Contains(choiceIndex);
        public bool IsCorrectChoice(QuizChoice choice) => choice.MuscleIds.All(Scenario.MuscleIds.Contains);

        public void Toggle(int choiceIndex)
        {
            if (Submitted || choiceIndex < 0 || choiceIndex >= Choices.Count)
                return;

            if (!selected.Add(choiceIndex))
                selected.Remove(choiceIndex);
        }

        public void Clear()
        {
            if (!Submitted)
                selected.Clear();
        }

        public QuizResult Submit()
        {
            if (Submitted)
                return QuizResult.AlreadySubmitted;
            if (selected.Count == 0)
                return QuizResult.Empty;

            Submitted = true;
            var selectedMuscles = new HashSet<int>(selected.SelectMany(index => Choices[index].MuscleIds));
            return selectedMuscles.SetEquals(Scenario.MuscleIds) ? QuizResult.Correct : QuizResult.Incorrect;
        }

        public void Next()
        {
            int previous = orderIndex < 0 ? -1 : ScenarioIndex;
            orderIndex++;
            if (orderIndex == order.Length || previous == -1)
            {
                for (int i = order.Length - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (order[i], order[j]) = (order[j], order[i]);
                }

                if (order[0] == previous)
                    (order[0], order[1]) = (order[1], order[0]);

                orderIndex = 0;
            }

            selected.Clear();
            Submitted = false;
        }
    }
}
