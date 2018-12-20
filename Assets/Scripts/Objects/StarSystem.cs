using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// system equals a Star like Sol in the Unity representation
public class StarSystem : MonoBehaviour {

    private Galaxy galaxy = null; // the galaxy the system is in
    private string type;
    private List<Planet> planets = new List<Planet>();
    private Vector3 outer_space_station;
    private string description;

    // Properties
    public Galaxy Galaxy
    {
        get { return galaxy; }
    }
    
    public List<Planet> Planets
    {
        get { return planets; }
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

    public Vector3 SpaceStation
    {
        get { return outer_space_station; }
        set { outer_space_station = value; }
    }

    private Vector3 random_position ()
    {
        float x = Random.Range (-54.33f, 0);
        float y = Random.Range (-7.23f, 0);
        float z = Random.Range (-36.06f, 0);

        return new Vector3 (x, y, z);
    }
    
    private Vector3 random_position (Vector3 lower_bounds, Vector3 upper_bounds )
    {
        float x = Random.Range (lower_bounds.x, upper_bounds.x);
        float y = Random.Range (lower_bounds.y, upper_bounds.y);
        float z = Random.Range (lower_bounds.z, upper_bounds.z);

        return new Vector3 (x, y, z);
    }
    
    private Vector3 random_sphere_position ( )
    {
        Vector3 pos = Random.insideUnitSphere;
        pos.Normalize ();
        pos *= Random.Range (0.3f, 2.5f);

        //0.0000761f; // 5 times earth-sun distance,  (8/525600 * 5) lightyears

        return pos + this.transform.position;
    }

    /// <summary>
    /// expects a name, a 3-tuple position and a home galaxy for initialization
    /// position and name can be determined randomly, if not given.
    /// </summary>
    public void initialize (Vector3 position_relative_to_galaxy, string system_name, Galaxy galaxy_object )
    {
        this.galaxy = galaxy_object;
        this.transform.position = position_relative_to_galaxy + galaxy_object.transform.position;
        this.name = system_name;
        this.outer_space_station = this.transform.position - new Vector3 (0f, 0f, 3f);
        this.tag = "Star System";

        //Debug.Log (this.name + " has been initialized.");
    }

    /// <summary>
    /// expects a home galaxy for initialization
    /// name and position are determined randomly.
    /// </summary>
    public void initialize ( Galaxy galaxy_object )
    {
        this.galaxy = galaxy_object;
        this.transform.position = random_position () + galaxy_object.transform.position;
        this.name = (string)("The System at " + this.transform.position);
        this.outer_space_station = this.transform.position - new Vector3 (0f, 0f, 3f);
        this.tag = "Star System";
        this.type = "F";

       // Debug.Log (this.name + " has been initialized.");
    }

    /// <summary>
    /// Add a new Planet to this System. A new Gameobject will be instatiated as child of this System.
    /// </summary>
    /// <param name="position">Relative to parent System</param>
    /// <param name="planet_type"></param>
    /// <param name="rotation_speed">Speed of rotation around this system</param>
    /// <param name="rotation_axis">directional axis the planet will be rotating around (the position is this systems coordinates)</param>
    public void add_planet ( Vector3 position, string planet_name, string planet_type, float rotation_speed, Vector3 rotation_axis )
    {
        GameObject planet = (GameObject)Instantiate(Resources.Load("Planet"), this.transform );
        Planet planet_script = planet.GetComponent<Planet> ();
        planet_script.initialize (position, planet_name, planet_type, this);
        planet_script.RotationSpeed = rotation_speed;
        planet_script.RotationAxis = rotation_axis;
        planets.Add (planet.GetComponent<Planet> ());
        
    }

    /// <summary>
    /// Add a new Planet to this System. A new Gameobject will be instatiated as child of this System.
    /// </summary>
    /// <param name="position">Relative to parent System</param>
    /// <param name="planet_type"></param>
    public void add_planet (Vector3 position, string planet_name, string planet_type )
    {
        GameObject planet = (GameObject)Instantiate(Resources.Load("Planet"), this.transform );
        planet.GetComponent<Planet> ().initialize (position, planet_name, planet_type, this );
        planets.Add (planet.GetComponent<Planet> () );
    }

    /// <summary>
    /// Add a new Planet to this System. A new Gameobject will be instatiated as child of this System. The Planet will be placed in a row with the other Planets.
    /// </summary>
    /// <param name="planet_type"></param>
    public void add_planet ( string planet_name, string planet_type )
    {
        // in line with the other planets; realtive to system
        Vector3 position = new Vector3 (this.transform.localScale.x + 0.2f * (planets.Count + 1), 0f, 0f);

        GameObject planet = (GameObject)Instantiate(Resources.Load("Planet"), this.transform );
        planet.GetComponent<Planet> ().initialize (position, planet_name, planet_type, this);
        planets.Add (planet.GetComponent<Planet> ());
    }

    /// <summary>
    /// Add a random Planet to this System. A new Gameobject will be instatiated as child of this System.
    /// </summary>
    public void add_random_planet ( )
    {
        // in line with the other planets; realtive to system
        Vector3 position = new Vector3 (0.4f + 0.2f * (planets.Count + 1), 0f, 0f);

        string name = "";
        if (planets.Count == 0)
            name = ((planets.Count + 1) + "st planet of " + this.name).ToString ();
        else
            name = ((planets.Count + 1) + "nd planet of " + this.name).ToString ();

        GameObject planet = (GameObject)Instantiate(Resources.Load("Planet"), this.transform );
        planet.GetComponent<Planet> ().initialize (position, name, "WK", this);
        planets.Add (planet.GetComponent<Planet> ());
    }

    public float distance_from_galaxy_center ()
    {
        return Vector3.Distance (this.transform.position, galaxy.transform.position);
    }

}
