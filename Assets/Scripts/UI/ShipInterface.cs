using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipInterface : MonoBehaviour {

    public Canvas canvas;
    private Ship ship;
    //private ModalPanel modal_panel;

    // Control Panel ---
    public GameObject control_panel;
    // Panel Movement
    private float inital_control_panel_angle; // respective to camera
    private bool control_panel_active = false;
    private bool rotate_control_panel = false;
    // Buttons
    public Button button_display_ship_values;
    public Button button_set_goal;
    public InputField search_field_goal;
    public Button button_initiate_jump;
    public Button button_log_out;
    public Button button_debug_set_position;
    public InputField search_field_position;


    // Status Panel ---
    public GameObject ship_status_panel;
    // Panel Movement
    private float inital_ship_status_panel_angle; // respective to camera
    private bool ship_status_panel_active = false;
    private bool rotate_ship_status_panel = false;
    // Content
    public Text text_damage_value;
    public Button button_repair;
    public Text text_fuel_units;
    public Button button_refuel;
    public Text text_emergency_fuel_available;
    public Text text_max_jump_distance;
    public Text text_budget_amount;
    public Button button_balance_budget;

    // Jump Panel ---
    public GameObject jump_panel;


    // General Info Text ---
    public Text text_game_info;

    // private variables
    private float rotation_speed = 100f;
    private float button_cooldown = 0.4f;
    private float last_key_press = 0f;


    void Start ()
    {
        ship = Ship.Instance (); // There should be only one ship
        //modal_panel = ModalPanel.Instance (); // As with a Modal Panel
        initialize_panel_movement ();
        initialize_buttons ();
        
    }

    private void initialize_buttons ()
    {
        button_display_ship_values.onClick.AddListener (activate_ship_status_panel);
        button_refuel.onClick.AddListener (ship.refuel);
        button_repair.onClick.AddListener (ship.repair);
        button_balance_budget.onClick.AddListener (ship.balance_budget);
    }

    /// <summary>
    /// // At the Beginning of the game, save the initial alignment of the Panels respective to the Screen, then quickly turn them out of visible space
    /// </summary>
    private void initialize_panel_movement ()
    {
        // At the Beginning of the game, save the initial alignment of the Control Panel respective to the Screen, then quickly turn it out of visible space
        inital_control_panel_angle = Vector3.Angle (control_panel.transform.forward, Camera.main.transform.forward);
        while ( !rotate_out_of_screen (control_panel.transform, true) ) ; // deactivate fast

        // As with the Ship Status Panel
        inital_ship_status_panel_angle = Vector3.Angle (ship_status_panel.transform.forward, Camera.main.transform.forward);
        while ( !rotate_out_of_screen (ship_status_panel.transform, true, true) ) ; // deactivate fast
    }


    /// <summary>
    /// Rotates a given object around the edge of the screen until it is no longer visible
    /// </summary>
    /// <param name="canvas_child_tf">The object to be rotated</param>
    public bool rotate_out_of_screen ( Transform canvas_child_tf, bool fast = false, bool left_edge = false )
    {
        // get current object position in screen space
        Vector3 point_screen_space = Camera.main.WorldToViewportPoint (canvas_child_tf.position);
        if ( point_screen_space.x < 1.1f && point_screen_space.y < 1.1f && point_screen_space.x > -0.1f && point_screen_space.y > -0.1f ) // check if still in screen space
        {
            // rotate around edge of screen
            Vector3 edge_of_screen = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0.5f, canvas.planeDistance));
            if ( left_edge )
            {
                edge_of_screen = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.5f, canvas.planeDistance));
                if ( fast )
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, -1000 * Time.deltaTime);
                else
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, -rotation_speed * Time.deltaTime);
            }
            else
            {
                if ( fast )
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, 1000 * Time.deltaTime);
                else
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, rotation_speed * Time.deltaTime);
            }
            
        }
        else
            return true;

        return false; // not done yet
    }

    /// <summary>
    /// Rotates a given object around the edge of the screen until it is at a specified angle in respect to the Camera
    /// </summary>
    /// <param name="canvas_child_tf">The object to be rotated</param>
    /// <param name="initial_angle_to_camera">inital_panel_angle = Vector3.Angle (panel.transform.forward, Camera.main.transform.forward );</param>
    /// <returns></returns>
    public bool rotate_into_screen ( Transform canvas_child_tf, float initial_angle_to_camera, bool fast = false, bool left_edge = false )
    {
        // get angle of turned object to camera transform
        float angle_to_camera_transform = Vector3.Angle (canvas_child_tf.forward, Camera.main.transform.forward);
        if ( angle_to_camera_transform > initial_angle_to_camera )
        {
            // rotate around edge of screen
            Vector3 edge_of_screen = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0.5f, canvas.planeDistance));
            if ( left_edge )
            {
                edge_of_screen = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.5f, canvas.planeDistance));
                if ( fast )
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, 1000 * Time.deltaTime);
                else
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, rotation_speed * Time.deltaTime);
            }
            else
            {
                if ( fast )
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, -1000 * Time.deltaTime);
                else
                    canvas_child_tf.RotateAround (edge_of_screen, Camera.main.transform.up, -rotation_speed * Time.deltaTime);
            }
        }
        else
            return true;

        return false; // not done yet
    }

    // Button clicks

    public void ClickEndGame ()
    {
        Application.Quit ();
    }

    public void ClickInitiateJump ()
    {
        JourneyStepValidater validater = new JourneyStepValidater ();
        validater.validate ();
    }

    public void activate_ship_status_panel ()
    {
        rotate_ship_status_panel = true;
        text_game_info.enabled = false;
    }

    private void update_text_values ()
    {
        text_damage_value.text = ship.Damage.ToString ();
        text_fuel_units.text = ship.LaseumUnits.ToString ();
        text_emergency_fuel_available.text = ship.LaseumEmergencyUnit.ToString ();
        text_max_jump_distance.text = ship.MaxJumpDistance.ToString ();
        text_budget_amount.text = ship.Budget.ToString () + " EC";
    }

    private void OnGUI ()
    {
        update_text_values ();

        // Jump
        if ( Input.GetKeyUp ("space") && Time.time - last_key_press > button_cooldown )
        {
            last_key_press = Time.time;
            ClickInitiateJump ();
        }
            

        // Control Panel Logic
        if ( Input.GetKeyDown ("tab") )
            rotate_control_panel = true;

        if ( control_panel_active && rotate_control_panel == true) //deactivate
        {
            rotate_control_panel = !rotate_out_of_screen (control_panel.transform);
            control_panel_active = rotate_control_panel;
            text_game_info.enabled = false;
        }
        else if ( rotate_control_panel == true ) //activate
        {
            rotate_control_panel = !rotate_into_screen (control_panel.transform, inital_control_panel_angle );
            control_panel_active = !rotate_control_panel;
        }

        // Ship Status Panel Logic
        if ( Input.GetKeyDown ("q") )
            activate_ship_status_panel ();

        if ( ship_status_panel_active && rotate_ship_status_panel == true ) //deactivate
        {
            rotate_ship_status_panel = !rotate_out_of_screen (ship_status_panel.transform, false, true);
            ship_status_panel_active = rotate_ship_status_panel;
            text_game_info.enabled = false;
        }
        else if ( rotate_ship_status_panel == true ) //activate
        {
            rotate_ship_status_panel = !rotate_into_screen (ship_status_panel.transform,/* inital_control_panel_angle*/ inital_ship_status_panel_angle, false, true); // see change!
            ship_status_panel_active = !rotate_ship_status_panel;
        }
    }
}


