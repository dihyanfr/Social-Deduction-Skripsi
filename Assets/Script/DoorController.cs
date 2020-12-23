using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public float sphereRadius;
    public float offset;

    Vector3 target;
    Vector3 normalPosition;

    bool open = false;

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(this.transform.position.x, this.transform.position.y - offset, this.transform.position.z);
        normalPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            float step = 2f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
        else
        {
            float step = 2f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, normalPosition, step);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            open = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Move");
            //float step = 2f * Time.deltaTime; // calculate distance to move
            //transform.position = Vector3.MoveTowards(transform.position, target, step);
            //return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        open = false;
    }

    void moveDoor()
    {
        
    }


}
