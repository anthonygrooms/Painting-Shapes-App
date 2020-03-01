using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintedObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // If this object goes nears the camera, destroy it
        if (transform.position.z < -7.5f)
            Destroy(gameObject);
    }
}
