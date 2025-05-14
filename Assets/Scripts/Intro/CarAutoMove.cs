using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAutoMove : MonoBehaviour
{
    public float speed = 20f;
    private bool shouldMove = true;

    void Update()
    {
        if (shouldMove)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stopper"))
        {
            shouldMove = false;
        }
    }
}
