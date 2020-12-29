using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DoorController : MonoBehaviour, IPunObservable
{
    public GameObject door;
    public float sphereRadius;
    public float offset;

    public AudioSource audioSource;
    public AudioClip doorSFX;

    bool playSFX;

    Vector3 target;
    Vector3 normalPosition;

    bool open = false;

    public bool isLocked;

    void Start()
    {
        target = new Vector3(door.transform.position.x, door.transform.position.y - offset, door.transform.position.z);
        normalPosition = new Vector3(door.transform.position.x, door.transform.position.y, door.transform.position.z);
    }

    void Update()
    {
        if (open && !isLocked)
        {
            if (!playSFX)
            {
                //audioSource.PlayOneShot(doorSFX);
                playSFX = true; 
            }

            float step = 2f * Time.deltaTime;
            door.transform.position = Vector3.MoveTowards(door.transform.position, target, step);

            if (door.transform.position == target)
            {
                //playSFX = false;
            }

            if (door.transform.position.y <= -1)
            {
                door.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
        else
        {
            float step = 2f * Time.deltaTime; 
            door.transform.position = Vector3.MoveTowards(door.transform.position, normalPosition, step);

            if (door.transform.position == normalPosition)
            {
                playSFX = false;
            }

            if (door.transform.position.y >= -1)
            {
                door.gameObject.layer = LayerMask.NameToLayer("Obstacle");
            }
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
            //audioSource.PlayOneShot(doorSFX);
            open = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //audioSource.PlayOneShot(doorSFX);
        open = false;
    }

    [PunRPC]
    public void lockDoor()
    {
        isLocked = true;
    }

    [PunRPC]
    public void unlockDoor()
    {
        isLocked = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.isLocked);
        }
        else
        {
            this.isLocked= (bool)stream.ReceiveNext();
        }
    }
}
