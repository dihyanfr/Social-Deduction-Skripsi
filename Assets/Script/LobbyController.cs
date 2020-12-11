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

    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject CancelButton;
    [SerializeField] private byte RoomSize;
    [SerializeField] private string Version;

    public void SetNickname()
    {
        PhotonNetwork.ConnectUsingSettings(); // Player masuk ke koneksi game
        PhotonNetwork.GameVersion = this.Version + SceneManagerHelper.ActiveSceneBuildIndex;

        PhotonNetwork.LocalPlayer.NickName = NickName.text;
        InputUI.SetActive(false);
        ConnectedUI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Sinkronisasi semua player yang join
        StartButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom()
    {
        StartButton.SetActive(false);
        CancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(); // Player masuk room secara random
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // Jika gagal masuk room, bagian ini dijalankan
    {
        CreateRoom();
    }

    public void CreateRoom()
    {
        int RoomNumber = Random.Range(0, 9999);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = RoomSize };
        PhotonNetwork.CreateRoom(RoomNumber.ToString(), roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public void LeaveRoom()
    {
        StartButton.SetActive(true);
        CancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}
