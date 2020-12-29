using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollectionController : MonoBehaviour
{
    public int totalBox;
    [SerializeField] public GameController gc;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Box")
        {
            GetComponent<PhotonView>().RPC("addBox", RpcTarget.All);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Box")
        {
            GetComponent<PhotonView>().RPC("minusBox", RpcTarget.All);
        }
    }


    [PunRPC]
    private void addBox()
    {
        totalBox++;
    }

    [PunRPC]
    private void minusBox()
    {
        totalBox--;
    }

}
