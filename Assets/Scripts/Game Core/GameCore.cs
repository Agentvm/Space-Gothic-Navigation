﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCore : MonoBehaviour
{

    public static GameCore instance = null; // Static instance of GameManager which allows it to be accessed by any other script.
    private GameVariables game_variables;   // Store a reference to our game variables.

    public GameVariables GameVariables // call it like: GameCore.instance.GameVariables
    {
        get { return game_variables; }
    }

    // Awake is always called before any Start functions
    void Awake ()
    {
        // check there is only one instance of this and that it is not destroyed on load
        if ( instance == null )
            instance = this;
        else if ( instance != this )
            Destroy (gameObject);
        DontDestroyOnLoad (gameObject);

        // Initialize game variables
        game_variables = this.GetComponent<GameVariables> ();
        game_variables.initialize ();
    }

    static public int roll_dice ( int times = 1, int faces = 100 )
    {
        int result = 0;

        for (int i = 0; i < times; i++ )
        {
            result += (int)Mathf.Round (Random.Range (1, faces));
        }

        return result;
    }

    static public void focus_main_camera ( Transform tf )
    {
        OrbitCameraZoom camera_script = Camera.main.GetComponent<OrbitCameraZoom> ();
        camera_script.Target.transform.position = tf.position;
        camera_script.Target.transform.rotation = tf.rotation;
        camera_script.Init ();
    }

}
