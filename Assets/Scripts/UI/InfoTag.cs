using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoTag : MonoBehaviour {

    [SerializeField] private Text info_text; // info about object under cursor

    GameObject hit;

    void OnEnable ()
    {
        disableDisplay ();
    }

    // Update is called once per frame
    void Update ()
    {
        // get the object the mouse is pointing on
        hit = Camera.main.MouseObject (); // extension method, see script at top level

        if ( hit != null)
        {
            // get the reference to the script attached to the ingredient the mouse is pointing on
            if ( hit.tag == "Player Ship" )
            {
                // traverse the transform parent tree to find the parent with the script attached
                Ship script_reference = Ship.Instance ();
                //Transform tf = this.transform;
                //while ( script_reference == null )
                //{
                //    tf = tf.parent;
                //    if ( tf.tag != "Player Ship" ) // some weird error made tf become the Canvas and from there it all went downhill
                //        return;
                //    script_reference = tf.GetComponent<Ship> ();
                //}

                info_text.text = "Player Ship"
                                + "\nName: " + script_reference.name
                                + "\nPosition: " + script_reference.transform.position
                                + "\nDescription: " + script_reference.Description;
                display ();
            }
            else if ( hit.transform.tag == "Planet" )
            {
                // get the reference to the script attached to the ingredient the mouse is pointing on
                Planet script_reference = hit.transform.gameObject.GetComponent<Planet> ();

                // fill text
                info_text.text = "Planet, Class " + script_reference.Type
                                + "\nName: " + script_reference.name
                                + "\nPosition: " + script_reference.transform.position
                                + "\nDescription: " + script_reference.Description;

                display ();
            }
            else if ( hit.transform.tag == "Star System" )
            {
                // get the reference to the script attached to the ingredient the mouse is pointing on
                StarSystem script_reference = hit.transform.gameObject.GetComponent<StarSystem> ();

                string planets_text = "\n";
                foreach (Planet planet in script_reference.Planets )
                {
                    planets_text += "\t" + planet.name + " (" + planet.Type + ")\n";
                }

                if (planets_text == "\n")
                    planets_text = "Unexplored";


                // fill text
                info_text.text = "Star System, Class " + script_reference.Type
                                + "\nName: " + script_reference.name
                                + "\nPosition: " + script_reference.transform.position
                                + "\nPlanets: " + planets_text
                                + "\nDescription: " + script_reference.Description;

                display ();
            }
            else if ( hit.transform.tag == "Galaxy" )
            {
                // get the reference to the script attached to the ingredient the mouse is pointing on
                Galaxy script_reference = hit.transform.gameObject.GetComponent<Galaxy> ();

                // fill text
                info_text.text = "Galaxy"
                                + "\nName: " + script_reference.name
                                + "\nPosition: " + script_reference.transform.position
                                + "\nDescription: " + script_reference.Description;

                display ();
            }
            // raycast didn't hit a valid object
            else disableDisplay ();

        }
        // raycast didn't hit anything
        else disableDisplay ();

    }

    // enables text and sets it to mouse position, applying offset
    void display ()
    {
        // compute offset (based on empirical values)
        float x_offset = info_text.rectTransform.rect.width /2 + 20;
        float y_offset = info_text.rectTransform.rect.height /2;

        // ensure the text panel doesn't flow out of screen
        Vector3 screen_space = Camera.main.WorldToViewportPoint (hit.transform.position);
        if ( screen_space.x > 0.5f )
            x_offset *= -1;
        if ( screen_space.y > 0.5f )
            y_offset *= -1;

        // apply changes to text transform
        transform.position = Input.mousePosition + new Vector3 (x_offset, y_offset, 0f);
        info_text.enabled = true;
    }

    void disableDisplay ()
    {
        info_text.text = "";
        info_text.enabled = false;
    }

}
