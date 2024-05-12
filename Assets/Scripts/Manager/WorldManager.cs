using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material worldMaterial;
    private Container container;
    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
