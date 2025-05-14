using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLooper : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionsPrefabs;

    [SerializeField]
    int poolSize = 20;

    [SerializeField]
    int visibleSectionsCount = 10;

    GameObject[] sectionsPool;
    Queue<GameObject> activeSections = new Queue<GameObject>();

    Transform playerCarTransform;

    const float sectionLength = 78f;
    float nextSpawnX;

    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize pool
        sectionsPool = new GameObject[poolSize];
        int prefabIndex = 0;
        for (int i = 0; i < poolSize; i++)
        {
            sectionsPool[i] = Instantiate(sectionsPrefabs[prefabIndex], Vector3.zero, Quaternion.identity);
            sectionsPool[i].SetActive(false);

            prefabIndex = (prefabIndex + 1) % sectionsPrefabs.Length;
        }

        // Spawn initial road segments
        nextSpawnX = playerCarTransform.position.x;
        for (int i = 0; i < visibleSectionsCount; i++)
        {
            SpawnNextSection();
        }
    }

    void Update()
    {
        // If the player has moved past the first road section, recycle it
        if (activeSections.Count > 0)
        {
            GameObject firstSection = activeSections.Peek();
            if (playerCarTransform.position.x - firstSection.transform.position.x > sectionLength * 1.5f)
            {
                // Recycle
                activeSections.Dequeue().SetActive(false);
                SpawnNextSection();
            }
        }
    }

    void SpawnNextSection()
    {
        GameObject section = GetRandomInactiveFromPool();
		float roadY = 19f;
        section.transform.position = new Vector3(nextSpawnX, roadY, playerCarTransform.position.z + 7);
        section.SetActive(true);
        activeSections.Enqueue(section);

        nextSpawnX += sectionLength;
    }

    GameObject GetRandomInactiveFromPool()
    {
        for (int i = 0; i < sectionsPool.Length; i++)
        {
            if (!sectionsPool[i].activeInHierarchy)
            {
                return sectionsPool[i];
            }
        }

        Debug.LogWarning("No available inactive sections in pool!");
        // Just return something to avoid crash
        return sectionsPool[0];
    }
}
