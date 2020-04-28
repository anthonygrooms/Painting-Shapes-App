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

    // Primitives
    public Dropdown shapeDropDown; // Reference to shape drop down menu
    public Dropdown animationDropDown; // Reference to animation drop down menu

    /* 2D array of gameobjects:
     * (rows corresponds to type of animation)
     * (column corresponds to type of shape)
     */
    //public GameObject[] cubes, capsules, spheres;
    //private GameObject[,] primitiveOptions = new GameObject[3,4];
    public GameObject[] primitiveOptions = new GameObject[3];
    private GameObject primitive;
    private List<GameObject> primitives = new List<GameObject>(); // List of painted primitives
    private Animator anim; // Animator

    // Sounds
    public static AudioSource[] audioSources;

    // Animation Information
    private static int animationState = 0;
    private float animationSpeed = 1f;
    public float speed;
    public Toggle randomizeSpeed;
    public Toggle randomizeType;
    public static bool randomizeTypeValue;

    // Start is called before the first frame update
    void Start()
    {
        /*
        GameObject[][] shapes = new GameObject[][] { cubes, capsules, spheres };
        // Fill the primitiveOptions with the primitive prefabs
        for (int i = 0; i < shapes.Length; i++)
        {
            for (int j = 0; j < shapes[i].Length; j++)
            {
                primitiveOptions[i, j] = shapes[i][j];
            }
        }
        */

        //Initialize variables
        objectCount = 0;
        destroy = true;
        hasPhysics = false;
        hasGravity = false;
        luminosity = 0.0f;
        audioSources = GetComponents<AudioSource>();
        speed = 1;
        randomizeTypeValue = false;
    }

    // Update is called once per frame
    void Update()
    {
        randomizeTypeValue = randomizeType.isOn;

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

        // Map 1,2,3 and 4 keys to animations

        if (!randomizeType.isOn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeAnimationState(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeAnimationState(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeAnimationState(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeAnimationState(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeAnimationState(4);
        }

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

            // Instantiate the desired primitive with the desired animation
            primitive = Instantiate(primitiveOptions[shapeDropDown.value]);
            primitive.transform.SetParent(transform);

            objectCount++; // Increase the object count
            primitive.AddComponent<PaintedObject>(); // Add PaintedObject component to primitive
            primitives.Add(primitive); // Add the primtive to the primitive list
            primitive.name += objectCount; // Rename the object
            primitive.transform.GetChild(0).GetComponent<PaintedObject>().AppendToName(objectCount.ToString());
            Renderer renderer = primitive.GetComponent<Renderer>();

            // If there is a renderer, assign it a random color
            if (renderer != null)
            {
                Material mat = renderer.material;
                mat.color = new Color(Random.Range(0, maxRed), Random.Range(0, maxGreen), Random.Range(0, maxBlue), 1); //Randomize color
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", new Vector4(mat.color.r, mat.color.g, mat.color.b) * luminosity);
            }

            // If there are children who have renderers, assign it a random color
            foreach (Transform child in primitive.transform)
            {
                Material mat = child.gameObject.GetComponent<Renderer>().material;
                Material matChild = null;
                if (child.childCount > 0)
                    matChild = child.GetChild(0).gameObject.GetComponent<Renderer>().material;
                if (mat != null)
                {
                    mat.color = new Color(Random.Range(0, maxRed), Random.Range(0, maxGreen), Random.Range(0, maxBlue), 1); //Randomize color
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", new Vector4(mat.color.r, mat.color.g, mat.color.b) * luminosity);

                    if (matChild != null)
                    {
                        matChild.color = new Color(Random.Range(0, maxRed), Random.Range(0, maxGreen), Random.Range(0, maxBlue), 1); //Randomize color
                        matChild.EnableKeyword("_EMISSION");
                        matChild.SetColor("_EmissionColor", new Vector4(mat.color.r, mat.color.g, mat.color.b) * luminosity);
                    }
                }
            }

            Animator anim;
            anim = primitive.transform.GetComponent<Animator>();
            if (anim != null)
            {
                if (randomizeType.isOn)
                    animationState = Random.Range(0, animationDropDown.options.Count);
                anim.SetInteger("state", animationState);
                if (randomizeSpeed.isOn)
                    anim.speed = Random.Range(0, 2f);
                else
                    anim.speed = speed;
            }
            else
                foreach (Transform child in primitive.transform)
                {
                    anim = child.transform.GetComponent<Animator>();
                    if (anim != null)
                    {
                        if (randomizeType.isOn)
                            animationState = Random.Range(0, animationDropDown.options.Count);
                        anim.SetInteger("state", animationState);
                        if (randomizeSpeed.isOn)
                            anim.speed = Random.Range(0, 2f);
                        else
                            anim.speed = speed;
                    }
                }

            /*
            // Set the state for each animator
            foreach (Transform child in transform)
            {
                Animator childAnimator = child.transform.GetComponent<Animator>();
                if (childAnimator != null)
                {
                    childAnimator.SetInteger("state", animationState);
                    if (randomizeSpeed.isOn)
                        childAnimator.speed = Random.Range(0,2);
                    else
                        childAnimator.speed = speed;
                }
                else
                foreach (Transform grandChild in child)
                {
                    Animator grandChildAnimator = grandChild.transform.GetComponent<Animator>();
                    if (grandChildAnimator != null)
                        {
                            grandChildAnimator.SetInteger("state", animationState);
                            if (randomizeSpeed.isOn)
                                grandChildAnimator.speed = Random.Range(0,2);
                            else
                                grandChildAnimator.speed = speed;
                        }
                }
            }
            */

            //DynamicGI.SetEmissive(renderer,renderer.material.color)
            primitive.transform.localScale = new Vector3(maxSize, maxSize, maxSize);
            
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

    // Changes the animation state
    public void ChangeAnimationState(int temp)
    {
        animationState = temp;
        if (animationDropDown.options.Count > animationState)
            animationDropDown.value = animationState;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Animator>()!=null)
            {
                child.GetComponent<Animator>().SetInteger("state", animationState);
            }
            foreach (Transform grandChild in child)
            {
                if (grandChild.gameObject.GetComponent<Animator>() != null)
                {
                    grandChild.gameObject.GetComponent<Animator>().SetInteger("state", animationState);
                }
            }
        }
    }

    // Get Animation State
    public static int GetAnimationState()
    {
        return animationState;
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

    public void ChangeAnimationDropDown(int i)
    {
        animationDropDown.ClearOptions();
        if (i == 0)
        {
            animationDropDown.AddOptions(new List<string>() {
            "No Animation",
            "Spin",
            "Bounce With Sphere",
            "Bounce",
            "Diamond"});
        }
        else if (i == 1)
        {
            animationDropDown.AddOptions(new List<string>() {
            "No Animation",
            "Spin",
            "Pulse",
            "Split"});
        }
        else if (i == 2)
        {
            animationDropDown.AddOptions(new List<string>() {
            "No Animation",
            "In And Out",
            "Bouncy",
            "Circle"});
        }
    }

    // Change Animation Speed
    public void ChangeAnimationSpeed(float speed)
    {
        this.speed = speed;
    }

    // Randomize Type
    public void RandomizeType(bool b)
    {
        if (!b)
            animationState = animationDropDown.value;
    }

    // Change Shape
    public void ChangeShape(int i)
    {
        if (i != 0 && animationState == 4)
            animationState = animationDropDown.value = 3;
    }

    //Destroy all primitives
    public void DestroyPrimitives()
    {
        for (int i = primitives.Count - 1; i >= 0; i--)
            Destroy(primitives[i]);
    }
}
