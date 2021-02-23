using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject InputUI;
    [SerializeField] private GameObject ConnectedUI;

    [SerializeField] private InputField NickName;
    
    [SerializeField] private Text name;
    [SerializeField] private Text feedback;

    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject CancelButton;
    [SerializeField] private InputField roomid;
    [SerializeField] private byte RoomSize;
    [SerializeField] private string Version;
    [SerializeField] private GameObject button;

    private void Start()
    {

    }

    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            feedback.text = "Please wait...";
            PhotonNetwork.ConnectUsingSettings(); // Player masuk ke koneksi game
            PhotonNetwork.GameVersion = this.Version + SceneManagerHelper.ActiveSceneBuildIndex;
        }
    }

    public void SetNickname()
    {
        
        name.text = NickName.text;
        feedback.text = "";

        InputUI.SetActive(false);
        ConnectedUI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Sinkronisasi semua player yang join
        StartButton.SetActive(true);
        feedback.text = "Connected to server. Choose room";
        //PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom()
    {
        PhotonNetwork.ConnectUsingSettings(); // Player masuk ke koneksi game
        PhotonNetwork.GameVersion = this.Version + SceneManagerHelper.ActiveSceneBuildIndex;

        PhotonNetwork.LocalPlayer.NickName = NickName.text;

        PhotonNetwork.JoinRandomRoom(); // Player masuk room secara random
        feedback.text = "Wait for connection";
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // Jika gagal masuk room, bagian ini dijalankan
    {
        feedback.text = "No room found";
        //CreateRoom();
    }

    public void CreateRoom()
    {
        feedback.text = "Creating Room";
        int RoomNumber = Random.Range(0, 9999);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = RoomSize };
        PhotonNetwork.CreateRoom(RoomNumber.ToString(), roomOps);


        

        PhotonNetwork.LocalPlayer.NickName = NickName.text;
        PhotonNetwork.JoinRoom(RoomNumber.ToString());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //CreateRoom();
    }

    public void findRoom()
    {
        feedback.text = "Finding Room";
        PhotonNetwork.JoinRoom(roomid.text);
        PhotonNetwork.ConnectUsingSettings(); // Player masuk ke koneksi game
        PhotonNetwork.GameVersion = this.Version + SceneManagerHelper.ActiveSceneBuildIndex;

        PhotonNetwork.LocalPlayer.NickName = NickName.text;
    }

    public void LeaveRoom()
    {
        feedback.text = "Cancelling";
        button.SetActive(false);
        StartButton.SetActive(true);
        CancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
    //test
}
