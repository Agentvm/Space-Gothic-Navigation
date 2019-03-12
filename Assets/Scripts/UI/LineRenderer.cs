using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderer : MonoBehaviour
{
    // When added to an object, it can be used to draw lines on the basis of Vectors
    private List<Vector3> lines = new List<Vector3>(); // lines to draw
    private List<int> white_lines = new List<int>(); // don't draw these indices
    private bool currently_rendering = false;
    private Color color = new Color (1, 0, 0, 0.7f );

    [SerializeField]/*static*/ Material lineMaterial;
    /*static*/ void CreateLineMaterial ()
    {
        if ( !lineMaterial )
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            //Shader shader = Shader.Find("Lines/Colored Blended");
            //Shader shader = Shader.Find("Unlit/Texture");
            //"Standard", "Unlit/Texture", "Legacy Shaders/Diffuse"
            lineMaterial = new Material (shader);
            
            
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            
            // Try to put it behind UI
            lineMaterial.SetInt ("_RenderQueue", (int)UnityEngine.Rendering.RenderQueue.Background);
            lineMaterial.SetInt ("_Queue", (int)UnityEngine.Rendering.RenderQueue.Background);
            lineMaterial.renderQueue = 1000;

            lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt ("_ZWrite", 1);
            

            // no longer supported
            /* lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
             "SubShader {Tags { \"RenderType\"=\"Opaque\" } Pass { " +
             "ZWrite On ZTest LEqual Cull Off Fog { Mode Off } " +
             "BindChannels {" +
             "Bind \"vertex\", vertex Bind \"color\", color }" +
             "} } }");*/
        }
    }

    void Awake ()
    {
        CreateLineMaterial ();
    }

    public void set_default_color (Color col)
    {
        color = col;
    }

    public void stop_rendering ()
    {
        currently_rendering = false;
    }

    public void draw_new_line (Vector3 point1, Vector3 point2 )
    {
        lines.Clear ();
        lines.Add (point1);
        lines.Add (point2);
        currently_rendering = true;
    }

    public void draw_new_line ( List<Vector3> points )
    {
        lines.Clear ();
        lines.AddRange (points);
        currently_rendering = true;
    }

    public void draw_new_line ( Vector3[] points )
    {
        lines.Clear ();
        lines.AddRange (points);
        currently_rendering = true;
    }

    public void draw_additional_line ( Vector3 point1, Vector3 point2 )
    {
        white_lines.Add (lines.Count);
        lines.Add (point1);
        lines.Add (point2);
        currently_rendering = true;
    }

    public void draw_additional_line ( List<Vector3> points )
    {
        white_lines.Add (lines.Count);
        lines.AddRange (points);
        currently_rendering = true;
    }

    public void draw_additional_line ( Vector3[] points )
    {
        white_lines.Add (lines.Count);
        lines.AddRange (points);
        currently_rendering = true;
    }

    public void clear_lines ()
    {
        lines.Clear ();
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject ()
    {
        if ( !currently_rendering )
            return;

        CreateLineMaterial ();
        // Apply the line material
        lineMaterial.SetPass (0);

        GL.PushMatrix ();
        // Set transformation matrix for drawing to
        // match our transform
        //GL.MultMatrix (transform.localToWorldMatrix);

        // Draw lines
        GL.Begin (GL.LINES);
        for ( int i = 1; i < lines.Count; ++i )
        {
            if ( white_lines.Contains (i) )
                continue;

            // Vertex color red (0.8f)
            GL.Color (color);

            // draw the next two points contained in lines and corrected by the current position of the spaceship this Renderer is attached to
            Vector3 point1 = /*this.transform.position + */lines[i - 1];
            Vector3 point2 = /*this.transform.position + */lines[i];

            GL.Vertex3 (point1.x, point1.y, point1.z);
            GL.Vertex3 (point2.x, point2.y, point2.z);

            // One vertex at transform position
            //GL.Vertex3 (lines[i-1].x , lines[i-1].y, lines[i-1].z );
            //GL.Vertex3 (lines[i].x, lines[i].y, lines[i].z );
            
            //Debug.Log ("Plotting Line from: " + lines[i - 1] + " to: " + lines[i] + "." );
        }

        // Add Ship's position marker
        /*GL.Color (new Color (0, 0, 1));
        Vector3 marker_bottom = this.transform.position - new Vector3 (0, 0, 5);
        Vector3 marker_top = this.transform.position + new Vector3 (0, 0, 5);
        GL.Vertex3 (marker_bottom.x, marker_bottom.y, marker_bottom.z);
        GL.Vertex3 (marker_top.x, marker_top.y, marker_top.z);*/

        GL.End ();
        GL.PopMatrix ();
    }
}
