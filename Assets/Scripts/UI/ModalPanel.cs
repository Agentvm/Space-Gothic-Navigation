using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

public class ModalPanel : MonoBehaviour
{

    // UI elements
    public Text header_text;
    public Text question_text;
    public Button yes_button;
    public Button okay_button;
    public Button no_button;
    public Button cancel_button;
    public InputField die_input_field;
    public Button options_button;

    public GameObject modal_panel_object;
    //public enum AnswerEnum { Yes, Okay, No, Cancel, DieInput, Options, None };
    //private int answer;

    //public int Answer
    //{
    //    get { return answer; }
    //}

    private static ModalPanel modal_panel;
    public static ModalPanel Instance ()
    {
        if ( !modal_panel )
        {
            modal_panel = FindObjectOfType (typeof (ModalPanel)) as ModalPanel;
            if ( !modal_panel )
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        return modal_panel;
    }

    /// <summary>
    /// Buttons: okay
    /// </summary>
    public void announcement ( string header, string announcement)
    {
        if ( modal_panel_object.activeSelf ) return;
        modal_panel_object.SetActive (true);
        //answer = (int)AnswerEnum.None;
        deactivateAllElements ();

        // setup buttons
        setupOkayButton ();

        // set texts
        this.header_text.text = header;
        this.question_text.text = announcement;
    }

    /// <summary>
    /// Buttons: Yes/No
    /// </summary>
    public void question ( string header, string question_text, UnityAction yesEvent, UnityAction noEvent )
    {
        if ( modal_panel_object.activeSelf ) return;
        modal_panel_object.SetActive (true);
        //answer = (int)AnswerEnum.None;
        deactivateAllElements ();

        // setup buttons
        setupYesButton (yesEvent);
        setupNoButton (noEvent);

        // set texts
        this.header_text.text = header;
        this.question_text.text = question_text;
    }

    /// <summary>
    /// Buttons: yes/cancel/custom
    /// </summary>
    public void dieThrowDialog ( string header, string question_text, UnityAction yesEvent, UnityAction cancelEvent, UnityAction<string> dieEvent, int die_throws = 1, int die_size = 100 )
    {
        if ( modal_panel_object.activeSelf ) return;
        modal_panel_object.SetActive (true);
        //answer = (int)AnswerEnum.None;
        deactivateAllElements ();

        // re-activate respective buttons
        setupYesButton (yesEvent);
        //setupNoButton (noEvent);
        setupCancelButton (cancelEvent);
        setupDiePanel (dieEvent);
        //setupOptionsButton (optionsEvent);

        // set texts
        this.header_text.text = header;
        this.question_text.text = question_text;
        this.die_input_field.placeholder.GetComponent<Text> ().text = die_throws + "d" + die_size;
    }

    /// <summary>
    /// Buttons: yes/cancel/custom/options
    /// </summary>
    public void dieThrowDialog ( string header, string question_text, UnityAction yesEvent, UnityAction cancelEvent, UnityAction<string> dieEvent, UnityAction optionsEvent, int die_throws = 1, int die_size = 100 )
    {
        if ( modal_panel_object.activeSelf ) return;
        modal_panel_object.SetActive (true);
        //answer = (int)AnswerEnum.None;
        deactivateAllElements ();

        // re-activate respective buttons
        setupYesButton (yesEvent);
        //setupNoButton (noEvent);
        setupCancelButton (cancelEvent);
        setupDiePanel (dieEvent);
        setupOptionsButton (optionsEvent);

        // set texts
        this.header_text.text = header;
        this.question_text.text = question_text;
        this.die_input_field.placeholder.GetComponent<Text> ().text = die_throws + "d" + die_size;
    }    

    public void testAllDialog ( string header, string question_text, UnityAction yesEvent, UnityAction noEvent,
                                                    UnityAction cancelEvent, UnityAction<string> dieEvent,
                                                    UnityAction optionsEvent, int die_throws = 1, int die_size = 100 )
    {
        if ( modal_panel_object.activeSelf ) return;
        modal_panel_object.SetActive (true);
        //answer = (int)AnswerEnum.None;
        deactivateAllElements ();

        // re-activate respective buttons
        setupYesButton (yesEvent);
        setupNoButton (noEvent);
        setupCancelButton (cancelEvent);
        setupDiePanel (dieEvent);
        setupOptionsButton (optionsEvent);

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

    void setupOkayButton ()
    {
        okay_button.gameObject.SetActive (true);
        okay_button.onClick.RemoveAllListeners ();
        //okay_button.onClick.AddListener (answerOkay); // testing
        okay_button.onClick.AddListener (closePanel);
    }

    void setupNoButton ( UnityAction button_event )
    {
        no_button.gameObject.SetActive (true);
        no_button.onClick.RemoveAllListeners ();
        no_button.onClick.AddListener (closePanel);
        if ( button_event != null )
            no_button.onClick.AddListener (button_event);
        //no_button.onClick.AddListener (answerNo); // testing
        
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

    void setupOptionsButton ( UnityAction button_event )
    {
        options_button.gameObject.SetActive (true);
        options_button.onClick.RemoveAllListeners ();
        options_button.onClick.AddListener (closePanel);
        if ( button_event != null )
            options_button.onClick.AddListener (button_event);
        //options_button.onClick.AddListener (answerOptions); // testing
        
    }

    void setupDiePanel ( UnityAction<string> edit_event )
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
        yes_button.gameObject.SetActive (false);
        okay_button.gameObject.SetActive (false);
        no_button.gameObject.SetActive (false);
        cancel_button.gameObject.SetActive (false);
        die_input_field.transform.parent.gameObject.SetActive (false);
        options_button.gameObject.SetActive (false);
    }

    public void closePanel ()
    {
        Debug.Log ("Modal Panel " + header_text.text + " is closing now.");
        modal_panel_object.SetActive (false);
    }

    // Testing

    /* This is the first Subroutine that worked when called in closePanel like so:
     * StartCoroutine (CloseWithDelay);
     */
    //public IEnumerator CloseWithDelay ()
    //{
    //    yield return new WaitForSeconds (.1f);
    //    modal_panel_object.SetActive (false);
    //}


    /*
     In GameVariables:
     
        ModalPanel.Instance ().testCoroutineDialog ("Die Throw", "Do you want the computer to throw the dice, or fill the result in yourself ?", yes_event, cancel_event, cancel_event, dieEvent, options_event, 1, 10);
        int answer = ModalPanel.Instance ().Answer;
        print ("got returned answer: " + (ModalPanel.AnswerEnum) answer + " / " + answer);
        print ("ModalPanel.Instance ().Answer: " + (ModalPanel.AnswerEnum)ModalPanel.Instance ().Answer + " / " + ModalPanel.Instance ().Answer);
     */


    //// Answers
    //void answerYes () { answer = (int)AnswerEnum.Yes; }
    //void answerOkay () { answer = (int)AnswerEnum.Okay; }
    //void answerNo () { answer = (int)AnswerEnum.No; }
    //void answerCancel () { answer = (int)AnswerEnum.Cancel; }
    //void answerOptions () { answer = (int)AnswerEnum.Options; }
    //void answerDieInput ( string obsolete ) { answer = (int)AnswerEnum.DieInput; }

    //public int testCoroutineDialog ( string header, string question, UnityAction yesEvent, UnityAction noEvent,
    //                                                UnityAction cancelEvent, UnityAction<string> dieEvent,
    //                                                UnityAction optionsEvent, int die_throws = 1, int die_size = 100 )
    //{
    //    modal_panel_object.SetActive (true);
    //    answer = (int)AnswerEnum.None;
    //    deactivateAllElements ();

    //    // re-activate respective buttons
    //    setupYesButton (yesEvent);
    //    setupNoButton (noEvent);
    //    setupCancelButton (cancelEvent);
    //    setupDiePanel (dieEvent);
    //    setupOptionsButton (optionsEvent);

    //    // set texts
    //    this.header_text.text = header;
    //    this.question_text.text = question;
    //    this.die_input_field.placeholder.GetComponent<Text> ().text = die_throws + "d" + die_size;

    //    StartCoroutine (QuestionAsked ());
    //    return answer;
    //}


    //private IEnumerator QuestionAsked ()
    //{
    //    // do stuff here, show win screen, etc.

    //    // just a simple time delay as an example
    //    //yield return new WaitForSeconds (2.5f);

    //    // wait for player to press space
    //    yield return waitForUserChoice (); // wait for this function to return

    //    // do other stuff after key press
    //    print ("Oh! Something happened. This only prints when closePanel () is disabled.");
    //}

    //private IEnumerator waitForUserChoice ()
    //{
    //    bool done = false;
    //    while ( !done ) // essentially a "while true", but with a bool to break out naturally
    //    {
    //        print ("(AnswerEnum) Answer != AnswerEnum.None: " + ((AnswerEnum)Answer != AnswerEnum.None));
    //        print ("Answer = " + (AnswerEnum)Answer);

    //        if ( (AnswerEnum)Answer != AnswerEnum.None )
    //        {
    //            done = true; // breaks the loop
    //        }
    //        yield return null; // wait until next frame, then continue execution from here (loop continues)
    //    }

    //    // now this function returns
    //}


}
