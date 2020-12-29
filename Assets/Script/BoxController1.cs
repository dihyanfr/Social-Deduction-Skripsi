using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoxController1 : MonoBehaviour
{
    public SphereCollider sc;
    public bool onPlayer;
    public bool onCollection;

    public GameController gc;

    public GameObject currentInteract;
    void Start()
    {
        sc = GetComponent<SphereCollider>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Collection")
        {
            onCollection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Collection")
        {
            onCollection = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player Triggered!");
            currentInteract = other.gameObject;
        }
    }

    [PunRPC]
    public void isTake(Vector3 _objectPos, Quaternion _objectRot)
    {
        sc.enabled = false;
        onPlayer = true;
        this.transform.parent = currentInteract.transform;
        this.transform.position = _objectPos;
        this.transform.rotation = _objectRot;
        this.GetComponent<Rigidbody>().isKinematic = true;
    }

    [PunRPC]
    public void Throw()
    {
        sc.enabled = true;
        onPlayer = false;
        this.transform.parent = null;
        this.GetComponent<Rigidbody>().isKinematic = false;

        if (onCollection)
        {
            this.GetComponent<SphereCollider>().enabled = false;
            gc.GetComponent<PhotonView>().RPC("taskDone", RpcTarget.AllBuffered, "Box");
        }
    }
}
