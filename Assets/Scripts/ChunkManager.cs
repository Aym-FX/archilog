using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    // Variables pour les paramètres du terrain
    public float dimension;
    public float resolution;
    public float radius = 1f;
    public float intensity = 1f;

    // Variables pour les courbes de déformation
    public AnimationCurve deformationCurve;
    public AnimationCurve Pattern1;
    public AnimationCurve Pattern2;
    public AnimationCurve Pattern3;

    public GameObject chunkPrefab;

    public List<Chunk> chunks = new List<Chunk>();

    public Material[] materialsTerrain;
    private int numMatTerrain;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Méthode pour initialiser les chunks de terrain   
    private void Start()
    {
        deformationCurve = Pattern1;
        Instantiate(chunkPrefab);
        chunks.AddRange(FindObjectsOfType<Chunk>());
    }

    // Méthode pour gérer les entrées utilisateur et mettre à jour les chunks
    void Update()
    {
        // Inputs pour déformer le terrain
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                foreach (Chunk chunk in chunks)
                {
                    if (hit.collider == chunk.GetComponent<MeshCollider>())
                    {
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            chunk.DeformTerrain(hit.point, false);
                        }
                        else
                        {
                            chunk.DeformTerrain(hit.point, true);
                        }
                    }
                }
            }
        }
        // Inputs pour changer l'intensité de déformation
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(intensity < 20)
                intensity += 1f;
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            if(intensity > 0)
                intensity -= 1f;
        }
        // Inputs pour changer le rayon de déformation
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if(radius < 20)
                radius += 1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if(radius > 0)
                radius -= 1f;
        }
        // Inputs des flèches directionnelles pour ajouter des chunks
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.forward, chunkPrefab);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.back, chunkPrefab);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.left, chunkPrefab);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.right, chunkPrefab);
            }
        }
        // inputs pour changer la courbe de déformation
        if (Input.GetKeyDown(KeyCode.P)) {
            if (deformationCurve == Pattern1)
                deformationCurve = Pattern2;
            else if (deformationCurve == Pattern2)
                deformationCurve = Pattern3;
            else
                deformationCurve = Pattern1;
        }
        // inputs pour mettre en surbrillance les chunks
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.ColorChunk();
            }
            
        }
        // inputs pour changer le matériau des chunks
        if (Input.GetKeyDown(KeyCode.F12))
        {
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            numMatTerrain++;
            foreach (Chunk chunk in tempChunks)
            {
                chunk.GetComponent<MeshRenderer>().material = materialsTerrain[(numMatTerrain) % materialsTerrain.Length];
            }
        }
    }
    
    // Méthode pour obtenir le chunk voisin dans une direction donnée
    public Chunk GetVoisins(Chunk chunk, Vector3 direction)
    {
        foreach (Chunk c in chunks)
        {
            if (c != chunk)
            {
                Vector3 diff = c.transform.position - chunk.transform.position;
                if (Mathf.Approximately(diff.magnitude, dimension) && Mathf.Approximately(Vector3.Dot(diff.normalized, direction.normalized), 1))
                {
                    return c;
                }
            }
        }
        return null;
    }

}
