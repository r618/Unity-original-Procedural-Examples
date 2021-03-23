using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class CrumpleMesh : MonoBehaviour
{
    // This script is placed in public domain. The author takes no responsibility for any possible harm.
    public float scale;
    public float speed;
    public bool recalculateNormals;
    private Vector3[] baseVertices;
    private Perlin noise;
    public virtual void Start()
    {
        this.noise = new Perlin();
    }

    public virtual void Update()
    {
        Mesh mesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh;
        if (this.baseVertices == null)
        {
            this.baseVertices = mesh.vertices;
        }
        Vector3[] vertices = new Vector3[this.baseVertices.Length];
        float timex = (Time.time * this.speed) + 0.1365143f;
        float timey = (Time.time * this.speed) + 1.21688f;
        float timez = (Time.time * this.speed) + 2.5564f;
        int i = 0;
        while (i < vertices.Length)
        {
            Vector3 vertex = this.baseVertices[i];
            vertex.x = vertex.x + (this.noise.Noise(timex + vertex.x, timex + vertex.y, timex + vertex.z) * this.scale);
            vertex.y = vertex.y + (this.noise.Noise(timey + vertex.x, timey + vertex.y, timey + vertex.z) * this.scale);
            vertex.z = vertex.z + (this.noise.Noise(timez + vertex.x, timez + vertex.y, timez + vertex.z) * this.scale);
            vertices[i] = vertex;
            i++;
        }
        mesh.vertices = vertices;
        if (this.recalculateNormals)
        {
            mesh.RecalculateNormals();
        }
        mesh.RecalculateBounds();
    }

    public CrumpleMesh()
    {
        this.scale = 1f;
        this.speed = 1f;
    }

}