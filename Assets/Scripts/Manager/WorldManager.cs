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
        container.GenerateMesh();
        container.UploadMesh();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
