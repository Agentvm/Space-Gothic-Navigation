using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameVariables : MonoBehaviour
{
    public ModalPanel modal_panel;

    public void initialize ()
    {
        // first, a new galaxy at world origin
        GameObject galaxy = (GameObject)Instantiate(Resources.Load("Galaxy"), new Vector3 (0f, 0f, 0f), new Quaternion (0f, 0f, 0f, 1f) );
        Galaxy galaxy_script_reference = galaxy.GetComponent<Galaxy> ();
        galaxy_script_reference.initialize ();

        // add stored systems
        galaxy_script_reference.read_systems_from_file ("systems.txt");
        //add some random Systems
        for ( int i = 0; i <= 10; i++ )
        {
            galaxy_script_reference.add_random_system ();
        }
                
        // add a ship to the galaxy
        Instantiate(Resources.Load("Ship") );
        Ship ship_script_reference = Ship.Instance (); //ship.GetComponent<Ship> ();
        ship_script_reference.initialize (galaxy_script_reference, galaxy_script_reference.get_system ("Sol").SpaceStation, "Starship Enterprise");

        // Focus main Camera
        Camera.main.GetComponent<OrbitCameraZoom> ().focus_main_camera (galaxy_script_reference );


        // UI
        // initialize the Modal Panel for direct User Communication
        modal_panel.gameObject.SetActive (true);
        modal_panel = ModalPanel.Instance ();
        modal_panel.modal_panel_object.SetActive (false);



    }

}
