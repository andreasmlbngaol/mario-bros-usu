using UnityEngine;

public class ProceduralSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // Drag a Cloud or Star prefab here
    public int objectCount = 10;
    public Vector2 minArea = new Vector2(-10, 0);
    public Vector2 maxArea = new Vector2(50, 8);

    void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            float randomX = Random.Range(minArea.x, maxArea.x);
            float randomY = Random.Range(minArea.y, maxArea.y);
            Vector3 spawnPos = new Vector3(randomX, randomY, 0);

            GameObject obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            
            // Random Scaling
            float randomScale = Random.Range(0.8f, 1.2f);
            obj.transform.localScale = new Vector3(randomScale, randomScale, 1);
        }
    }
}