using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StatusCheck is used in the testing workflow, its on each unit and allows for them to be clickable 
/// </summary>
public class StatusCheck : MonoBehaviour
{

    CabinetTesting cabinetTesting;

    // Use this for initialization
    void Start()
    {
        cabinetTesting = GetComponentInParent<CabinetTesting>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "StatusIndicator")
                {
                    Renderer rend = hit.transform.GetComponent<Renderer>();
                    Material mat = rend.material;
                    if (mat.color == Color.red)
                    {
                        mat.color = Color.green;
                        mat.SetColor("_EmissionColor", Color.green);
                        cabinetTesting.unitsCorrect++;
                    }
                }
            }
        }
    }
}
