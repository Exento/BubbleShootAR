using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastButtonPress : MonoBehaviour
{
    void Update()
    {
        // Check if the screen is touched
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Create a ray from the touch position
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is the ARButton
                if (hit.transform.name == "ARButton")
                {
                    hit.transform.GetComponent<ARButton>().Pressed();
                }
            }
        }
    }
}
