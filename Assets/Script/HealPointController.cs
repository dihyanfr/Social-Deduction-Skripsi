using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealPointController : MonoBehaviour
{
    public ParticleSystem healingRing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            healingRing.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Player Healing");
            other.GetComponentInParent<PhotonView>().RPC("getHealed", RpcTarget.AllBuffered);
           

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            healingRing.Stop();
        }
    }
}
