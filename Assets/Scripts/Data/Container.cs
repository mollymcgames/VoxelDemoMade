using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Container : MonoBehaviour
{
    public Vector3 containerPosition;

    private Dictionary<Vector3, Voxel> data;
    private MeshData meshData = new MeshData();

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    //Assign the material to the mesh renderer and set the position of the container
    public void Initialize(Material mat, Vector3 position)
    {
        ConfigureComponents();
        data = new Dictionary<Vector3, Voxel>();
        meshRenderer.sharedMaterial = mat;
        containerPosition = position;
    }

    public void ClearData()
    {
        data.Clear();
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

        Vector3 blockPos;
        Voxel block; //single block to draw the cube

        int counter = 0;
        Vector3[] faceVertices = new Vector3[4];
        Vector2[] faceUVs = new Vector2[4];

        VoxelColor voxelColor;
        Color voxelColorAlpha;
        Vector2 voxelSmoothness;

        foreach (KeyValuePair<Vector3, Voxel> kvp in data)
        {
            if (kvp.Value.ID == 0)
                continue;

            blockPos = kvp.Key;
            block = kvp.Value;

            voxelColor = WorldManager.Instance.WorldColors[block.ID - 1]; //0 is air so index this way so not to get out of bounds error
            voxelColorAlpha = voxelColor.color; //color
            voxelColorAlpha.a = 1; //alpha 
            voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);

            //Itereate over each face of the cube
            for (int i = 0 ; i < 6; i++)
            {
                if (this[blockPos + voxelFaceChecks[i]].isSolid)
                    continue;
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

                    meshData.colors.Add(voxelColorAlpha);
                    meshData.UVs2.Add(voxelSmoothness);
                    meshData.triangles.Add(counter++);
                }
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

    public Voxel this[Vector3 index]
    {
        get
        {
            if (data.ContainsKey(index))
                return data[index];
            else
                return emptyVoxel;
        }

        set
        {
            if (data.ContainsKey(index))
                data[index] = value;
            else
                data.Add(index, value);
        }
    }

    public static Voxel emptyVoxel = new Voxel() { ID = 0 };

    #region Mesh Data

    //Create a mesh with the given vertices, triangles, and normals
    public struct MeshData
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> UVs;
        public List<Vector2> UVs2; // Use UVs2 as there will be the option to add textures
        public List<Color> colors;

        public bool Initialized;

        public void ClearData()
        {
            if (!Initialized)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                UVs = new List<Vector2>();
                UVs2 = new List<Vector2>();
                colors = new List<Color>();

                Initialized = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                UVs.Clear();
                UVs2.Clear();
                colors.Clear();
                mesh.Clear();
            }
        }

        public void UploadMesh(bool sharedVertices = false)
        {
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetColors(colors);
            
            mesh.SetUVs(0, UVs);
            mesh.SetUVs(2, UVs2); //2nd channel
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

    static readonly Vector3[] voxelFaceChecks = new Vector3[6]
    {
        new Vector3(0, 0, -1), //Back face
        new Vector3(0, 0, 1), //Front face
        new Vector3(-1, 0, 0), //Left face
        new Vector3(1, 0, 0), //Right face
        new Vector3(0, -1, 0), //Bottom face
        new Vector3(0, 1, 0) //Top face
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
