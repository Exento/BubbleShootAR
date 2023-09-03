using UnityEngine;

public class RaycastButtonPress : MonoBehaviour
{
    void Update()
    {
        // Create a ray and RaycastHit variable to store the hit result
        Ray ray;
        RaycastHit hit;

        // Check if the screen is touched or mouse button is pressed
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            // Create a ray from either the touch or mouse position
            if (Input.touchCount > 0)
            {
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }

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