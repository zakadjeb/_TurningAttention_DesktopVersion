using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingSpaces : MonoBehaviour
{
    // Start is called before the first frame update

    public Manager m;
    public string ObjectName;

    void Start()
    {
        m = GameObject.Find("Manager").GetComponent<Manager>();        
    }

    // Update is called once per frame
    void Update()
    {
        if(!m.ExperimentDone) {    
            // Activate the gameobject if it's summoned by the manager
            if(gameObject.name == m.TurningState[m.CurrentTrial]) {
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(true);
                }
            }
            else{
                foreach (Transform child in transform) {
                    child.gameObject.SetActive(false);
                }
            }

            // Position the space according to the TurningDirection list in the manager
            if(gameObject.name == m.TurningState[m.CurrentTrial] && m.TurningDirection[m.CurrentTrial] == "TurningRight") {
                transform.position = new Vector3(0,0,0);
                transform.rotation = Quaternion.Euler(0,0,0);
            }

            if(gameObject.name == m.TurningState[m.CurrentTrial] && m.TurningDirection[m.CurrentTrial] == "TurningLeft") {
                transform.position = new Vector3(-5f,2.9f,0);
                transform.rotation = Quaternion.Euler(0,0,-180f);
            }
        }
    }
}
