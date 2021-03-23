using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 Generates a trail that is always facing upwards using the scriptable mesh interface.
 vertex colors and uv's are generated similar to the builtin Trail Renderer.
 To use it
 1. create an empty game object
 2. attach this script and a MeshRenderer
 3. Then assign a particle material to the mesh renderer
*/
[System.Serializable]
public class TronTrailSection : object
{
    public Vector3 point;
    public Vector3 upDir;
    public float time;
}
[System.Serializable]
// Remove old sections
// Add a new trail section
// Rebuild the mesh
// We need at least 2 sections to create the line
// Use matrix instead of transform.TransformPoint for performance reasons
// Generate vertex, uv and colors
// Calculate u for texture uv and color interpolation
// Calculate upwards direction
// Generate vertices
// fade colors out over time
// Generate triangles indices
// Assign to mesh	
[UnityEngine.RequireComponent(typeof(MeshFilter))]
public partial class TronTrail : MonoBehaviour
{
    public float height;
    public float time;
    public bool alwaysUp;
    public float minDistance;
    public Color startColor;
    public Color endColor;
    private List<TronTrailSection> sections = new List<TronTrailSection>();
    public virtual void LateUpdate()
    {
        Vector3 position = this.transform.position;
        float now = Time.time;
        while ((this.sections.Count > 0) && (now > (((float) this.sections[this.sections.Count - 1].time) + this.time)))
        {
            this.sections.RemoveAt(this.sections.Count - 1);
        }
        if ((this.sections.Count == 0) || ((((Vector3) this.sections[0].point) - position).sqrMagnitude > (this.minDistance * this.minDistance)))
        {
            TronTrailSection section = new TronTrailSection();
            section.point = position;
            if (this.alwaysUp)
            {
                section.upDir = Vector3.up;
            }
            else
            {
                section.upDir = this.transform.TransformDirection(Vector3.up);
            }
            section.time = now;
            this.sections.Insert(0, section);
        }
        Mesh mesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh;
        mesh.Clear();
        if (this.sections.Count < 2)
        {
            return;
        }
        Vector3[] vertices = new Vector3[this.sections.Count * 2];
        Color[] colors = new Color[this.sections.Count * 2];
        Vector2[] uv = new Vector2[this.sections.Count * 2];
        TronTrailSection previousSection = (TronTrailSection) this.sections[0];
        TronTrailSection currentSection = (TronTrailSection) this.sections[0];
        Matrix4x4 localSpaceTransform = this.transform.worldToLocalMatrix;
        int i = 0;
        while (i < this.sections.Count)
        {
            previousSection = currentSection;
            currentSection = (TronTrailSection) this.sections[i];
            float u = 0f;
            if (i != 0)
            {
                u = Mathf.Clamp01((Time.time - currentSection.time) / this.time);
            }
            Vector3 upDir = currentSection.upDir;
            vertices[(i * 2) + 0] = localSpaceTransform.MultiplyPoint(currentSection.point);
            vertices[(i * 2) + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + (upDir * this.height));
            uv[(i * 2) + 0] = new Vector2(u, 0);
            uv[(i * 2) + 1] = new Vector2(u, 1);
            Color interpolatedColor = Color.Lerp(this.startColor, this.endColor, u);
            colors[(i * 2) + 0] = interpolatedColor;
            colors[(i * 2) + 1] = interpolatedColor;
            i++;
        }
        int[] triangles = new int[((this.sections.Count - 1) * 2) * 3];
        i = 0;
        while (i < (triangles.Length / 6))
        {
            triangles[(i * 6) + 0] = i * 2;
            triangles[(i * 6) + 1] = (i * 2) + 1;
            triangles[(i * 6) + 2] = (i * 2) + 2;
            triangles[(i * 6) + 3] = (i * 2) + 2;
            triangles[(i * 6) + 4] = (i * 2) + 1;
            triangles[(i * 6) + 5] = (i * 2) + 3;
            i++;
        }
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public TronTrail()
    {
        this.height = 2f;
        this.time = 2f;
        this.minDistance = 0.1f;
        this.startColor = Color.white;
        this.endColor = new Color(1, 1, 1, 0);
        this.sections = new List<TronTrailSection>();
    }

}