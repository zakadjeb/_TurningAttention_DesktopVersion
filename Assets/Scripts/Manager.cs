using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using UnityEngine.UI;
using Assets.LSL4Unity.Scripts;

public class Manager : MonoBehaviour
{
    [Header("Timer-settings")]
    public int TimeInDark = 3;              // Time in the dark
    public int DarkSD = 1;                  // Inter trial variation in the dark
    public float DarkTimer = 0f;            // Current time in the dark
    public bool isDark;                     // Currently dark or not
    public bool posnerDone = false;         // Currently done with task or not
    public List<int> TimeInDarkList;        // The list of order for all darktimes

    [Header("Game-settings")]
    public int TotalNumberOfTrials = 150;   // At least 50 trials per trial for ERSPs
    public int CurrentTrial = 0;            // Current trial number
    public bool TrialDone = false;          // Turns true whenever a trial is done
    public bool ExperimentDone = false;     // Tracks whether experiment is done
    public GameObject BlackSphere;          // The black sphere around head of the rig
    public GameObject XRRig;                // The VR rig
    public GameObject TwentyDegs;           // 20 Degs space
    public GameObject FortyfiveDegs;        // 45 Degs space
    public GameObject NinetyDegs;           // 90 Degs space
    public GameObject EndText;              // The end text
    public List<string> TurningState;       // The list of order for all conditions
    public List<string> TurningDirection;   // The list of order for all directions

    [Header("Posner-settings")]
    public float StimulusDisplayTime = 0.1f;// For how long should the stimulus be on display
    public float PosnerBalance = 0.8f;      // How many of the stimuli should pop up according to the turning direction
    public bool NewTrial;                   // True whenever a new trial sets off

    [Header("LSL String")]
    public LSLMarkerStream marker;          // Creating the marker
    public string LSLstatus;                // Current state of the experiment, string to send with LSL marker
    public string CurrentCondition;         // Current condition
    public string CurrentDirection;         // Current direction
    public string CurrentLight;             // Current light conditions (Lights on or off)
    public string CurrentPosnerWall;        // Which Posner-Wall triggered the stimulus
    public string CurrentStimulus;          // Current stimulus (Right or left)
    public string CurrentHitOrMiss;         // Whether successful in clicking
    public bool EventMarkerRun;             // Track whether marker has been sent or not

    // Start is called before the first frame update
    void Start()
    {
        // Getting LSL stream
        marker = FindObjectOfType<LSLMarkerStream>();

        // Find gameobjects
        XRRig = GameObject.Find("XRRig");
        BlackSphere = XRRig.transform.Find("BlackSphere").gameObject;
        TwentyDegs = GameObject.Find("TwentyDegs");
        FortyfiveDegs = GameObject.Find("FortyfiveDegs");
        NinetyDegs = GameObject.Find("NinetyDegs");
        EndText = XRRig.transform.Find("EndCanvas").gameObject;

        // Pseudo-randomize the list of conditions
        for (int i = 0; i < (TotalNumberOfTrials / 3); i++)
        {
            TurningState.Add("TwentyDegs");     // 20 Degs
            TurningState.Add("FortyfiveDegs");  // 45 Degs
            TurningState.Add("NinetyDegs");     // 90 Degs
            TurningState = TurningState.OrderBy(x => UnityEngine.Random.value).ToList();
        }

        // Pseudo-randomize the list of turning direction
        for (int i = 0; i < (TotalNumberOfTrials / 2); i++)
        {
            TurningDirection.Add("TurningRight");      // Right
            TurningDirection.Add("TurningLeft");       // Left
            TurningDirection = TurningDirection.OrderBy(x => UnityEngine.Random.value).ToList();
        }

        // Pseudo-randomize the list of time in dark
        System.Random rnd = new System.Random();
        for (int i = 0; i < TotalNumberOfTrials; i++)
        {
            TimeInDarkList.Add(rnd.Next(TimeInDark-DarkSD, TimeInDark+(DarkSD*2)));
        }

        // Start settings
        DarkTimer = TimeInDarkList[CurrentTrial];
        BlackSphere.SetActive(true);
        isDark = true;
        CurrentLight = "LightsOff";
        EventMarkerRun = false;
    }

    // Update is called once per frame
    void Update()
    {        
        // Checking first if the experiment is done
        if(CurrentTrial == TotalNumberOfTrials){
            EndText.SetActive(true);
            ExperimentDone = true;
            marker.Write("ExperimentDone");
            print("ExperimentDone");
        }

        // Setting the current situation for LSL string
        if(!ExperimentDone){CurrentCondition = TurningState[CurrentTrial];}
        if(!ExperimentDone){CurrentDirection = TurningDirection[CurrentTrial];}
        if(isDark){CurrentLight="LightsOff";}

        // Timer-loop
        // Countdown for the time in the dark
        if(DarkTimer >= 0 && isDark)  {
            NewTrial = false;
            DarkTimer -= Time.deltaTime;
            BlackSphere.SetActive(true);
            sendMarker();
        }

        // Once the countdown hits zero or less
        if(DarkTimer <= 0 && isDark) {
            isDark = false;
            BlackSphere.SetActive(false);
            CurrentLight = "LightsOn";
            EventMarkerRun = false;
            sendMarker();
        }

        // When the trial is over
        if(!ExperimentDone && !isDark && posnerDone && Input.GetKeyDown(KeyCode.Space)) {
            CurrentTrial += 1;
            NewTrial = true;
            DarkTimer = TimeInDarkList[CurrentTrial];
            isDark = true;
        }

    }

    // Function for sending marker
    void sendMarker()
    {
        if(!EventMarkerRun) {
            LSLstatus = "#" + CurrentTrial.ToString() + ";" + CurrentCondition + ";" + CurrentDirection + ";" + CurrentLight;
            marker.Write(LSLstatus);
            print(LSLstatus);
            EventMarkerRun = true;
        }
    }
}
