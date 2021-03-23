using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class HeightmapGenerator : MonoBehaviour
{
    // This script is placed in public domain. The author takes no responsibility for any possible harm.
    public Texture2D heightMap;
    public Material material;
    public Vector3 size;
    public virtual void Start()
    {
        this.GenerateHeightmap();
    }

    public virtual void GenerateHeightmap()
    {
         // Create the game object containing the renderer
        this.gameObject.AddComponent(typeof(MeshFilter));
        this.gameObject.AddComponent<MeshRenderer>();
        if (this.material)
        {
            this.GetComponent<Renderer>().material = this.material;
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.white;
        }
        // Retrieve a mesh instance
        Mesh mesh = ((MeshFilter) this.GetComponent(typeof(MeshFilter))).mesh;
        int width = Mathf.Min(this.heightMap.width, 255);
        int height = Mathf.Min(this.heightMap.height, 255);
        int y = 0;
        int x = 0;
        // Build vertices and UVs
        Vector3[] vertices = new Vector3[height * width];
        Vector2[] uv = new Vector2[height * width];
        Vector4[] tangents = new Vector4[height * width];
        Vector2 uvScale = new Vector2(1f / (width - 1), 1f / (height - 1));
        Vector3 sizeScale = new Vector3(this.size.x / (width - 1), this.size.y, this.size.z / (height - 1));
        y = 0;
        while (y < height)
        {
            x = 0;
            while (x < width)
            {
                float pixelHeight = this.heightMap.GetPixel(x, y).grayscale;
                Vector3 vertex = new Vector3(x, pixelHeight, y);
                vertices[(y * width) + x] = Vector3.Scale(sizeScale, vertex);
                uv[(y * width) + x] = Vector2.Scale(new Vector2(x, y), uvScale);
                // Calculate tangent vector: a vector that goes from previous vertex
                // to next along X direction. We need tangents if we intend to
                // use bumpmap shaders on the mesh.
                Vector3 vertexL = new Vector3(x - 1, this.heightMap.GetPixel(x - 1, y).grayscale, y);
                Vector3 vertexR = new Vector3(x + 1, this.heightMap.GetPixel(x + 1, y).grayscale, y);
                Vector3 tan = Vector3.Scale(sizeScale, vertexR - vertexL).normalized;
                tangents[(y * width) + x] = new Vector4(tan.x, tan.y, tan.z, -1f);
                x++;
            }
            y++;
        }
        // Assign them to the mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        // Build triangle indices: 3 indices into vertex array for each triangle
        int[] triangles = new int[((height - 1) * (width - 1)) * 6];
        int index = 0;
        y = 0;
        while (y < (height - 1))
        {
            x = 0;
            while (x < (width - 1))
            {
                 // For each grid cell output two triangles
                triangles[index++] = (y * width) + x;
                triangles[index++] = ((y + 1) * width) + x;
                triangles[index++] = ((y * width) + x) + 1;
                triangles[index++] = ((y + 1) * width) + x;
                triangles[index++] = (((y + 1) * width) + x) + 1;
                triangles[index++] = ((y * width) + x) + 1;
                x++;
            }
            y++;
        }
        // And assign them to the mesh
        mesh.triangles = triangles;
        // Auto-calculate vertex normals from the mesh
        mesh.RecalculateNormals();
        // Assign tangents after recalculating normals
        mesh.tangents = tangents;
    }

    public HeightmapGenerator()
    {
        this.size = new Vector3(200, 30, 200);
    }

}