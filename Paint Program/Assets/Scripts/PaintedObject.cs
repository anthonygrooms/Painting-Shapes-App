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
                print(hit.transform.name + " --- " + gameObject.name);
                if (hit.transform.name == gameObject.name)
                {
                    Destroy(gameObject);
                    Click.audioSources[0].Play(); // Play deleted object sound
                }
            }
        }

        // If this object goes nears the camera, destroy it
        if (transform.position.z < -7.5f && !Click.LaunchMode)
            Destroy(gameObject);
    }

    public void AppendToName(string s)
    {
        gameObject.name += s;
    }
}
