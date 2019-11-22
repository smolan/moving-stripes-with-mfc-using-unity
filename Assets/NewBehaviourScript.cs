using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float Radius = 10f;    //半径  
    public float Height = 2;           ///3.1415926f;
    private MeshFilter meshFilter;


    void Start()
    {

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = Createmeshp(Radius, Height);

    }

  
    Mesh Createmeshp(float radius, float Height)
    {
        int p_vertices_count = 4;
        Vector3[] p_vertices = new Vector3[p_vertices_count];
        p_vertices[0] = new Vector3(-radius, -Height / 2, radius);
        p_vertices[1] = new Vector3(-radius, Height / 2, radius);
        p_vertices[2] = new Vector3(radius, -Height / 2, radius);
        p_vertices[3] = new Vector3(radius, Height / 2, radius);

        //triangles

        int[] p_triangles = new int[6];

        p_triangles[0] = 0;
        p_triangles[1] = 1;
        p_triangles[2] = 2;

        p_triangles[3] = 3;
        p_triangles[4] = 2;
        p_triangles[5] = 1;

        //uv:
        Vector2[] p_uvs = new Vector2[4];
        float p_uvSetup = 1.0f ;

 
        p_uvs[0] = new Vector2(p_uvSetup * 0, 1);
        p_uvs[1] = new Vector2(p_uvSetup * 0, 0);
        p_uvs[2] = new Vector2(p_uvSetup * 1, 1);
        p_uvs[3] = new Vector2(p_uvSetup * 1, 0);


        //负载属性与mesh
        Mesh mesh = new Mesh();
        mesh.vertices = p_vertices;
        mesh.triangles = p_triangles;
        mesh.uv = p_uvs;
        return mesh;
    }

    // Update is called once per frame
    void Update()
    {
  
    }
}