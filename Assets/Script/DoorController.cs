using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public float sphereRadius;
    public float offset;

    Vector3 target;
    Vector3 normalPosition;

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3(this.transform.position.x, this.transform.position.y - offset, this.transform.position.z);
        normalPosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Move");
            float step = 2f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Move");
            float step = 2f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target, step);

        }

    }

    private void OnTriggerExit(Collider other)
    {
        while(this.transform.position != normalPosition)
        {
            float step = 2f * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, normalPosition, step);
        }
    }


}
