using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLooper : MonoBehaviour
{
    [SerializeField]
	GameObject[] sectionsPrefabs;
	
	GameObject[] sectionsPool = new GameObject[20];
	
	GameObject[] sections = new GameObject[10];

}
