using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Role : MonoBehaviour, IPunObservable
{
    public int totalPlayer;
    public int totalMastermind;
    public int totalCrewmate;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string getRole()
    {
        totalPlayer++;
        int ran = Random.Range(0, 2);

        if (ran == 0)
        {
            if (totalMastermind != 1)
            {
                //totalMastermind++;
                GetComponent<PhotonView>().RPC("addMastermind", RpcTarget.AllBuffered);
                return "Mastermind";
            }
            else
            {
                //totalCrewmate++;
                GetComponent<PhotonView>().RPC("addCrewmate", RpcTarget.AllBuffered);
                return "Crewmate";
            }
        }
        else
        {
            //totalCrewmate++;
            GetComponent<PhotonView>().RPC("addCrewmate", RpcTarget.AllBuffered);
            return "Crewmate";
        }
    }

    [PunRPC]
    public void addMastermind()
    {
        totalMastermind++;
    }
    [PunRPC]
    public void addCrewmate()
    {
        totalCrewmate++;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.totalMastermind);
        }
        else
        {
            this.totalMastermind = (int)stream.ReceiveNext();
        }
    }
}
