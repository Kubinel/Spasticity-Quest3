using UnityEngine;
using System.Collections.Generic;

public class FileRead: MonoBehaviour
{
    public TextAsset json;

    [System.Serializable]
    public class Muscles
    {
        public string title;
        public string text;
    }

    [System.Serializable]
    public class MuscleList
    {
        public List<Muscles> muscles;
    }

    public MuscleList ml = new MuscleList();

    public List<Muscles> func(TextAsset json)
    {

        ml = JsonUtility.FromJson<MuscleList>(json.text);

        List<Muscles> res = new List<Muscles>();

        return res;
    }

     private void Awake() 
    {
        func(json);    
    }

    public string GetTitle(int i)
    {
        return ml.muscles[i].title;
    }

    public string GetText(int i)
    {
        return ml.muscles[i].text;
    }
}
