using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class WaitingRoomController : MonoBehaviourPunCallbacks
{

    private PhotonView myPV;

    [SerializeField] private string Scene;
    [SerializeField] private string menuScene;

    private int playerCount;
    private int roomSize;

    [SerializeField] private int minPlayer;

    //UI
    [SerializeField] private Text playerCountText;
    [SerializeField] private Text timerText;

    private bool readyCountdown;
    private bool readyStart;
    private bool startGame;

    //Countdown
    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullGametime;


    void Start()
    {
        myPV = GetComponent<PhotonView>();
        fullGameTimer = maxFullGametime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;

        PlayerCountUpdate();
    }

    private void PlayerCountUpdate()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;

        playerCountText.text = playerCount + " / " + roomSize;

        if(playerCount == roomSize) //Jika room penuh maka langsung start
        {
            readyStart = true;
        }
        else if(playerCount >= minPlayer) // Jika room sudah melebihi batas minimal maka akan mulai berhitung mundur
        {
            readyCountdown = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();

        if (PhotonNetwork.IsMasterClient)
        {
            myPV.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);// Master client mengirim pesan untuk sinkron timer ke player lain
        }
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn)
    {
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if (timeIn < fullGameTimer)
        {
            fullGameTimer = timeIn;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        WaitingForOtherPlayer();
    }

   void WaitingForOtherPlayer()
    {
        if (playerCount == 1)
        {
            ResetTimer();
        }

        if (readyStart)
        {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyCountdown)
        {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerText.text = tempTimer;

        if(timerToStartGame <= 0f)
        {
            if (startGame) return;

            StartGame();
        }
    }

    void ResetTimer()
    {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGametime;
    }

    public void StartGame()
    {
        startGame = true;
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(Scene);
    }

    public void Cancel()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuScene);
    }
}
