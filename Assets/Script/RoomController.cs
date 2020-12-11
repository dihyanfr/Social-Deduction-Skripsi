using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private string WaitingSceneName;
    [SerializeField] private string SceneName;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this); //
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        //StartScene();
        PhotonNetwork.LoadLevel(WaitingSceneName);
    }

    private void StartScene()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
            //PhotonNetwork.LoadLevel(SceneName);
        //}
    }
}
