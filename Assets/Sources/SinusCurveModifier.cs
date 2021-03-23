using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SinusCurveModifier : MonoBehaviour
{
    // This script is placed in public domain. The author takes no responsibility for any possible harm.
    public float scale;
    public float speed;
    private Vector3[] baseHeight;
    public virtual void Update()
    {
        Mesh mesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh;
        if (this.baseHeight == null)
        {
            this.baseHeight = mesh.vertices;
        }
        Vector3[] vertices = new Vector3[this.baseHeight.Length];
        int i = 0;
        while (i < vertices.Length)
        {
            Vector3 vertex = this.baseHeight[i];
            vertex.y = vertex.y + (Mathf.Sin((((Time.time * this.speed) + this.baseHeight[i].x) + this.baseHeight[i].y) + this.baseHeight[i].z) * this.scale);
            vertices[i] = vertex;
            i++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    public SinusCurveModifier()
    {
        this.scale = 10f;
        this.speed = 1f;
    }

}