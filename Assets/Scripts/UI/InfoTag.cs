using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoTag : MonoBehaviour {

    [SerializeField] private Text info_text; // info about object under cursor
    private Vector2 offset = new Vector2 (80, 60); // from cursor

    GameObject hit;

    // Update is called once per frame
    void Update ()
    {
        // get the object the mouse is pointing on
        hit = Camera.main.MouseObject (); // extension method, see script at top level

        if ( hit != null )
        {


            // get the reference to the script attached to the ingredient the mouse is pointing on
            if ( hit.tag == "Player Ship" )
            {
                // traverse the transform parent tree to find the parent with the script attached
                Ship script_reference = hit.transform.gameObject.GetComponent<Ship> ();
                Transform tf = this.transform;
                while (script_reference == null )              
                {
                    tf = tf.parent;
                    if ( tf.tag != "Player Ship" ) // some weird error made tf become the Canvas and from there it all went downhill
                        return;
                    script_reference = tf.GetComponent<Ship> ();
                }

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

    // enables text and sets it to mouse position
    void display ()
    {
        offset.x = this.GetComponent<Text> ().rectTransform.rect.width / 2 + 20;
        offset.y = this.GetComponent<Text> ().rectTransform.rect.height / 2;
        Vector2 modified_offset = new Vector2 (Mathf.Abs (offset.x), Mathf.Abs (offset.y ));
        if ( Input.mousePosition.x > Screen.width/2 )
            modified_offset.x *= -1;
        if ( Input.mousePosition.y > Screen.height/2 )
            modified_offset.y *= -1;

        transform.position = Input.mousePosition + new Vector3 (modified_offset.x, modified_offset.y, 0f);
        info_text.enabled = true;
    }

    void disableDisplay ()
    {
        info_text.enabled = false;
    }

}
