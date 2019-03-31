using System.Collections;
//using System;
//using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy : MonoBehaviour
{

    private List<StarSystem> systems = new List<StarSystem>();
    private string description;

    // Properties
    public List<StarSystem> Systems
    {
        get { return systems; }
        //set { systems = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    /// <summary>
    /// /// <summary>
    /// Initialise this Galaxy with name and Position
    /// </summary>
    public void initialize (string galaxy_name, Vector3 galaxy_position )
    {
        this.name = galaxy_name;
        this.transform.position = galaxy_position;
        this.tag = "Galaxy";
    }

    /// <summary>
    /// The Galaxy is called Milky Way and it's origin is (0, 0, 0).
    /// </summary>
    public void initialize ()
    {
        this.name = "Milky Way";
        this.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
        this.tag = "Galaxy";
    }

    /// <summary>
    /// Search for StarSystem in this galaxy per string.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>null or the StarSystem matching the string</returns>
    public StarSystem get_system ( string name )
    {
        foreach ( var sys in Systems )
            if ( sys.name == name )
                return sys;
        //print ("ERROR: requested system <" + name + "> could not be found");
        return null;
    }

    /// <summary>
    /// Search for StarSystem in this galaxy per position.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>null or the StarSystem at the given position</returns>
    public StarSystem get_system ( Vector3 pos )
    {
        foreach ( var sys in Systems )
            if ( Vector3.Distance (sys.transform.position, pos ) < 0.00001 || Vector3.Distance (sys.SpaceStation, pos) < 0.00001 )
                return sys;
        //print ("ERROR: requested system at position <" + pos + "> could not be found");
        return null;
    }

    /// <summary>
    /// Add a new System to this galaxy. A new Gameobject will be instatiated as child of this galaxy.
    /// </summary>
    public void add_system ( Vector3 position, string system_name )
    {
        GameObject system = (GameObject)Instantiate(Resources.Load("StarSystem"), this.transform );
        system.GetComponent<StarSystem> ().initialize (position, system_name, this );
        systems.Add (system.GetComponent<StarSystem> () );
    }

    /// <summary>
    /// Add a random System to this galaxy. A new Gameobject will be instatiated as child of this galaxy.
    /// </summary>
    public void add_random_system ( )
    {
        GameObject system = (GameObject)Instantiate(Resources.Load("StarSystem"), this.transform );
        StarSystem system_script_reference = system.GetComponent<StarSystem> ();
        system_script_reference.initialize (this);
        int range = Random.Range (0, 3);
        for ( int i = 0; i < range; i++ )
        {
            system_script_reference.add_random_planet ();
        }
        systems.Add (system.GetComponent<StarSystem> ());
    }

    /// <summary>
    /// add Systems from text file with format:
    ///     system_name#type#x#y#z#{planet1,type1,planet2,type2,planet3,type3, ...};
    /// Example:
    ///     Sol#G#0.0#0.0#0.0#{Terra,WK,Mars,MK,Pluto,MK};
    /// </summary>
    public void read_systems_from_file ( string filename )
    {
        string text = System.IO.File.ReadAllText(filename );

        // remove newlines and carriage returns, then split the text into lines by the ';' sign
        text = text.Replace ("\r", "");
        text = text.Replace ("\n", "");
        string[] lines = text.Split(';');

        foreach ( string line in lines )
        {
            // removing whitespaces at both ends, then split the line into parts at the # sign
            string line_trimmed = line.Trim (' ');
            string[] parts = line_trimmed.Split ('#');

            if ( parts.Length == 6 || parts.Length == 5 )
            {
                try
                {


                    // try parsing the position
                    float x, y, z;
                    if ( parts[2].Contains (".") && parts[3].Contains (".") && parts[3].Contains (".") )
                    {
                        x = float.Parse (parts[2]);
                        y = float.Parse (parts[3]);
                        z = float.Parse (parts[4]);
                    }
                    else
                    {
                        print ("in Galaxy.cs, read_systems_from_file: Problem occured while parsing the following line: \n" + line_trimmed + "\nSkipping line.");
                        continue;
                    }
                    
                    // instantiate gameObject from prefab out of recource folder and initialize the script attached with the parsed data (position and name )
                    GameObject system = (GameObject)Instantiate(Resources.Load("StarSystem"), this.transform );
                    StarSystem system_script = system.GetComponent<StarSystem> ();
                    //system_script.initialize (new Vector3 (-y, -z, x), parts[0], this); // swapped y and z to comform with unity standard
                    system_script.initialize (new Vector3 (-y, -z, -x), parts[0], this); // swapped y and z to comform with unity standard
                    system_script.Type = parts[1];

                    if (parts.Length == 6 )
                    {
                        // process and add planets
                        parts[5] = parts[5].Trim ('{');
                        parts[5] = parts[5].Trim ('}');
                        string[] planets_and_types = parts[5].Split (',');
                        for ( int i = 0; i < planets_and_types.Length; i += 2 )
                        {
                            system_script.add_planet (planets_and_types[i], planets_and_types[i + 1]);
                        }
                    }
                    // store script reference
                    Systems.Add (system.GetComponent<StarSystem> ());
                }
                catch ( UnityException exception )
                {
                    print ("in Galaxy.cs, read_systems_from_file: Exception \"" + exception.ToString () + "\" occured while processing the following line: \n" + line_trimmed);
                }
                catch ( MissingReferenceException exception )
                {
                    print ("in Galaxy.cs, read_systems_from_file: Exception \"" + exception.ToString () + "\" occured while processing the following line: \n" + line_trimmed);
                }
                catch ( UnassignedReferenceException exception )
                {
                    print ("in Galaxy.cs, read_systems_from_file: Exception \"" + exception.ToString () + "\" occured while processing the following line: \n" + line_trimmed);
                }
                catch ( KeyNotFoundException exception )
                {
                    print ("in Galaxy.cs, read_systems_from_file: Exception \"" + exception.ToString () + "\" occured while processing the following line: \n" + line_trimmed);
                }

            }
            else
            {
                if ( line_trimmed != "")
                    print ("in Galaxy.cs, read_systems_from_file: Wrong number of '#' while processing the following line: \n" + line_trimmed);
            }

        }

    }

    public StarSystem farthest_system ()
    {
        StarSystem farthest_sys = null;
        float max_dist = 0;

        foreach (StarSystem sys in this.systems )
        {
            float dist = Vector3.Distance (this.transform.position, sys.transform.position);
            if ( dist > max_dist )
            {
                max_dist = dist;
                farthest_sys = sys;
            }
                
        }

        return farthest_sys;
    }

}
