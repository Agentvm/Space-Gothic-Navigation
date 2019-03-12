using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

public class ModalValuePanel : MonoBehaviour
{

    // UI elements
    public Text header_text;
    public Button okay_button;
    public Button cancel_button;
    public InputField input_field;

    public GameObject modal_panel_object;

    private static ModalValuePanel modal_value_panel;
    public static ModalValuePanel Instance ()
    {
        if ( !modal_value_panel )
        {
            modal_value_panel = FindObjectOfType (typeof (ModalValuePanel)) as ModalValuePanel;
            if ( !modal_value_panel )
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        return modal_value_panel;
    }/*

        /// <summary>
    /// Buttons: yes/cancel/custom
    /// </summary>
    public void dieThrowDialog ( string header, string question_text, UnityAction yesEvent, UnityAction cancelEvent, UnityAction<string> dieEvent, int max_skill_level = 100 )
    {
        if ( modal_panel_object.activeSelf ) return;
        modal_panel_object.SetActive (true);
        //answer = (int)AnswerEnum.None;
        deactivateAllElements ();

        // re-activate respective buttons
        setupYesButton (yesEvent);
        //setupNoButton (noEvent);
        setupCancelButton (cancelEvent);
        setupInputPanel (dieEvent);
        //setupOptionsButton (optionsEvent);

        // set texts
        this.header_text.text = header;
        this.question_text.text = question_text;
        this.die_input_field.placeholder.GetComponent<Text> ().text = die_throws + "d" + die_size;
    }

    void setupYesButton ( UnityAction button_event )
    {
        yes_button.gameObject.SetActive (true);
        yes_button.onClick.RemoveAllListeners ();
        yes_button.onClick.AddListener (closePanel);
        if ( button_event != null )
            yes_button.onClick.AddListener (button_event);
        //yes_button.onClick.AddListener (answerYes); // testing
        
    }

    void setupCancelButton ( UnityAction button_event )
    {
        cancel_button.gameObject.SetActive (true);
        cancel_button.onClick.RemoveAllListeners ();
        cancel_button.onClick.AddListener (closePanel);
        if ( button_event != null )
            cancel_button.onClick.AddListener (button_event);
        //cancel_button.onClick.AddListener (answerCancel); // testing        
    }

    void setupInputPanel ( UnityAction<string> edit_event )
    {
        die_input_field.transform.parent.gameObject.SetActive (true);
        die_input_field.onEndEdit.RemoveAllListeners ();
        if ( edit_event != null )
            die_input_field.onEndEdit.AddListener (edit_event);
        //die_input_field.onEndEdit.AddListener (answerDieInput); // testing
    }

    /// <summary>
    /// Deactivate the clickable elements
    /// </summary>
    void deactivateAllElements ()
    {
        //yes_button.gameObject.SetActive (false);
        okay_button.gameObject.SetActive (false);
        //no_button.gameObject.SetActive (false);
        cancel_button.gameObject.SetActive (false);
        die_input_field.transform.parent.gameObject.SetActive (false);
        //options_button.gameObject.SetActive (false);
    }

    public void closePanel ()
    {
        Debug.Log ("Modal Panel " + header_text.text + " is closing now.");
        modal_panel_object.SetActive (false);
    }

    */
}