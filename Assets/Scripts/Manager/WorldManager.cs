using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{

    public Material worldMaterial;
    private Container container;

    public VoxelColor[] WorldColors;

    void Start()
    {
        if (_instance != null)
        {
            if (_instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        GameObject cont = new GameObject("Container");
        cont.transform.parent = transform;
        container = cont.AddComponent<Container>();

        container.Initialize(worldMaterial, Vector3.zero);

        for (int x = 6; x < 9; x++)
        {
            for (int z = 6; z < 9; z++)
            {
                int randomYHeight = Random.Range(1, 5);
                for (int y = 0; y < randomYHeight; y++)
                {
                    container[new Vector3(x, y, z)] = new Voxel { ID = 1 };
                }
            }
        }
        container.GenerateMesh();
        container.UploadMesh();
        
    }

    //Make a singleton for the colors might be less efficient but it's easier to manage
    private static WorldManager _instance; //Set up a static reference to world manager
    public static WorldManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WorldManager>();
            }
            return _instance;
        }
    }
}
