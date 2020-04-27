using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereObject : MonoBehaviour
{
    private MeshRenderer mR;
    // Start is called before the first frame update
    void Start()
    {
        mR = GetComponent<MeshRenderer>();
        mR.enabled = (Click.GetAnimationState() == 2 ? true : false);
    }

    // Update is called once per frame
    void Update()
    {
        mR.enabled = (Click.GetAnimationState() == 2 ? true : false);
    }
}
