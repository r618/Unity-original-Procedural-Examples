using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Generates an extrusion trail from the attached mesh
// Uses the MeshExtrusion algorithm in MeshExtrusion.cs to generate and preprocess the mesh.
[System.Serializable]
public class ExtrudedTrailSection : object
{
    public Vector3 point;
    public Matrix4x4 matrix;
    public float time;
}
[System.Serializable]
// Remove old sections
// Add a new trail section to beginning of array
// We need at least 2 sections to create the line
// all elements get the direction by looking up the next section
// When the angle of the rotation compared to the last segment is too high
// smooth the rotation a little bit. Optimally we would smooth the entire sections array.
// except the last one, which just copies the previous one
// Rebuild the extrusion mesh	
[UnityEngine.RequireComponent(typeof(MeshFilter))]
public partial class ExtrudedMeshTrail : MonoBehaviour
{
    public float time;
    public bool autoCalculateOrientation;
    public float minDistance;
    public bool invertFaces;
    private Mesh srcMesh;
    private MeshExtrusion.Edge[] precomputedEdges;
    public virtual void Start()
    {
        this.srcMesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).sharedMesh;
        this.precomputedEdges = MeshExtrusion.BuildManifoldEdges(this.srcMesh);
    }

    private List<ExtrudedTrailSection> sections = new List<ExtrudedTrailSection>();
    public virtual void LateUpdate()
    {
        Quaternion previousRotation = default(Quaternion);
        Vector3 position = this.transform.position;
        float now = Time.time;
        while ((this.sections.Count > 0) && (now > (((float) this.sections[this.sections.Count - 1].time) + this.time)))
        {
            this.sections.RemoveAt(this.sections.Count - 1);
        }
        if ((this.sections.Count == 0) || ((((Vector3) this.sections[0].point) - position).sqrMagnitude > (this.minDistance * this.minDistance)))
        {
            ExtrudedTrailSection section = new ExtrudedTrailSection();
            section.point = position;
            section.matrix = this.transform.localToWorldMatrix;
            section.time = now;
            this.sections.Insert(0, section);
        }
        if (this.sections.Count < 2)
        {
            return;
        }
        Matrix4x4 worldToLocal = this.transform.worldToLocalMatrix;
        Matrix4x4[] finalSections = new Matrix4x4[this.sections.Count];
        int i = 0;
        while (i < this.sections.Count)
        {
            if (this.autoCalculateOrientation)
            {
                if (i == 0)
                {
                    var direction = this.sections[0].point - this.sections[1].point;
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                    previousRotation = rotation;
                    finalSections[i] = worldToLocal * Matrix4x4.TRS(position, rotation, Vector3.one);
                }
                else
                {
                    if (i != (this.sections.Count - 1))
                    {
                        var direction = this.sections[i].point - this.sections[i + 1].point;
                        var rotation = Quaternion.LookRotation(direction, Vector3.up);
                        if (Quaternion.Angle(previousRotation, rotation) > 20)
                        {
                            rotation = Quaternion.Slerp(previousRotation, rotation, 0.5f);
                        }
                        previousRotation = rotation;
                        finalSections[i] = worldToLocal * Matrix4x4.TRS(this.sections[i].point, rotation, Vector3.one);
                    }
                    else
                    {
                        finalSections[i] = finalSections[i - 1];
                    }
                }
            }
            else
            {
                if (i == 0)
                {
                    finalSections[i] = Matrix4x4.identity;
                }
                else
                {
                    finalSections[i] = worldToLocal * this.sections[i].matrix;
                }
            }
            i++;
        }
        MeshExtrusion.ExtrudeMesh(this.srcMesh, ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh, finalSections, this.precomputedEdges, this.invertFaces);
    }

    public ExtrudedMeshTrail()
    {
        this.time = 2f;
        this.autoCalculateOrientation = true;
        this.minDistance = 0.1f;
        this.sections = new List<ExtrudedTrailSection>();
    }

}