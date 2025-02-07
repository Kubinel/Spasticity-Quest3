using UnityEngine;

public class Muscle : MonoBehaviour
{
    public Color highlightColor;
    public ObjectStorer objectStorer;
    public int poradie;

    public bool quizz;
    public bool muscleInfo;

    public Options options;

    private MuscleInfo muscleInfoObject;
    private GameObject centerEyeAnchor;
    private FileRead fr;

    private string muscleName;
    private Renderer myRenderer;
    private MuscleInfo mIObject = null;

    private bool highlight;

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetName()
    {
        return muscleName;
    }

    public Options GetOptions()
    {
        return options;
    }

    private void Awake() 
    {
        muscleName = this.name;       
    }

    private void Start() 
    {
        if(muscleInfo)
        {
            myRenderer = GetComponent<Renderer>();

            highlightColor = Color.blue;

            muscleInfoObject = objectStorer.GetMuscleInfo();
            centerEyeAnchor = objectStorer.GetCenterEyeAnchor();
            fr = objectStorer.GetFR();
        }

        if(!quizz)
        {
            mIObject = Instantiate(muscleInfoObject, Vector3.zero, Quaternion.identity);
            mIObject.name = muscleName;

            mIObject.SetCenterEyeAnchor(centerEyeAnchor);
            mIObject.SetFR(fr);
            mIObject.SetMuscle(this);
            mIObject.SetPoradie(poradie);
        }

        objectStorer.AddMuscle(this);
    }

    public void Highligt()
    {
        highlight = !highlight;
        if (highlight)
        {
            myRenderer.material.SetColor("_Color", highlightColor);
        }
        else
        {
            myRenderer.material.SetColor("_Color", Color.white);
        }
    }

    public void HighligtGreen()
    {
        myRenderer.material.SetColor("_Color", Color.green);
    }

    public void UnHighligtGreen()
    {
        myRenderer.material.SetColor("_Color", Color.white);
    }

    public void Unhighlight()
    {
        highlight = false;
        myRenderer.material.SetColor("_Color", Color.white);
        if (mIObject != null)
            mIObject.Unhighlight();
    }

    public void KeepHighlight()
    {
        myRenderer.material.SetColor("_Color", highlightColor);
    }

    public void CancelKeepHighlight()
    {
        if(!highlight)
            myRenderer.material.SetColor("_Color", Color.white);
    }

    public Vector3 GetPostionForLine()
    {
        return transform.GetChild(0).gameObject.transform.position;
    }

    public Vector3 GetPostionForTitle()
    {
        return transform.GetChild(1).gameObject.transform.position;
    }

    public int GetPoradie()
    {
        return poradie;
    }
}
