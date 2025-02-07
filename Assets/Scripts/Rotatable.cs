using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : MonoBehaviour
{
    public GameObject centerEyeAnchor;

    void Update()
    {
        transform.LookAt(2* transform.position - centerEyeAnchor.transform.position);
    }
}
