using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DragTransform : MonoBehaviour
{
    public Color mouseOverColor;
    private Color originalColor;
    public virtual void Start()
    {
        this.originalColor = this.GetComponent<Renderer>().sharedMaterial.color;
    }

    public virtual void OnMouseEnter()
    {
        this.GetComponent<Renderer>().material.color = this.mouseOverColor;
    }

    public virtual void OnMouseExit()
    {
        this.GetComponent<Renderer>().material.color = this.originalColor;
    }

    public virtual IEnumerator OnMouseDown()
    {
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector3 offset = this.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
        while (Input.GetMouseButton(0))
        {
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
            this.transform.position = curPosition;
            yield return null;
        }
    }

    public DragTransform()
    {
        this.mouseOverColor = Color.blue;
    }

}