using UnityEngine;
using System.Collections;

public class KeycapSizer : MonoBehaviour {

    public void Resize(float w, float h)
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int i = 0;
            while (i < vertices.Length)
            {
                vertices[i] += normals[i] * Mathf.Sin(Time.time);
                if (vertices[i].x > 0.0f)
                {
                    vertices[i].x += (w - 1) / 2;
                }
                if (vertices[i].x < 0.0f)
                {
                    vertices[i].x -= (w - 1) / 2;
                }
                if (vertices[i].z > 0.0f)
                {
                    vertices[i].z += (h - 1) / 2;
                }
                if (vertices[i].z < 0.0f)
                {
                    vertices[i].z -= (h - 1) / 2;
                }
                i++;
            }
            mesh.vertices = vertices;
        }
    }
}
