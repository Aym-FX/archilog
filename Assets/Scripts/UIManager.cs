using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    
    public GameObject panelF1;
    public GameObject panelF3;
    public GameObject panelF10;

    public TextMeshProUGUI txtIntensity;
    public TextMeshProUGUI txtRadius;
    public TextMeshProUGUI txtPattern;

    public TextMeshProUGUI txtFPS;
    public TextMeshProUGUI txtMeshCount;
    public TextMeshProUGUI txtVerticesCount;
    public TextMeshProUGUI txtTrianglesCount;

    private ChunkManager chunkManager;
    private int pattern;
    private Shader originalShader;

    private void Start()
    {
        // Récupération de l'instance du gestionnaire de chunks
        chunkManager = ChunkManager.Instance;

        // Désactivation des panneaux de l'interface utilisateur
        panelF1.SetActive(false);
        panelF3.SetActive(false);
        panelF10.SetActive(false);

        // Récupération du shader original
        originalShader = chunkManager.chunks[0].GetComponent<Renderer>().material.shader;

        // Démarrage de la coroutine pour mettre à jour le FPS
        StartCoroutine(UpdateFPS());
    }

    public void Update()
    {
        // Détection du pattern actuel
        detectPattern();

        // Mise à jour des textes de l'interface utilisateur
        txtIntensity.text = " Intensité: " + chunkManager.intensity;
        txtRadius.text = " Rayon: " + chunkManager.radius;
        txtPattern.text = " Pattern : " + pattern;

        // Gestion des entrées utilisateur pour afficher ou cacher les panneaux
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (panelF1.activeSelf)
            {
                panelF1.SetActive(false);
            }
            else
            {
                DeactivateAllPanels();
                panelF1.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (panelF3.activeSelf)
            {
                panelF3.SetActive(false);
            }
            else
            {
                DeactivateAllPanels();
                panelF3.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (panelF10.activeSelf)
            {
                panelF10.SetActive(false);
            }
            else
            {
                DeactivateAllPanels();
                panelF10.SetActive(true);
            }
        }
    }

    // Méthode pour désactiver tous les panneaux
    private void DeactivateAllPanels()
    {
        panelF1.SetActive(false);
        panelF3.SetActive(false);
        panelF10.SetActive(false);
    }

    // Méthode pour détecter le pattern actuel
    private void detectPattern()
    {
        if(chunkManager.deformationCurve == chunkManager.Pattern1)
        {
            pattern = 1;
        }
        else if(chunkManager.deformationCurve == chunkManager.Pattern2)
        {
            pattern = 2;
        }
        else if(chunkManager.deformationCurve == chunkManager.Pattern3)
        {
            pattern = 3;
        }
    }

    // Méthodes pour obtenir le nombre de meshes, de vertices et de triangles
    private int GetMeshCount()
    {
        int count = 0;
        foreach (Chunk chunk in chunkManager.chunks)
        {
            count++;
        }
        return count;
    }

    private int GetTotalVertices()
    {
        int count = 0;
        foreach (Chunk chunk in chunkManager.chunks)
        {
            if (chunk.p_mesh != null)
            {
                count += chunk.p_mesh.vertexCount;
            }
        }
        return count;
    }

    private int GetTotalTriangles()
    {
        int count = 0;
        foreach (Chunk chunk in chunkManager.chunks)
        {
            if (chunk.p_mesh != null)
            {
                count += chunk.p_mesh.triangles.Length / 3;
            }
        }
        return count;
    }

    // Coroutine pour mettre à jour le FPS et les compteurs de meshes, vertices et triangles
    private IEnumerator UpdateFPS()
    {
        while (true)
        {
            txtFPS.text = " FPS: " + (1.0f / Time.deltaTime).ToString("0.");
            txtMeshCount.text = " Mesh Count: " + GetMeshCount();
            txtVerticesCount.text = " Total Vertices: " + GetTotalVertices();
            txtTrianglesCount.text = " Total Triangles: " + GetTotalTriangles();
            yield return new WaitForSeconds(1.0f);
        }
    }
}
