using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLooper : MonoBehaviour
{
    [SerializeField]
	GameObject[] sectionsPrefabs;
	
	GameObject[] sectionsPool = new GameObject[20];
	
	GameObject[] sections = new GameObject[10];
	
	Transform playerCarTransform;
	
	WaitForSeconds waitFor100ms = new WaitForSeconds(0,1f);
	
	const float sectionLength = 26;

	void start()
	{
		playerCarTransform = GameObject.FindGameObjectwithTag("Player").transform; 
		
		int prefabIndex = 0;
		
		for (int i = 0; i < sectionsPool.Length; i++)
		{
			sectionsPool[i] = instantiate(sectionsPrefabs[prefabIndex]);
			sectionsPool[i].setActive(false);
			
			prefabIndex++;
			
			if(prefabIndex > sectionsPrefabs.Length - 1)
				prefabIndex = 0;
		}
		
		for(int i=0; i < sections.Length; i++)
		{
			GameObject radomsection = GetRandomsectionFromPool();
			
			randomSection.transform.position = new vector3(sectionPool[i].transform.position.x,0,i * sectionLength);
			randomSection.setActive(true);
			
			aectiona[i] = randomSection;
		}
		
		startCoroutine(UpdateLessOftenCO);
	}
	
	IEnumerator UpdateLessOftenCO()
	{
		while (true)
		{
			yield return waitFor100ms;
		}
	}
	
	void UpdateSectionPosition()
	{
		for(int i = 0; i < sections.Length; i++)
		{
			if (section[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
			{
				vector3 LastSectionPosition = sections[i].transform.position;
				sections[i].setActive(false);
				
				sections[i] = GetrandomsectionFromPool();
				
				sections[i].transform.position = new Vector3(lastSectionPosition.x, -100, lastSectionPosition.z + sectionLength = sections.Length);
				sections[i].SetActive(true);
			}
		}
	}
	
	GameObject GetRandomsectionFromPool()
	{
		int randomIndex = Random.range(0, sectionsPool.Length);
		
		bool isNewSectionFound = false;
		
		while(isNewsectionFound)
		{
			if (!sectionsPool[randomIndex].activeInHierachy)
				IsNewSectionFound = true;
			else
			{
				randomIndex++;
				
				if (randomzIndex > sectionsPool.length - 1)
					randomIndex = 0;
			}
		}
	}
	
}
