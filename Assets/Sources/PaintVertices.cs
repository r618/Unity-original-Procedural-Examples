using UnityEngine;
using System.Collections;

public enum FallOff
{
    Gauss = 0,
    Linear = 1,
    Needle = 2
}

[System.Serializable]
public partial class PaintVertices : MonoBehaviour
{
    public float radius;
    public float pull;
    private MeshFilter unappliedMesh;
    public FallOff fallOff;
    public static float LinearFalloff(float distance, float inRadius)
    {
        return Mathf.Clamp01(1f - (distance / inRadius));
    }

    public static float GaussFalloff(float distance, float inRadius)
    {
        return Mathf.Clamp01(Mathf.Pow(360f, -Mathf.Pow(distance / inRadius, 2.5f) - 0.01f));
    }

    public virtual float NeedleFalloff(float dist, float inRadius)
    {
        return (-(dist * dist) / (inRadius * inRadius)) + 1f;
    }

    public virtual void DeformMesh(Mesh mesh, Vector3 position, float power, float inRadius)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        float sqrRadius = inRadius * inRadius;
        // Calculate averaged normal of all surrounding vertices	
        Vector3 averageNormal = Vector3.zero;
        int i = 0;
        while (i < vertices.Length)
        {
            float sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            // Early out if too far away
            if (sqrMagnitude > sqrRadius)
            {
                goto Label_for_9;
            }
            float distance = Mathf.Sqrt(sqrMagnitude);
            float falloff = PaintVertices.LinearFalloff(distance, inRadius);
            averageNormal = averageNormal + (falloff * normals[i]);
            Label_for_9:
            i++;
        }
        averageNormal = averageNormal.normalized;
        // Deform vertices along averaged normal
        i = 0;
        while (i < vertices.Length)
        {
            var sqrMagnitude = (vertices[i] - position).sqrMagnitude;
            // Early out if too far away
            if (sqrMagnitude > sqrRadius)
            {
                goto Label_for_10;
            }
            var distance = Mathf.Sqrt(sqrMagnitude);
            float falloff = 0;
            switch (this.fallOff)
            {
                case FallOff.Gauss:
                    falloff = PaintVertices.GaussFalloff(distance, inRadius);
                    break;
                case FallOff.Needle:
                    falloff = this.NeedleFalloff(distance, inRadius);
                    break;
                default:
                    falloff = PaintVertices.LinearFalloff(distance, inRadius);
                    break;
            }
            vertices[i] = vertices[i] + ((averageNormal * falloff) * power);
            Label_for_10:
            i++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public virtual void Update()
    {
        RaycastHit hit = default(RaycastHit);
        // When no button is pressed we update the mesh collider
        if (!Input.GetMouseButton(0))
        {
             // Apply collision mesh when we let go of button
            this.ApplyMeshCollider();
            return;
        }
        // Did we hit the surface?
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            MeshFilter filter = (MeshFilter) hit.collider.GetComponent(typeof(MeshFilter));
            if (filter)
            {
                 // Don't update mesh collider every frame since physX
                 // does some heavy processing to optimize the collision mesh.
                 // So this is not fast enough for real time updating every frame
                if (filter != this.unappliedMesh)
                {
                    this.ApplyMeshCollider();
                    this.unappliedMesh = filter;
                }
                // Deform mesh
                Vector3 relativePoint = filter.transform.InverseTransformPoint(hit.point);
                this.DeformMesh(filter.mesh, relativePoint, this.pull * Time.deltaTime, this.radius);
            }
        }
    }

    public virtual void ApplyMeshCollider()
    {
        if (this.unappliedMesh && (MeshCollider) this.unappliedMesh.GetComponent(typeof(MeshCollider)))
        {
            ((MeshCollider) this.unappliedMesh.GetComponent(typeof(MeshCollider))).sharedMesh = this.unappliedMesh.mesh;
        }
        this.unappliedMesh = null;
    }

    public PaintVertices()
    {
        this.radius = 1f;
        this.pull = 10f;
        this.fallOff = FallOff.Gauss;
    }

}