using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintedObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if the player clicked this object
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray,out RaycastHit hit))
            {
                if (hit.transform.name == gameObject.name)
                    Destroy(gameObject);
            }
        }

        // If this object goes nears the camera, destroy it
        if (transform.position.z < -7.5f && !Click.LaunchMode)
            Destroy(gameObject);
    }
}
