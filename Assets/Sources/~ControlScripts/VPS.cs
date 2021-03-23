using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class VPS : MonoBehaviour
{
    private Mesh mesh;
    public bool showFPS;
    public virtual void Start()
    {
        this.GetComponent<GUIText>().material.color = Color.black;
    }

    public virtual void LateUpdate()
    {
        if (!this.mesh)
        {
            this.mesh = (Mesh) UnityEngine.Object.FindObjectOfType(typeof(Mesh));
        }
        if ((Time.frameCount % 5) != 0)
        {
            int vps = (int) ((this.mesh.vertexCount / Time.smoothDeltaTime) / 1000);
            this.GetComponent<GUIText>().text = ("Vertices per second:\n" + vps) + "k";
            int fps = (int) (1f / Time.smoothDeltaTime);
            if (this.showFPS)
            {
                this.GetComponent<GUIText>().text = this.GetComponent<GUIText>().text + ("\nFrames per second:\n" + fps);
            }
        }
    }

}