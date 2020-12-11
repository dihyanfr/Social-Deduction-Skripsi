using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject Character;

    [SerializeField] public Text task1;
    [SerializeField] public Text task2;
    [SerializeField] public Text task3;
    [SerializeField] public Text task4;

    [SerializeField] private int roomSize;
    [SerializeField] private string[] playerList;

    private int totalHaveRole;
    private int totalMasterMind;
    private int totalHelper;


    private void Start()
    {
        InitiatePlayer();

        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        Debug.Log("Player List: " + PhotonNetwork.PlayerList[0]);
        
    }

    private void Update()
    {
        
    }

    public void InitiatePlayer()
    {
        PhotonNetwork.Instantiate(Character.name, SpawnPoint.position, Quaternion.identity);
    }

    public int getRoles()
    {
        int Ran = Random.Range(0, 2);

        if(Ran == 1)
        {
            if(totalMasterMind >= 1)
            {
                
                return 0;
            }
            else
            {
                totalMasterMind++;
                return 1;
            }
        }

        return 0;
    }
}
