using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class InvSlotVisualizer : MonoBehaviour
{
    [System.Serializable]
    public struct SlotPosition
    {
        public Vector2 position;
        public string label;
    }

    public List<SlotPosition> slots = new();
    public GameObject markerPrefab;

    private List<GameObject> markers = new();

    private void Start()
    {
        foreach (var slot in slots)
        {
            GameObject marker = Instantiate(markerPrefab, transform);
            var rectTransform = marker.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = slot.position;

            // Optional: Add a label 
            //marker.GetComponentInChildren<Text>().text = slot.label;

            markers.Add(marker);
        }
    }

    // Optional: Call this to hide markers after setup (e.g., after debugging)
    public void HideMarkers()
    {
        foreach (var m in markers)
            m.SetActive(false);
    }
}
