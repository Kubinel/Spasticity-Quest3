using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Spasticity.Quiz.Tests
{
    public sealed class QuizSessionTests
    {
        private QuizSession session;

        [SetUp]
        public void SetUp() => session = new QuizSession(new Random(12));

        [Test]
        public void EveryOriginalScenarioIsReachableAndAcceptsItsExactChoices()
        {
            int[][] originalKeys =
            {
                new[] { 48, 56, 59 },
                new[] { 5, 6, 8, 9 },
                new[] { 50, 51 },
                new[] { 28, 29 },
                new[] { 31, 32 },
                new[] { 3, 33, 34 },
                new[] { 3, 31, 32, 33, 34 },
                new[] { 5, 6, 8, 9, 48, 56, 59 },
                new[] { 5, 6, 8, 9, 28, 29, 48, 56, 59 },
                new[] { 5, 6, 8, 9, 31, 32, 48, 50, 51, 56, 59 }
            };
            var seen = new HashSet<int>();
            int previous = -1;
            for (int round = 0; round < originalKeys.Length; round++)
            {
                Assert.That(seen.Add(session.ScenarioIndex), Is.True);
                CollectionAssert.AreEqual(originalKeys[session.ScenarioIndex], session.Scenario.MuscleIds);
                int[] correct = CorrectIndices();
                Assert.That(session.ExpectedChoiceCount, Is.EqualTo(correct.Length));
                CollectionAssert.AreEquivalent(session.Scenario.MuscleIds,
                    correct.SelectMany(i => QuizSession.Choices[i].MuscleIds));
                foreach (int index in correct.Reverse())
                    session.Toggle(index);
                Assert.That(session.Submit(), Is.EqualTo(QuizResult.Correct));

                previous = session.ScenarioIndex;
                session.Next();
            }
            Assert.That(session.ScenarioIndex, Is.Not.EqualTo(previous));
        }

        [Test]
        public void EmptySubmissionCanBeRetried()
        {
            Assert.That(session.Submit(), Is.EqualTo(QuizResult.Empty));
            Assert.That(session.Submitted, Is.False);
            session.Toggle(CorrectIndices()[0]);
            Assert.That(session.Submit(), Is.EqualTo(QuizResult.Incorrect));
        }

        [Test]
        public void ExtraSelectionFailsEvenWhenEveryCorrectChoiceIsPresent()
        {
            int[] correct = CorrectIndices();
            foreach (int index in correct)
                session.Toggle(index);
            int wrong = Enumerable.Range(0, QuizSession.Choices.Count).First(i => !correct.Contains(i));
            session.Toggle(wrong);
            Assert.That(session.Submit(), Is.EqualTo(QuizResult.Incorrect));
        }

        [Test]
        public void SubmittedRoundStaysLockedUntilNextScenario()
        {
            session.Toggle(0);
            session.Submit();
            session.Toggle(0);
            session.Toggle(1);
            session.Clear();
            Assert.That(session.IsSelected(0), Is.True);
            Assert.That(session.IsSelected(1), Is.False);
            Assert.That(session.Submit(), Is.EqualTo(QuizResult.AlreadySubmitted));
            session.Next();
            Assert.That(session.IsSelected(0), Is.False);
            Assert.That(session.Submitted, Is.False);
        }

        [Test]
        public void ToggleClearAndInvalidIndicesLeaveNoSelection()
        {
            session.Toggle(0);
            session.Toggle(0);
            session.Toggle(1);
            session.Clear();
            session.Toggle(-1);
            session.Toggle(QuizSession.Choices.Count);
            Assert.That(session.Submit(), Is.EqualTo(QuizResult.Empty));
        }

        private int[] CorrectIndices() => Enumerable.Range(0, QuizSession.Choices.Count)
            .Where(i => session.IsCorrectChoice(QuizSession.Choices[i])).ToArray();
    }
}
