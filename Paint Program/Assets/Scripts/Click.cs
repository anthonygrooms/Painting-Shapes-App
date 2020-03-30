using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Click : MonoBehaviour
{
    // Declare Variables

    // Keeps track of how many objects have been created
    static int objectCount;

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
    public Slider RangeSlider;
    public GameObject RangeLabel;
    private float range;
    public static bool LaunchMode { get; set; }
    public Toggle hasPhysicsButton;
    public Text launchModeLabel;

    // Destroy
    [SerializeField]
    private bool destroy;

    // Primitive
    public Dropdown shapeDropDown; // References to drop down menu
    private PrimitiveType[] primtiveOptions = new PrimitiveType[] { PrimitiveType.Capsule, PrimitiveType.Sphere, PrimitiveType.Cube }; // List of primitives that can be painted
    private GameObject primitive;
    private List<GameObject> primitives = new List<GameObject>(); // List of painted primitives

    // Sounds
    public static AudioSource[] audioSources;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize variables
        objectCount = 0;
        destroy = true;
        hasPhysics = false;
        hasGravity = false;
        luminosity = 0.0f;
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update color and size data realtime based on sliders
        maxRed = colorSliders[0].value;
        maxGreen = colorSliders[1].value;
        maxBlue = colorSliders[2].value;
        maxSize = sizeSlider.value;

        // Allow user to control range slider via scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        range += scrollInput * 25;
        RangeSlider.value += scrollInput * 25;
        range = RangeSlider.value = Mathf.Clamp(RangeSlider.value, 0, 100);
        
        //If user is holding down left click, paint the desired primitive to the screen with desired attributes
        if (Input.GetMouseButton(0))
        {
            Vector3 clickPosition = -Vector3.one; // Default the click position

            //Use raycasting to position the primitive
            Plane plane = new Plane(Vector3.forward, -range);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray,out float distancetoPlane))
            {
                clickPosition = ray.GetPoint(distancetoPlane);
            }
            
            primitive = GameObject.CreatePrimitive(primtiveOptions[shapeDropDown.value]); // Instantiate the desired primitive
            objectCount++; // Increase the object count
            primitive.AddComponent<PaintedObject>(); // Add PaintedObject component to primitive
            primitives.Add(primitive); // Add the primtive to the primitive list
            primitive.name += objectCount; // Rename the object
            Renderer renderer = primitive.GetComponent<Renderer>();
            Material mat = renderer.material;
            renderer.material.color = new Color(Random.Range(0, maxRed), Random.Range(0, maxGreen), Random.Range(0, maxBlue), 1); //Randomize color
            renderer.material.EnableKeyword("_EMISSION");
            renderer.material.SetColor("_EmissionColor", new Vector4(mat.color.r, mat.color.g, mat.color.b) * luminosity);
            //DynamicGI.SetEmissive(renderer,renderer.material.color)
            primitive.transform.localScale = new Vector3(Random.Range(0, maxSize), Random.Range(0, maxSize), Random.Range(0, maxSize));// Randomize size
            
            // If launch mode is on, force physics to be on... things cannot be launched without physics :)
            if (LaunchMode)
            {
                hasPhysicsButton.isOn = true;
                RangeSlider.value = range = 0;
                audioSources[1].Play(); // Play the shooting sound
            }

            // Apply physics to the primitive if necessary
            if (hasPhysics)
            {
                shapeRigidBody = primitive.AddComponent<Rigidbody>();
                shapeRigidBody.useGravity = hasGravity; // Turn gravity off if necessary
            }

            // If launch mode is on, launch the objects to the desired coordinate (mouse position of screen into world space)
            if (LaunchMode)
            {
                primitive.transform.position = new Vector3(0, -1, -10); //Reposition the object
                Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,10));
                shapeRigidBody.velocity = 4 * new Vector3(v.x, v.y, range + 10);
            }
            else
            {
                primitive.transform.position = clickPosition; //Reposition the object
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
        luminosity = luminositySlider.value;
    }

    // Update how far out objects spawn
    public void ChangeRange()
    {
        range = RangeSlider.value;
    }

    // Turn launch mode off and on, set the visibility of the range slider/label to oppose the state of the launch mode
    public void ChangeLaunchMode()
    {
        LaunchMode = !LaunchMode;
        launchModeLabel.text = LaunchMode ? "Launch Mode (On)" : "Launch Mode (Off)";
        RangeSlider.gameObject.SetActive(!LaunchMode);
        RangeLabel.SetActive(!LaunchMode);
    }

    //Destroy all primitives
    public void DestroyPrimitives()
    {
        for (int i = primitives.Count - 1; i >= 0; i--)
            Destroy(primitives[i]);
    }
}
