using UnityEngine;
/// <summary>
/// MouseDrag is on each of the units in the fitting digital twin, it allows for them to be moved and snapped in position
/// </summary>
public class MouseDrag : MonoBehaviour {

    // Use this for initialization
    float distance = 10.5f;
    GameObject electricalImplemented;
    private void OnEnable()
    {
        electricalImplemented = GameObject.Find("ElecImplemented");
    }
    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objDistance = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.position = objDistance;
    }

    private void OnMouseUp()
    {
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(Mathf.Round(currentPos.x), Mathf.Round(currentPos.y), Mathf.Round(currentPos.z));
        transform.parent = electricalImplemented.transform;
        transform.GetComponent<BoxCollider>().enabled = false;
        electricalImplemented.GetComponent<CabinetFitting>().CheckUnitPosition(transform.localPosition);

    }

}