public class JourneyStepValidater
{
    private Ship ship;
    private ModalPanel modal_panel;
    private bool general_check = false;
    private bool fuel_check = false;
    private bool damage_check = false;
    private bool cancel = false;


    public JourneyStepValidater ()
    {
        ship = Ship.Instance ();
        modal_panel = ModalPanel.Instance ();
    }

    public void validate ( )
    {
        if ( cancel ) return;

        if ( general_check && fuel_check && damage_check )
        {
            ship.initiate_planned_jump ();
            return;
        }
            

        if ( !general_check )
        {
            validate_journey_step ();
            if (!general_check)
                return;
        }

        if ( !fuel_check && !cancel)
        {
            check_fuel ();
            if ( !fuel_check )
                return;
        }

        if ( !damage_check && !cancel )
        {
            check_damage ();
            if ( !damage_check )
                return;
        }

        if ( general_check && fuel_check && damage_check )
        {
            ship.initiate_planned_jump ();
        }
    }

    private void approve_damage ()
    {
        Debug.Log ("approved damage.");
        damage_check = true;
        validate ();
    }

    private void approve_fuel ()
    {
        Debug.Log ("approved fuel.");
        fuel_check = true;
        validate ();
    }

    private void cancel_validation ()
    {
        Debug.Log ("cancelling validation.");
        cancel = true;
    }

