using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

    private StarSystem system;
    private string description;
    private string type;
    private float rotation_speed = 0f;
    private Vector3 rotation_axis = Vector3.zero;


    // Properties
    public StarSystem System
    {
        get { return system; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    public float RotationSpeed
    {
        get { return rotation_speed; }
        set { rotation_speed = value; }
    }

    public Vector3 RotationAxis
    {
        get { return rotation_axis; }
        set { rotation_axis = value; }
    }

    private Vector3 random_position ()
    {
        float x = Random.Range (-0.0001f, 0.0001f);
        float y = Random.Range (-0.0001f, 0.0001f);
        float z = Random.Range (-0.000001f, 0.000001f);

        return new Vector3 (x, y, z);
    }

    private void Start ()
    {
        if (rotation_speed == 0f)
            rotation_speed = Random.Range (6f, 10) * Vector3.Distance (this.transform.position, this.system.transform.position) * 3;
        if (rotation_axis == Vector3.zero )
            rotation_axis = this.system.transform.up + new Vector3 (Random.Range (0.3f, -0.3f), 1f, Random.Range (0.3f, -0.3f));
    }

    private void Update ()
    {
        transform.RotateAround (this.system.transform.position, rotation_axis/*this.system.transform.up*/, rotation_speed * Time.deltaTime);
    }

    private Vector3 random_sphere_position ()
    {
        Vector3 pos = Random.insideUnitSphere;
        pos.Normalize ();
        pos *= Random.Range (0.3f, 2.5f); // 5 times earth-sun distance,  (8/525600 * 5) lightyears

        return pos + this.transform.position;
    }

    /// <summary>
    /// expects a name, a 3-tuple position and a home system for initialization
    /// name and position can be determined randomly, if not given.
    /// </summary>
    public void initialize ( Vector3 position_relative_to_system, string planet_name, string planet_type, StarSystem system_object )
    {
        system = system_object;
        this.name = planet_name;
        this.transform.position = position_relative_to_system + system.transform.position;
        this.type = planet_type;
        this.tag = "Planet";
    }

    /// <summary>
    /// expects a home system for initialization
    /// name and position are determined randomly.
    /// </summary>
    public void initialize ( StarSystem system_object )
    {
        system = system_object;
        Vector3 random_pos = random_position ();
        this.name = (string)("The System at " + random_pos );
        this.transform.position = random_pos + system.transform.position;
        this.type = "WK";
        this.tag = "Planet";
    }

}
