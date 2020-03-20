using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Click : MonoBehaviour
{
    // Declare Variables

    //Colors
    public Slider[] colorSliders = new Slider[3]; // References to color sliders
    [SerializeField]
    [Range(0,1)]
    private float maxRed, maxGreen, maxBlue;

    //Scale
    [SerializeField]
    public Slider sizeSlider; // References to size slider
    [Range(.1f, 4)]
    private float maxSize;

    // Physics
    [SerializeField]
    private bool hasPhysics, hasGravity;
    private Rigidbody shapeRigidBody;
    [SerializeField]
    public Slider luminositySlider;
    private float luminosity;

    // Destroy
    [SerializeField]
    private bool destroy;

    // Primitive
    public Dropdown shapeDropDown; // References to drop down menu
    private PrimitiveType[] primtiveOptions = new PrimitiveType[] { PrimitiveType.Capsule, PrimitiveType.Sphere, PrimitiveType.Cube }; // List of primitives that can be painted
    private GameObject primitive;
    private List<GameObject> primitives = new List<GameObject>(); // List of painted primitives

    // Start is called before the first frame update
    void Start()
    {
        //Initialize variables
        destroy = true;
        hasPhysics = false;
        hasGravity = false;
        luminosity = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Update color and size data realtime based on sliders
        maxRed = colorSliders[0].value;
        maxGreen = colorSliders[1].value;
        maxBlue = colorSliders[2].value;
        maxSize = sizeSlider.value;

        //If user is holding down left click, paint the desired primitive to the screen with desired attributes
        if (Input.GetMouseButton(0))
        {
            Vector3 clickPosition = -Vector3.one; // Default the click position

            //Use raycasting to position the primitive
            Plane plane = new Plane(Vector3.forward, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distancetoPlane;
            if (plane.Raycast(ray,out distancetoPlane))
            {
                clickPosition = ray.GetPoint(distancetoPlane);
            }
            
            primitive = GameObject.CreatePrimitive(primtiveOptions[shapeDropDown.value]); // Instantiate the desired primitive
            primitive.AddComponent<PaintedObject>(); /// Add PaintedObject component to primitive
            primitives.Add(primitive); // Add the primtive to the primitive list
            primitive.transform.position = clickPosition; //Reposition the object
            Renderer renderer = primitive.GetComponent<Renderer>();
            Material mat = renderer.material;
            renderer.material.color = new Color(Random.Range(0, maxRed), Random.Range(0, maxGreen), Random.Range(0, maxBlue), 1); //Randomize color
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", new Vector4(mat.color.r, mat.color.g, mat.color.b) * luminosity);
            //DynamicGI.SetEmissive(renderer,renderer.material.color)
            primitive.transform.localScale = new Vector3(Random.Range(0, maxSize), Random.Range(0, maxSize), Random.Range(0, maxSize));// Randomize size

            // Apply physics to the primitive if necessary
            if (hasPhysics)
            {
                shapeRigidBody = primitive.AddComponent<Rigidbody>();
                shapeRigidBody.useGravity = hasGravity; // Turn gravity off if necessary
            }

            // Destroy the primitive in three seconds if neccessary
            if (destroy)
                Destroy(primitive, 3);
        }
    }

    // Negate the value of destroy
    public void ChangeDestroy()
    {
        destroy = !destroy;
    }

    // Negate the value of hasPhysics
    public void ChangeHasPhysics()
    {
        hasPhysics = !hasPhysics;
    }

    // Negate the value of hasGravity
    public void ChangeHasGravity()
    {
        hasGravity = !hasGravity;
    }

    // Update luminosity of objects
    public void ChangeLuminosity()
    {
        print(luminosity);
        luminosity = luminositySlider.value;
    }

    //Destroy all primitives
    public void DestroyPrimitives()
    {
        for (int i = primitives.Count - 1; i >= 0; i--)
            Destroy(primitives[i]);
    }
}
