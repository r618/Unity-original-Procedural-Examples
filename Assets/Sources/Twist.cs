using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Twist : MonoBehaviour
{
    // This script is placed in public domain. The author takes no responsibility for any possible harm.
    // twist a mesh by this amount
    public float twist;
    public float inputSensitivity;
    private Vector3[] baseVertices;
    private Vector3[] baseNormals;
    public virtual void Update()
    {
        this.twist = this.twist + ((Input.GetAxis("Horizontal") * this.inputSensitivity) * Time.deltaTime);
        Mesh mesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh;
        if (this.baseVertices == null)
        {
            this.baseVertices = mesh.vertices;
        }
        if (this.baseNormals == null)
        {
            this.baseNormals = mesh.normals;
        }
        Vector3[] vertices = new Vector3[this.baseVertices.Length];
        Vector3[] normals = new Vector3[this.baseVertices.Length];
        int i = 0;
        while (i < vertices.Length)
        {
            vertices[i] = this.DoTwist(this.baseVertices[i], this.baseVertices[i].y * this.twist);
            normals[i] = this.DoTwist(this.baseNormals[i], this.baseVertices[i].y * this.twist);
            i++;
        }
        mesh.vertices = vertices;
        mesh.normals = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public virtual Vector3 DoTwist(Vector3 pos, float t)
    {
        float st = Mathf.Sin(t);
        float ct = Mathf.Cos(t);
        var new_pos = Vector3.zero;
        new_pos.x = (pos.x * ct) - (pos.z * st);
        new_pos.z = (pos.x * st) + (pos.z * ct);
        new_pos.y = pos.y;
        return new_pos;
    }

    public Twist()
    {
        this.twist = 1f;
        this.inputSensitivity = 1.5f;
    }

}