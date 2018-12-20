using UnityEngine;
using System.Collections;


[AddComponentMenu ("Camera-Control/3dsMax Camera Style")]
public class OrbitCameraZoom : MonoBehaviour
{
    [SerializeField]private Transform target;
    //private Vector3 targetOffset;
    private float distance = 150.0f;
    private float maxDistance = 800;
    private float minDistance = 6f;
    private float xSpeed = 200.0f;
    private float ySpeed = 200.0f;
    private int yMinLimit = -400;
    private int yMaxLimit = 400;
    private int zoomRate = 150;
    private float panSpeed = 3f;
    private float zoomDampening = 25f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Ship player_ship;
    private SlerpCoroutine slerp_coroutine_script;

    public Transform Target
    {
        get{return target;}
        set{target = value;}
    }

    public float MaxDistance
    {
        get{return maxDistance;}
        set{maxDistance = value;}
    }

    public float MinDistance
    {
        get{return minDistance;}
        set{minDistance = value;}
    }

    void Start ()
    {
        slerp_coroutine_script = Target.GetComponent<SlerpCoroutine> ();
        if ( player_ship == null )
            player_ship = GameObject.FindGameObjectsWithTag ("Player Ship")[0].GetComponent<Ship> (); // There should be only one ship
        Init ();
    }
    void OnEnable () { Init (); }

    public void focus_main_camera ( Galaxy galaxy )
    {
        /*OrbitCameraZoom camera_script = Camera.main.GetComponent<OrbitCameraZoom> ();
        if ( camera_script == null )
            camera_script = Camera.main.gameObject.AddComponent<OrbitCameraZoom> ();*/

        Target.transform.position = galaxy.transform.position;
        Target.transform.rotation = galaxy.transform.rotation;
        StarSystem sys = galaxy.farthest_system ();
        if ( sys != null )
        {
            MaxDistance = sys.distance_from_galaxy_center () * 2;
            MinDistance = sys.transform.localScale.x * 3;
        }
            
            
        Init ();
    }

    public void Init ()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if ( !Target )
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            Target = go.transform;
        }

        distance = Vector3.Distance (transform.position, Target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle (Vector3.right, transform.right);
        yDeg = Vector3.Angle (Vector3.up, transform.up);
    }

    public void Update ()
    {
        if (player_ship.Galaxy.get_system (player_ship.transform.position )) // if ship is dockled in galaxy
            slerp_coroutine_script.setTarget (player_ship.Galaxy.get_system (player_ship.transform.position).transform.position, Target.rotation );
        else
            slerp_coroutine_script.setTarget (player_ship.transform.position, Target.rotation );
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
    void LateUpdate ()
    {
        // If Control and Alt and Middle button? ZOOM!
        if ( Input.GetMouseButton (2) && Input.GetKey (KeyCode.LeftAlt) && Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.LeftAlt) && Input.GetMouseButton (0) )
        {
            desiredDistance -= Input.GetAxis ("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs (desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        else if ( Input.GetMouseButton (1) || Input.GetKey (KeyCode.LeftControl) && Input.GetMouseButton (0)/*&& Input.GetKey (KeyCode.LeftAlt) */)
        {
            xDeg += Input.GetAxis ("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis ("Mouse Y") * ySpeed * 0.02f;

            ////////OrbitAngle

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle (yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler (yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp (currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        else if ( Input.GetMouseButton (2) || Input.GetKey (KeyCode.LeftShift) && Input.GetMouseButton (0) )
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            Target.rotation = transform.rotation;
            Target.Translate (Vector3.right * -Input.GetAxis ("Mouse X") * panSpeed);
            Target.Translate (transform.up * -Input.GetAxis ("Mouse Y") * panSpeed, Space.World);
        }

        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp (desiredDistance, MinDistance, MaxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp (currentDistance, desiredDistance, Time.deltaTime * zoomDampening);

        // calculate position based on the new currentDistance 
        position = Target.position - (rotation * Vector3.forward * currentDistance/* + targetOffset*/);
        transform.position = position;
    }

    private static float ClampAngle ( float angle, float min, float max )
    {
        if ( angle < -360 )
            angle += 360;
        if ( angle > 360 )
            angle -= 360;
        return Mathf.Clamp (angle, min, max);
    }
}