using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float startDelay = 2;
    private float spawnInterval = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomAnimal", startDelay, spawnInterval);
    }

    public GameObject[] animalPrefabs;
    public int animalIndex;
    private float spawnRangeX = 10;
    private float spawnPosZ = 20;


    // Update is called once per frame
    void Update()
    {

    }

    void SpawnRandomAnimal()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0, spawnPosZ);
        animalIndex = Random.Range(0, animalPrefabs.Length);
        Instantiate(animalPrefabs[animalIndex], spawnPos, animalPrefabs[animalIndex].transform.rotation);
    }
}
