using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PosnerParadigm : MonoBehaviour
{
    // Start is called before the first frame update

    public Manager m;                       // Link to Manager
    public GameObject StimulusRight;        // The Right cube
    public GameObject StimulusLeft;         // The Left cube
    public GameObject PosnerCanvas;         // The canvas
    public GameObject XRRig;                // The rig
    public string CurrentCondition;         // A string to compare with for hit/miss data
    public int CurrentPosnerTrial = 0;      // The current trial number of the posner paradigm
    public int NumberOfClicks = 10;         // The number of clicks during the paradigm
    public bool EventMarkerRun = false;     // Boolean for SendMarker 
    public bool HasRun = false;             // Boolean to reset the trial number of the posner paradigm
    public bool StimulusShown = false;      // Boolean for the stimulus
    public List<string> PosnerList;         // The list of order for the posner paradigm
    void Start()
    {
        m = GameObject.Find("Manager").GetComponent<Manager>();
        XRRig = GameObject.Find("XRRig");
        PosnerCanvas = XRRig.transform.Find("PosnerCanvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (m.NewTrial){
            CurrentPosnerTrial = 0;
            NumberOfClicks = 10;
            CurrentCondition = "";
            HasRun = false;}      // Whenever a new trial sets off, the HasRun is set to false

        if (!HasRun) {
            // Pseudo-randomize the list of turning direction
            for (int i = 0; i < (10 / 2); i++)
            {
                PosnerList.Add("Right");      // Right
                PosnerList.Add("Left");       // Left
                PosnerList = PosnerList.OrderBy(x => UnityEngine.Random.value).ToList();
            }
            HasRun = true;
        }

        if (CurrentPosnerTrial == 9 && NumberOfClicks == 0) {
            m.posnerDone = true;
            PosnerCanvas.SetActive(true);
        }
        else {
            PosnerCanvas.SetActive(false);
        }
        
        // Collect the clicks/answers
        if (CurrentPosnerTrial < 10 && NumberOfClicks > 0) {
            m.posnerDone = false;
            if (Input.GetMouseButtonDown(0) && CurrentCondition == "Left") {
                NumberOfClicks -= 1;
                sendMarker(m.LSLstatus + ";" + m.CurrentPosnerWall + ";" + "Hit");
            }
            if (Input.GetMouseButtonDown(1) && CurrentCondition == "Right") {
                NumberOfClicks -= 1;
                sendMarker(m.LSLstatus + ";" + m.CurrentPosnerWall + ";" + "Hit");
            }
            if (Input.GetMouseButtonDown(0) && CurrentCondition == "Right") {
                NumberOfClicks -= 1;
                sendMarker(m.LSLstatus + ";" + m.CurrentPosnerWall + ";" + "Miss");
            }
            if (Input.GetMouseButtonDown(1) && CurrentCondition == "Left") {
                NumberOfClicks -= 1;
                sendMarker(m.LSLstatus + ";" + m.CurrentPosnerWall + ";" + "Miss");
            }
        }
    }

    void OnTriggerEnter(Collider o){
        m.CurrentPosnerWall = o.transform.parent.name;
        if(PosnerList[CurrentPosnerTrial] == "Right" && !StimulusShown && o.transform.parent.name.StartsWith("Posner")) {
            StimulusRight.SetActive(true);
            CurrentCondition = "Right";
            sendMarker(m.LSLstatus + ";" + o.transform.parent.name + ";" + "Right");
            StartCoroutine(DisableStimulus());
            if(CurrentPosnerTrial < 9){
                CurrentPosnerTrial += 1;
                }
            StimulusShown = true;
        }

        if(PosnerList[CurrentPosnerTrial] == "Left" && !StimulusShown && o.transform.parent.name.StartsWith("Posner")) {
            StimulusLeft.SetActive(true);
            CurrentCondition = "Left";
            sendMarker(m.LSLstatus + ";" + o.transform.parent.name + ";" + "Left");
            StartCoroutine(DisableStimulus());
            if(CurrentPosnerTrial < 9){
                CurrentPosnerTrial += 1;
                }
            StimulusShown = true;
        }   
    }

    private void OnTriggerExit(Collider other){
        StimulusShown = false;
        EventMarkerRun = false;
    }

    // Function for sending marker
    void sendMarker(string StringToSend)
    {
        m.marker.Write(StringToSend);
        print(StringToSend);
    }

    // Function to disable the stimulus
    IEnumerator DisableStimulus() {
        yield return new WaitForSeconds(m.StimulusDisplayTime);
        StimulusLeft.SetActive(false);
        StimulusRight.SetActive(false);
    }
}
