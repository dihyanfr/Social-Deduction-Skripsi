using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject currentInteract;
    private Transform objectPos;


    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentInteract)
        {
            objectPos = currentInteract.transform.Find("ObjectPos");
        }
    }

    public void grabBox()
    {
        Debug.Log("Grabbeb");
        this.transform.SetParent(currentInteract.transform);
        this.transform.position = objectPos.transform.position;
        this.transform.rotation = objectPos.transform.rotation;
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.CompareTag("Player"))
        {
            currentInteract = collision.gameObject;
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        //currentInteract = null;
    }
}
