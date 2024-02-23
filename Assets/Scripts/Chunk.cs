using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    public Mesh p_mesh;
    ChunkManager chunkManager = ChunkManager.Instance;

    void Start()
    {
        // Initialisation du mesh
        p_mesh = new Mesh();
        p_mesh.Clear();

        // Calcul du nombre de vertices et de la taille des steps
        int numVertices = Mathf.RoundToInt(chunkManager.resolution) + 1;
        float stepSize = chunkManager.dimension / chunkManager.resolution;

        if (float.IsNaN(stepSize))
        {
            Debug.LogError("Erreur de calcul de stepSize");
            return;
        }

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float offsetX = -chunkManager.dimension / 2;
        float offsetZ = -chunkManager.dimension / 2;

        // Boucle pour créer les vertices et les triangles du mesh
        for (int i = 0; i < numVertices; i++)
        {
            for (int j = 0; j < numVertices; j++)
            {
                float x = i * stepSize + offsetX;
                float z = j * stepSize + offsetZ;
                float y = 0;

                vertices.Add(new Vector3(x, y, z));

                if (i < numVertices - 1 && j < numVertices - 1)
                {
                    int topLeft = i * numVertices + j;
                    int topRight = topLeft + 1;
                    int bottomLeft = (i + 1) * numVertices + j;
                    int bottomRight = bottomLeft + 1;

                    triangles.Add(topLeft);
                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);

                    triangles.Add(topRight);
                    triangles.Add(bottomRight);
                    triangles.Add(bottomLeft);
                }
            }
        }

        p_mesh.vertices = vertices.ToArray();
        p_mesh.triangles = triangles.ToArray();
        p_mesh.RecalculateNormals();
        p_mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = p_mesh;
        GetComponent<MeshCollider>().sharedMesh = p_mesh;
    }

    // Méthode pour déformer le terrain en un point donné
    public void DeformTerrain(Vector3 hitPoint, bool isUp, bool propagate = true)
    {
        Vector3[] vertices = p_mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(hitPoint, transform.TransformPoint(vertices[i]));
            if (distance < chunkManager.radius)
            {
                float deformation = chunkManager.deformationCurve.Evaluate(distance / chunkManager.radius) * chunkManager.intensity;
                if (isUp)
                    vertices[i] += deformation * Vector3.up;
                else
                    vertices[i] -= deformation * Vector3.up;
            }
        }
        p_mesh.vertices = vertices;
        p_mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = p_mesh;

        if (propagate)
        {
            Chunk[] neighbors = { chunkManager.GetVoisins(this, Vector3.left), chunkManager.GetVoisins(this, Vector3.right), chunkManager.GetVoisins(this, Vector3.forward), chunkManager.GetVoisins(this, Vector3.back) };
            foreach (Chunk neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    neighbor.DeformTerrain(hitPoint, true, false);
                }
            }
        }
    }

    // Méthode pour ajouter du terrain dans une direction donnée
    public void AddTerrain(Vector3 direction, GameObject chunkPrefab)
    {
        if (!Physics.Raycast(transform.position, direction, chunkManager.dimension))
        {
            GameObject newTerrain = Instantiate(chunkPrefab, transform.position + direction * chunkManager.dimension, Quaternion.identity);
            newTerrain.name = "Terrain " + direction.ToString();
            chunkManager.chunks.Add(newTerrain.GetComponent<Chunk>());
        }
    }

    // Méthode pour mettre en surbrillance les chunks
    public void ColorChunk()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material originalMaterial = renderer.material;

        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = new Color(Random.value, Random.value, Random.value);

        StartCoroutine(ResetMaterial(renderer, originalMaterial, 3f));
    }

    // Coroutine pour réinitialiser le matériau après un délai
    IEnumerator ResetMaterial(MeshRenderer renderer, Material originalMaterial, float delay)
    {
        yield return new WaitForSeconds(delay);
        renderer.material = originalMaterial;
    }

}
