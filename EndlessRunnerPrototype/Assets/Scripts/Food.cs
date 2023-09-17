using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float rotationSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerManager.totalPoints += 1;
            Destroy(gameObject);
            Debug.Log("Total points: " + PlayerManager.totalPoints);
        }
    }
}