    private void validate_journey_step ()
    {
        Debug.Log ("checking General.");

        // Catch Errors
        if ( ship.Path.Count == 0 )
        {
            Debug.Log ("[Error]: There is no path set. Choose a valid goal first.");
            modal_panel.announcement ("Fehlercode: D324c4", "Es wurde keine Reisesoute festgelegt. Bitte wählen sie ein gültiges Reisziel aus.");
            cancel_validation ();
            return;
        }

        if ( Vector3.Distance (ship.Path[ship.Path.Count - 1], ship.transform.position) < 0.0001 )
        {
            Debug.Log ("[Error]: You already arrived.");
            modal_panel.announcement ("Fehlercode: D92a4", "Sie befinden sich bereits am gewählten Zielort.");
            cancel_validation ();
            return;
        }

        if ( ship.Path.Count > 1 )
        {
            if ( Vector3.Distance (ship.transform.position, ship.Path[1]) > ship.MaxJumpDistance + 0.1f )
            {
                Debug.Log ("[Error]: There must have been an Error in pathplanning, because the next jump is too far to be made.\n" + "Dist: " + Vector3.Distance (ship.transform.position, ship.Path[1]));
                modal_panel.announcement ("Fehlercode: D1715", "Scheinbar hat es einen Fehler in der Pfadplanung gegeben, da der nächste Routenpunkt nicht innerhalb unserer Sprungreichweite liegt.");
                cancel_validation ();
                return;
            }
        }

        if ( ship.LaseumUnits == 0 && !ship.LaseumEmergencyUnit )
        {
            Debug.Log ("[Error]: You have no Laseum left to fuel this jump");
            modal_panel.announcement ("Fehlercode: D5", "Es sind keinerlei Treibstoffreserven mehr übrig.");
            cancel_validation ();
            return;
        }

        Debug.Log ("validate_journey_step () returning true.");
        general_check = true;
    }

    void check_fuel ()
    {
        Debug.Log ("checking Fuel.");

        if ( ship.LaseumUnits == 0 && ship.LaseumEmergencyUnit )
        {
            Debug.Log ("[Warning]: You have no Laseum left but your emergency reserve.");
            StarSystem next_goal = ship.Galaxy.get_system (ship.Path[1] );
            if ( next_goal != null )
                modal_panel.question ("Laseumreserven kritisch", "Bis auf die Notreserve sind unsere Treibstoffvorräte erschöpft, Kapitän. " +
                 "Mit diesem Sprung sollten wir versuchen, ein bewohntes System zu erreichen. Wollen sie den Sprung nach " + next_goal.name + " durchführen?", approve_fuel, cancel_validation);
            else
                modal_panel.question ("Achtung! Laseumreserven kritisch", "Bis auf die Notreserve sind unsere Treibstoffvorräte erschöpft, Kapitän. " +
                "Mit diesem Sprung werden wir im freien Raum landen, unfähig zu einem weiteren Hyperraumsprung.\nWollen sie den Sprung wirklich durchführen?", approve_fuel, cancel_validation);
            return;
        }

        Debug.Log ("check_fuel () returning true.");
        fuel_check = true;
    }

    void check_damage ()
    {
        Debug.Log ("checking Damage.");

        if ( ship.Damage > 1 )
        {
            Debug.Log ("[Warning]: Your Ship is damaged. The Space Jump die roll will be " + Mathf.Round ((float)ship.Damage / 2) + " points harder.");
            modal_panel.question ("Schiff beschädigt", "Kapitän, durch den letzten Hyperraumsprung sind Schäden an Rumpf und Triebwerken aufgetreten. Die nächste Probe auf Raumsprungtechnik wird um "
                                                + Mathf.Round ((float)ship.Damage / 2) + " erschwert sein. Soll der Sprung trotzdem jetzt initialisiert werden?", approve_damage, cancel_validation);
            return;
        }

        Debug.Log ("check_damage () returning true.");
        damage_check = true;
    }
}