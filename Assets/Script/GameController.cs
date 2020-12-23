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

    [SerializeField] public GameObject[] hackTask;
    [SerializeField] public string storageCode;
    [SerializeField] public int code1;
    [SerializeField] public int code2;
    [SerializeField] public int code3;

    [SerializeField] public GameObject[] doorTask;

    [SerializeField] private int roomSize;
    [SerializeField] private string[] playerList;

    [SerializeField] public float playerViewRadius = 13f;
    [SerializeField] public bool visionOnSabotage;

    private int totalHaveRole;
    private int totalMasterMind;
    private int totalHelper;


    private void Start()
    {
        code1 = Random.Range(0, 9);
        code2 = Random.Range(0, 9);
        code3 = Random.Range(0, 9);

        storageCode = code1.ToString() + code2.ToString() + code3.ToString();

        Debug.Log(storageCode);

        setHackCode();

        //InitiatePlayer();

        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        Debug.Log("Player List: " + PhotonNetwork.PlayerList[0]);

        
    }

    private void Update()
    {
        
    }

    //public void InitiatePlayer()
    //{
    //    PhotonNetwork.Instantiate(Character.name, SpawnPoint.position, Quaternion.identity);
    //}

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

    public void setHackCode()
    {

        for (int i = 0; i < hackTask.Length; i++)
        {
            int rnd = Random.Range(0, hackTask.Length);
            GameObject tempGO = hackTask[rnd];
            hackTask[rnd] = hackTask[i];
            hackTask[i] = tempGO;
        }

        Debug.Log("SET HACK CODE........");
        Debug.Log(hackTask[0]);

        for (int i = 0; i < 3; i++)
        {

        }
        hackTask[0].GetComponent<TypeGame>().setCode(code1.ToString(), 1);
        hackTask[1].GetComponent<TypeGame>().setCode(code2.ToString(), 2);
        hackTask[2].GetComponent<TypeGame>().setCode(code3.ToString(), 3);

        for (int i = 3; i < hackTask.Length; i++)
        {
            hackTask[i].GetComponentInParent<MiniGameController>().enabled = false;
            Destroy(hackTask[i].GetComponentInParent<MiniGameController>());
        }

        doorTask[0].GetComponent<CodeGame>().code = storageCode;

    }

}
