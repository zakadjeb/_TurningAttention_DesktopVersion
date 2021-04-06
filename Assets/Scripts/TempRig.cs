using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempRig : MonoBehaviour
{
    private float MovementY;
    public float Turning;
    [SerializeField] [Range(0f, 10f)] public float MovingSpeed;
    [SerializeField] [Range(0f, 120f)] public float TurningSpeed;
    public Manager m;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(-2.5f,0f,-2.8f);
        Turning = 180f;
        TurningSpeed = 100f;
        MovingSpeed = 0.5f;

        m = GameObject.Find("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        MovementY = Input.GetAxis ("Vertical");

        // Reset position of the Rig
        if(m.NewTrial){
            Turning = 180f;
            transform.position = new Vector3(-2.5f,0,-2.8f);
        }

        if (!m.NewTrial && MovementY != 0) 
        {
            //transform.position += transform.right * MovementX * Time.deltaTime * MovingSpeed;
            transform.position += transform.forward * MovementY * Time.deltaTime * MovingSpeed;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            Turning += Time.deltaTime * TurningSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            Turning -= Time.deltaTime * TurningSpeed;
        }
        transform.eulerAngles = new Vector3(0.0f, Turning, 0.0f);
    }
}
