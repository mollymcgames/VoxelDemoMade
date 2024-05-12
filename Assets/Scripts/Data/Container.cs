using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Container : MonoBehaviour
{
    public Vector3 containerPosition;
    private MeshData meshData = new MeshData();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    //Assign the material to the mesh renderer and set the position of the container
    public void Initialize(Material mat, Vector3 position)
    {
        ConfigureComponents();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;

    }

    //Configure the components of the container
    private void ConfigureComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    //Create a function to do something
    public void GenerateMesh()
    {
        meshData.ClearData();

        Vector3 blockPos = new Vector3(8, 8, 8); //single block position to draw the cube
        Voxel block = new Voxel() { ID = 1 }; //Create a new Voxel with an ID of 1

        int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        //Itereate over each face of the cube
        for (int i = 0 ; i < 6; i++)
        {
            //Draw the face if the block is solid

            //Collect the appropriate vertices from the default verticies and add the block positon 
            for (int j = 0; j < 4; j++)
            {
                faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + blockPos;
                faceUVs[j] = voxelUVs[j];
            }

            for (int j = 0; j < 6; j++)
            {
                meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                meshData.UVs.Add(faceUVs[voxelTris[i, j]]);
                meshData.triangles.Add(counter++);
            }
        }

    }

    public void UploadMesh()
    {
        meshData.UploadMesh();
        if (meshRenderer == null)
            ConfigureComponents();

        meshFilter.mesh = meshData.mesh;
        if (meshData.vertices.Count > 3)
            meshCollider.sharedMesh = meshData.mesh;
    
    }

    #region Mesh Data

    //Create a mesh with the given vertices, triangles, and normals
    public struct MeshData
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> UVs;

        public bool Initialized;

        public void ClearData()
        {
            if (!Initialized)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                UVs = new List<Vector2>();

                Initialized = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                UVs.Clear();
                mesh.Clear();
            }
        }

        public void UploadMesh(bool sharedVertices = false)
        {
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetUVs(0, UVs);
            mesh.Optimize();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.UploadMeshData(false);
        }

    }

    #endregion

    //Region that makes up the mesh data
    #region Voxel Statics
    static readonly Vector3[] voxelVertices = new Vector3[8]
    {
        new Vector3(0, 0, 0), //0 - bottom left of cube
        new Vector3(1, 0, 0), //1 - bottom right of cube
        new Vector3(0, 1, 0), //2 - top right of cube
        new Vector3(1, 1, 0), //3 - top left of cube

        new Vector3(0, 0, 1), //4 - bottom left of cube on front face
        new Vector3(1, 0, 1), //5 - bottom right of cube on front face
        new Vector3(0, 1, 1), //6 - top right of cube on front face
        new Vector3(1, 1, 1) //7   - top left of cube on front face
    };

    static readonly int[,] voxelVertexIndex = new int[6, 4]
    {
        {0, 1, 2, 3}, //Back face
        {4, 5, 6, 7}, //Front face
        {4, 0, 6, 2}, //Left face
        {5, 1, 7, 3}, //Right face
        {0, 1, 4, 5}, //Bottom face
        {2, 3, 6, 7} //Top face
    };

    static readonly Vector2[] voxelUVs = new Vector2[4]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1)
    };

    static readonly int[,] voxelTris = new int[6, 6]
    {
        {0, 2, 3, 0, 3, 1}, 
        {0, 1, 2, 1, 3, 2},
        {0, 2, 3, 0, 3, 1},
        {0, 1, 2, 1, 3, 2},
        {0, 1, 2, 1, 3, 2},
        {0, 2, 3, 0, 3, 1}
    };

    #endregion
}
