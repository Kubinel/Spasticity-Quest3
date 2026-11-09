using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class MuscleCatalog
{
    [Serializable]
    private sealed class Entry
    {
        public string title = "";
    }

    [Serializable]
    private sealed class Catalog
    {
        public Entry[] muscles = Array.Empty<Entry>();
    }

    private readonly Entry[] entries;

    // Introductory actions for the meshes exposed in Preview. See docs/asset-and-content-review.md.
    private static readonly Dictionary<int, string> Actions = new Dictionary<int, string>
    {
        { 1, "Moves the thumb away from the plane of the palm." },
        { 2, "Abducts the thumb at its basal joint." },
        { 3, "Draws the thumb toward the palm and helps produce a strong pinch." },
        { 5, "Bends the elbow and helps turn the palm upward." },
        { 6, "Bends the elbow and helps turn the palm upward." },
        { 8, "Bends the elbow, including when the palm faces down." },
        { 9, "Bends the elbow, especially with the forearm in a neutral position." },
        { 19, "Extends the wrist and draws it toward the thumb side." },
        { 20, "Extends the wrist and draws it toward the thumb side." },
        { 21, "Extends the wrist and draws it toward the little-finger side." },
        { 22, "Straightens the little finger." },
        { 28, "Bends the wrist and draws it toward the thumb side." },
        { 29, "Bends the wrist and draws it toward the little-finger side." },
        { 31, "Bends the fingertip joints of the four fingers." },
        { 32, "Bends the middle joints of the four fingers." },
        { 33, "Bends the thumb at its knuckle." },
        { 34, "Bends the joint at the tip of the thumb." },
        { 37, "Helps bend the index knuckle while straightening its two finger joints." },
        { 38, "Helps bend the middle-finger knuckle while straightening its two finger joints." },
        { 39, "Helps bend the ring-finger knuckle while straightening its two finger joints." },
        { 40, "Helps bend the little-finger knuckle while straightening its two finger joints." },
        { 42, "Rotates and draws the thumb across the palm for opposition." },
        { 48, "Draws the upper arm toward the trunk and rotates it inward." },
        { 50, "Turns the forearm so that the palm faces down." },
        { 51, "Turns the forearm palm-down and can assist elbow bending." },
        { 56, "Rotates the upper arm inward and helps stabilize the shoulder." },
        { 59, "Draws the upper arm toward the trunk and assists inward rotation and extension." },
        { 60, "The triceps brachii straightens the elbow." },
        { 61, "Contributes to straightening the elbow." },
        { 62, "Straightens the elbow and also crosses the shoulder joint." },
        { 63, "Connects the triceps to the olecranon of the ulna." }
    };

    public MuscleCatalog(TextAsset asset)
    {
        if (asset == null)
            throw new ArgumentNullException(nameof(asset));

        entries = JsonUtility.FromJson<Catalog>(asset.text)?.muscles;
        if (entries == null || entries.Length == 0)
            throw new InvalidOperationException("The muscle catalog has no entries.");
    }

    public string Name(int id) => entries[id].title;
    public string Description(int id) => Actions[id];
}
