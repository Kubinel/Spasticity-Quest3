using UnityEngine;
using TMPro;
using System;

public class MuscleInfo : MonoBehaviour
{
    private string title;

    private float ofsetX;
    private float ofsetY;
    private float ofsetZ;
    private bool lightUp;

    public TMP_Text titleObject;
    public TMP_Text highlightTitleObject;

    public int poradie;

    public LineRenderer line;
    public LineRenderer lineRenderer;
    public Muscle muscle;
    public GameObject centerEyeAnchor;
    public FileRead fr;

    private Options options;

    void Start()
    {
        options = muscle.GetOptions();
        lineRenderer = Instantiate(line, Vector3.zero, Quaternion.identity);
        options.AddLine(lineRenderer.gameObject);
        options.AddMuscleInfo(this.gameObject);
        
        if (muscle != null)
        {
            title = muscle.GetName();
        }

        title = fr.GetTitle(poradie);

        titleObject.SetText(title);
        highlightTitleObject.SetText(title);

        transform.position = muscle.GetPostionForTitle();
    }

    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, muscle.GetPostionForLine());

        transform.position = muscle.GetPostionForTitle();
        
        transform.LookAt(2* transform.position - centerEyeAnchor.transform.position);
    }

    private void OnDisable() 
    {
        if (lineRenderer != null)
            lineRenderer.GetComponent<Renderer>().enabled = false;
    }

    private void OnEnable() 
    {   
        if (lineRenderer != null)
            lineRenderer.GetComponent<Renderer>().enabled = true;    
    }

    public void Highlight()
    {
        muscle.Highligt();
        lightUp = !lightUp;

        if (lightUp)
        {
            highlightTitleObject.gameObject.SetActive(false);
            titleObject.gameObject.SetActive(true);
        }
        else
        {
            highlightTitleObject.gameObject.SetActive(true);
            titleObject.gameObject.SetActive(false);
        }
    }

    public void KeepHighlight()
    {
        muscle.KeepHighlight();

        highlightTitleObject.gameObject.SetActive(true);
        titleObject.gameObject.SetActive(false);

    }

    public void CancelKeepHighlight()
    {
        if(!lightUp)
        {
            muscle.CancelKeepHighlight();
            
            highlightTitleObject.gameObject.SetActive(false);
            titleObject.gameObject.SetActive(true);
        } 
    }

    public void Unhighlight()
    {
        lightUp = false;
        highlightTitleObject.gameObject.SetActive(false);
        titleObject.gameObject.SetActive(true);
    }


    public void setCenterEyeAnchor(GameObject cea)
    {
        this.centerEyeAnchor = cea;
    }

    public void SetFR(FileRead fileR)
    {
        this.fr = fileR;
    }

    public void SetMuscle(Muscle m)
    {
        this.muscle = m;
    }

    public void SetPoradie(int p)
    {
        this.poradie = p;
    }

    public void SetOfsetX(float ox)
    {
        ofsetX = ox;
    }

    public void SetOfsetY(float oy)
    {
        ofsetY = oy;
    }

    public void SetOfsetZ(float oz)
    {
        ofsetZ = oz;
    }

    public string GetTitle()
    {
        return title;
    }

    internal void SetCenterEyeAnchor(GameObject cEA)
    {
        centerEyeAnchor = cEA;
    }

}
