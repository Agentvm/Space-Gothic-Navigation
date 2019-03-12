using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ship : MonoBehaviour {

    private Galaxy galaxy = null; // galaxy it is in
    private Vector3 goal;
    private Vector3 current_planning_position;
    private List<Vector3> path = new List<Vector3>();
    private LineRenderer line_renderer_reference;
    private string description;
    private ModalPanel modal_panel;

    // Ship Values
    private Character pilot = new Character ();
    private List<Character> crew = new List<Character> ();
    private double max_jump_distance = 15.0; // lightyears
    private double damage = -300000;
    //private double max_damage = 100;
    private int laseum_units = 200000;
    private bool laseum_emergency_unit = true;
    private double budget = 0;


    private static Ship ship;
    public static Ship Instance ()
    {
        if ( !ship )
        {
            ship = FindObjectOfType (typeof (Ship)) as Ship;
            if ( !ship )
                Debug.LogError ("There needs to be one active Ship script on a GameObject in your scene.");
        }
        return ship;
    }


    // Properties
    public Galaxy Galaxy
    {
        get { return galaxy; }
    }

    public List<Vector3> Path
    {
        get { return path; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public double MaxJumpDistance
    {
        get {return max_jump_distance;}
        set {max_jump_distance = value;}
    }

    public double Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    //public double MaxDamage
    //{
    //    get {return max_damage;}
    //    set {max_damage = value;}
    //}

    public int LaseumUnits
    {
        get {return laseum_units;}
        set {laseum_units = value;}
    }

    public bool LaseumEmergencyUnit
    {
        get {return laseum_emergency_unit;}
        set {laseum_emergency_unit = value;}
    }

    public double Budget
    {
        get {return budget;}
        set {budget = Mathf.Round ((float)value * 100.0f) / 100.0f;}
    }


    void Update ()
    {
        // add the clicked system as pathplanning goal
        if (Input.GetButtonUp ("Fire1") )
        {
            GameObject target = Camera.main.MouseObject ();
            if (target != null && target.tag == "Star System")
            {
                set_goal (target.GetComponent<StarSystem> () );
            }
        }
    }

    void Awake ()
    {
        line_renderer_reference = this.GetComponent<LineRenderer> ();
    }

    void Start ()
    {
        modal_panel = ModalPanel.Instance ();
    }

    /// <summary>
    /// Recursively sets all Childs of this gameobject to the value specified by tag_name
    /// </summary>
    /// <param name="tag_name">The value the tags are set to</param>
    void tag_childs ( string tag_name, Transform trans )
    {
        foreach ( Transform child in trans )
        {
            child.tag = tag_name;
            tag_childs (tag_name, child.transform );
        }
    }

    /// <summary>
    /// expects a name, a 3-tuple position and a home galaxy for initialization
    /// position and name can be determined randomly, if not given.
    /// </summary>
    public void initialize ( Galaxy galaxy_object, Vector3 ship_position, string ship_name )
    {
        this.name = ship_name;
        galaxy = galaxy_object;
        this.transform.position = ship_position;

        // Tag all Ship parts accordingly
        this.tag = "Player Ship";
        tag_childs ("Player Ship", this.transform );
    }

    /// <summary>
    /// expects a name, a 3-tuple position and a home galaxy for initialization
    /// name and position are determined randomly.
    /// </summary>
    public void initialize ( Galaxy galaxy_object )
    {
        this.name = "Starship Enterprise";
        galaxy = galaxy_object;
        this.transform.position = new Vector3 (0.01f, 0.0f, 0.0f);

        // Tag all Ship parts accordingly
        this.tag = "Player Ship";
        tag_childs ("Player Ship", this.transform);
    }

    public void add_crew_member (Character soon_to_be_member)
    {
        crew.Add (soon_to_be_member);
    }


    public void refuel ()
    {
        if ( !galaxy.get_system (this.transform.position) )
        {
            modal_panel.announcement ("Im leeren Weltraum", "In unserer Nähe gibt es keine geeignete Raumstation, an der wir auftanken können.");
            return;
        }

        int price = Random.Range (2, 5);

        for ( int i = 0; i < 2 - LaseumUnits; i++ )
            Budget -= price;
        if (!LaseumEmergencyUnit )
            Budget -= price/2;

        LaseumEmergencyUnit = true;
        LaseumUnits = 2;
    }

    public void repair ()
    {
        if (!galaxy.get_system (this.transform.position))
        {
            modal_panel.announcement ("Im leeren Weltraum", "In unserer Nähe gibt es keine geeignete Raumstation, an der wir das Schiff reparieren können.");
            return;
        }
        
        Budget -= Random.Range (0.2f, 1) * Damage;
        Damage = 0;
    }

    public void balance_budget ()
    {
        Budget = 0;
    }

    /// <summary>
    /// Set a navigation goal in this galaxy by supplying a system name and start the pathplanning algorithm.
    /// </summary>
    /// <param name="goal_system">A String describing a System name in this ship's galaxy</param>
    public void set_goal (string goal_system)
    {
        if ( galaxy.get_system (goal_system) )
            set_goal (galaxy.get_system (goal_system).SpaceStation );
        else
        {
            modal_panel.announcement ("Error", "Das Zielsystem \"" + goal_system + "\" ist in der Datenbank nicht vorhanden.");
            return;
        }

        plan_path ();
    }

    /// <summary>
    /// Set a navigation goal in this galaxy by supplying a position. If the position is in a Star System,
    /// the resulting goal will be it's outer space station. Also starts the pathplanning algorithm.
    /// </summary>
    /// <param name="nav_goal">A Vector3 describing any point in Space</param>
    public void set_goal ( Vector3 nav_goal )
    {
        goal = nav_goal;
        StarSystem sys = galaxy.get_system (nav_goal);

        if ( sys == null ) // no Star System at goal position
        {
            Debug.Log ("Setting goal: " + goal);
            Debug.Log ("[Warning]: No star system detected as destination.");
        }
        else
        {
            goal = sys.SpaceStation;
            Debug.Log ("Setting goal to " + sys.name + " at " + sys.transform.position + ".");
        }

        plan_path ();
    }

    /// <summary>
    /// Set a navigation goal in this galaxy by supplying a StarSystem reference and start the pathplanning algorithm.
    /// </summary>
    /// <param name="nav_goal_system">Any StarSystem which is contained in this.galaxy.Systems</param>
    public void set_goal ( StarSystem nav_goal_system )
    {
        goal = nav_goal_system.SpaceStation;

        Debug.Log ("Setting goal to " + nav_goal_system.name + " at " + nav_goal_system.transform.position + ".");

        plan_path ();
    }

    double get_distance_from_line ( Vector3 start, Vector3 end, Vector3 point )
    {
        return Vector3.Magnitude (Vector3.Cross (point - start, point - end)) / Vector3.Distance (end, start);
    }

    /// <summary>
    /// Finds StarSystem locations in jump range in this galaxy. The locations must be between the ship and this.goal.
    /// </summary>
    /// <param name="current_planning_position"> The current virtual position of the ship in pathplanning </param>
    /// <returns> A List of possible jump locations (Vector3)</returns>
    List<Vector3> find_jump_locations ( double max_jump_distance_in_function )
    {
        // find locations along the way
        List<Vector3> locations = new List<Vector3>();
        double ship_goal_distance = Vector3.Distance (current_planning_position, goal );
        double system_goal_distance;

        foreach ( StarSystem sys in galaxy.Systems )
        {

            system_goal_distance = Vector3.Distance (sys.SpaceStation, goal );
            double ship_system_distance = Vector3.Distance (current_planning_position, sys.SpaceStation );

            // skip if the proposed goal takes us farther back (greedy)
            if ( system_goal_distance >= ship_goal_distance || ship_system_distance > max_jump_distance_in_function || ship_system_distance < 0.0002)
                continue;

            //if location is near the direct line from ship to goal, add it to the possible jump locations
            if ( get_distance_from_line (this.transform.position, goal, sys.SpaceStation ) <= max_jump_distance_in_function * 1.2f)
                locations.Add (sys.SpaceStation );

            //Debug.Log ("Jump Location \"" + sys.name + "\" is " + ship_system_distance + " light years away from us and " + system_goal_distance + " light years away from our goal \"" + galaxy.get_system (goal).name + "\".");

        }
        //if goal has not been set via system or planet, it might not be an available location yet
        if ( !locations.Contains (goal) && Vector3.Distance (current_planning_position, goal) < max_jump_distance_in_function )
            locations.Add (goal);

        return locations;
    }

    /// <summary>
    /// find the point farthest from ship, but still in range
    /// </summary>
    /// <param name="locations"> A Vector3 List of Locations </param>
    /// <param name="current_planning_position"> The virtual position of the ship in pathplanning </param>
    /// <returns>A valid jump location that is closest to this.goal or current_planning_position, if no System was found (returning null or boolean would be cool)</returns>
    Vector3 find_best_jump_location ( List<Vector3> locations )
    {
        double current_best_distance_route_point_to_goal = Vector3.Distance (current_planning_position, goal);
        Vector3 current_best_route_point = current_planning_position;

        double distance_route_point_to_goal = 0.0;

        // choose a location to jump to
        foreach ( Vector3 route_point in locations )
        {
            //double distance_to_route_point = Vector3.Distance (route_point, current_planning_position);
            distance_route_point_to_goal = Vector3.Distance (route_point, goal);

            // find the jump locations that is reachable and at shortest dictance to this.goal
            if ( /*distance_to_route_point < max_jump_distance && */distance_route_point_to_goal < current_best_distance_route_point_to_goal )
            {
                current_best_route_point = route_point;
                current_best_distance_route_point_to_goal = distance_route_point_to_goal;
            }
        }

        return current_best_route_point;
    }

    /// <summary>
    /// repeatedly calls find_jump_locations and picks one of the returned locations to plan a greedy path to this.goal
    /// over star systems and empty space, if necessary.
    /// </summary>
    /// <returns>Success or Failure</returns>
    bool plan_path ()
    {
        path.Clear ();
        path.Insert (0, this.transform.position); // add current position as start point of the path
        int iterations = 0;
        current_planning_position = this.transform.position; // the current virtual position of the ship in pathplanning

        // are you already where you want to be?
        if (Vector3.Distance (goal, this.transform.position) < 0.001 )
        {
            StarSystem goal_system = galaxy.get_system (goal);
            if ( goal_system != null )
                modal_panel.announcement ("Pfadplanung abgebrochen", "Wir befinden uns bereits am Zielort " + goal_system.name + " bei " + goal_system.transform.position);
            else
                modal_panel.announcement ("Pfadplanung abgebrochen", "Unsere Zielposition " + goal + " entspricht unserer jetzigen Position: " + this.transform.position + ".");

            return false;
        }

        // plan
        while ( iterations < 100)
        {
            // find best jump location
            Vector3 route_point = find_best_jump_location ( find_jump_locations (MaxJumpDistance ) );
            
            // if there was no valid StarSystem found as next route point
            // then route point matches the current planning position
            if ( Vector3.Distance (route_point, current_planning_position) < 0.0001 )
            {
                if (!add_route_point_in_space () ) // if already in free space or no route point in double radius found
                    return false; // still no valid solution found, failure
            }
            else // valid system found
            {
                // add the best route point to path
                add_route_point (route_point );
            }            

            // Success condition
            if (path.Count > 0 && path[path.Count - 1] == goal)
            {
                if (path.Count > 2)
                    turn_ship_to_point (path[2] ); // 0 is the start position, 1 the first step and 2 the second
                else
                    turn_ship_to_point (path[1]);
                Debug.Log ("Path planned." );
                plot_path ();

                return true;
            }

            iterations++;
        }

        // failure
        modal_panel.announcement ("Fehler in der Pfadplanung", "Die Pfadplanung wurde nach " + (iterations) + " Iterationen gestoppt. Kein gültiger Pfad konnte bestimmt werden.");
        //print_path ();
        plot_path ();        

        return false;        
    }


    /// <summary>
    /// Make the Ship face a point in space.
    /// </summary>
    /// <param name="point">Any Vector 3</param>
    void turn_ship_to_point (Vector3 point)
    {
        //Quaternion quat = new Quaternion (0,0,0,1 );
        //quat.SetFromToRotation (point, this.transform.position );
        //this.transform.rotation = quat; // turn ship to next jump goal
        //transform.Rotate (0, 90, 0 );
        transform.LookAt (point);
    }

    /// <summary>
    /// Add a System as route point
    /// </summary>
    /// <param name="route_point">A Vector 3 that contains the position of a system</param>
    /// <returns>The route point added</returns>
    void add_route_point (Vector3 route_point )
    {
        if ( !path.Contains (route_point) && Vector3.Distance (route_point, this.transform.position) > 0.001)
        {
            if ( galaxy.get_system (route_point) )
                Debug.Log ("Added \"" + galaxy.get_system (route_point).name + "\" at: " + route_point + " to route, distance to goal: " + Vector3.Distance (route_point, goal) + " lightyears.");
            path.Add (route_point); 
            this.current_planning_position = route_point;
        }
    }
    
    /// <summary>
    /// Adds a route point in space to skip a gap between systems that is greater than this.max_jump_distance
    /// </summary>
    /// <param name="current_planning_position">The current ship position in the pathplanning process</param>
    /// <returns>A valid jump location that is closest to this.goal or current_planning_position, if no System was found (returning null or boolean would be cool)</returns>
    bool add_route_point_in_space ( )
    {
        // find best jump location in double the jump radius
        // calculating a jump point in empty space between the farthest reachable System
        // and the current position
        Vector3 route_point = find_best_jump_location ( find_jump_locations (MaxJumpDistance * 2 ) );
        
        if ( galaxy.get_system (current_planning_position) != null && Vector3.Distance (route_point, current_planning_position) >= 0.0001 ) // currently not in free space and route point in double radius found
        {        
            // make a jump point in empty space
            Vector3 jump_point_in_space = (route_point - current_planning_position); // vector in direction of next system
            jump_point_in_space.Normalize (); // lenght 1
            jump_point_in_space *= (float)MaxJumpDistance; // lenght of max_jump distance
            jump_point_in_space += current_planning_position; // shift into current position, so head of the manufactured vector points from ship in direction of goal.

            Debug.Log ("[Warning]: No valid Systems in range. Added a jump point in empty space at: " + jump_point_in_space + " to route." );
            add_route_point (jump_point_in_space );
            this.current_planning_position = jump_point_in_space;
            return true;
        }
        else // No route point in double radius found
        {
            modal_panel.announcement ("Fehler in der Pfadplanung", "Die Route führt in den leeren Raum. Die Pfadplanung wurde gestoppt, kein gültiger Pfad konnte bestimmt werden.");
            plot_path ();
            path.Clear ();

            return false;
        }
    }

    void print_path ()
    {
        Debug.Log ("--- Path Lenght is " + path.Count + ". Elements: ");
        for (int i = 0; i < path.Count; i++ )
        {
            if ( galaxy.get_system (path[i]) )
                Debug.Log ("System " + galaxy.get_system (path[i]) + "at: " + path[i] );
            else
                Debug.Log ("Position " + path[i]);

            if (Vector3.Distance (path[i], path[i-1]) >= this.MaxJumpDistance )
            {
                Debug.Log ("[Warning]: Jump from path[" + (i - 1) + "] to path [" + i + "] is " + (Vector3.Distance (path[i], path[i-1]) ) + " lightyears long.");
            }
        }
    }

    void plot_path ()
    {
        // Line Drawing
        line_renderer_reference.draw_new_line (path);
    }
    

    /// <summary>
    /// make the next step of the journey. Print how long the step took,
    /// how much fuel was consumed and where you are now
    /// </summary>
    public void initiate_planned_jump ()
    {
        // consume fuel
        if ( LaseumUnits > 0 ) LaseumUnits -= 1;
        else if ( LaseumEmergencyUnit )
            LaseumEmergencyUnit = false;
        else
            modal_panel.announcement ("Fehlercode: D5", "Es sind keinerlei Treibstoffreserven mehr übrig.");

        print ("doing the jump");

        // move the ship one step along the path
        this.transform.position = path[1];
        path.RemoveAt (0);
        if (path.Count > 1)
            turn_ship_to_point (path[1]);
        
        plot_path ();

        // apply damage
        // insert Dialog
        Damage += GameCore.roll_dice (8, 10);

    }

    /// <summary>
    /// Display gameplay options in the ships sensor range.
    /// </summary>
    public void scan_surroundings ( )
    {
        //int d_100 = GameCore.roll_dice ();
    }

}
