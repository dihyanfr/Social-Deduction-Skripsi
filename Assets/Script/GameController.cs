﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IPunObservable
{
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject Character;

    //[SerializeField] public Text task1;
    //[SerializeField] public Text task2;
    //[SerializeField] public Text task3;
    //[SerializeField] public Text task4;

    [SerializeField] public GameObject[] hackTask;
    [SerializeField] public string storageCode;
    [SerializeField] public int code1;
    [SerializeField] public int code2;
    [SerializeField] public int code3;
    [SerializeField] public int task1;
    [SerializeField] public int task2;
    [SerializeField] public int task3;
    private bool isSet = false;

    public bool isRandom = false;

    [SerializeField] public GameObject[] doorTask;

    [SerializeField] private int roomSize;
    [SerializeField] private string[] playerList;

    
    //Roles
    private int totalHaveRole;
    private int totalMasterMind;
    private int totalHelper;

    //MemoryTask
    [Header("Memory Task Settings")]
    public Text memoryGameText;
    public int memoryGame;

    //BoxTask
    [Header("Box Task Settings")]
    [SerializeField] public GameObject collectionPoint;
    [SerializeField] public int totalBox;
    [SerializeField] public Text boxText;

    //Door Sabotage
    [Header("Door Sabotage Settings")]
    [SerializeField] public bool isDoorLocked;
    [SerializeField] public float lockedTime = 10f;
    float tempLockedTime;
    [SerializeField] public Image lockedDoor;
    [SerializeField] public Image lockedDoorBar;
    [SerializeField] public GameObject[] bigDoor;

    //Vision Sabotage
    [Header("Vision Sabotage Settings")]
    [SerializeField] public bool isVisionSabotage;
    [SerializeField] public GameObject[] lightStage;
    [SerializeField] public float playerViewRadius = 13f;
    [SerializeField] public float radiusNormal = 13f;
    [SerializeField] public float radiusOnSabotage = 1.5f;

    private void Start()
    {
        bigDoor = GameObject.FindGameObjectsWithTag("DoorDetection");
        lightStage = GameObject.FindGameObjectsWithTag("Light");

        code1 = Random.Range(0, 9);
        code2 = Random.Range(0, 9);
        code3 = Random.Range(0, 9);

        task1 = Random.Range(0, hackTask.Length);
        task2 = Random.Range(0, hackTask.Length);
        task3 = Random.Range(0, hackTask.Length);

        if (task2 == task1)
        {
            while (task2 == task1)
            {
                task2 = Random.Range(0, hackTask.Length);
            }
        }

        if (task3 == task2 || task3 == task1)
        {
            while (task3 == task2 || task3 == task1)
            {
                task3 = Random.Range(0, hackTask.Length);
            }
        }

        storageCode = code1.ToString() + code2.ToString() + code3.ToString();

        //GetComponentInParent<PhotonView>().RPC("setHackCode", RpcTarget.All, code1,code2,code3);

        //setHackCode();

        //InitiatePlayer();

        //roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        //Debug.Log("Player List: " + PhotonNetwork.PlayerList[0]);
        
    }

    private void Update()
    {
        if (!isSet)
        {
            setHackCode(code1, code2, code3);
            //GetComponentInParent<PhotonView>().RPC("setHackCode", RpcTarget.AllBuffered, code1, code2, code3);
        }

        if (isDoorLocked)
        {
            if(tempLockedTime <= lockedTime)
            {
                tempLockedTime += Time.deltaTime;

                lockedDoorBar.fillAmount = tempLockedTime / lockedTime;
                
                Debug.Log("tempLockedTime = " + tempLockedTime);
            }
            else
            {
                isDoorLocked = false;
                tempLockedTime = 0;
                GetComponent<PhotonView>().RPC("unlockDoor", RpcTarget.AllBuffered);
            }
        }



        memoryGameText.text = memoryGame.ToString();
        boxText.text = totalBox.ToString();
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

    [PunRPC]
    public void setHackCode(int c1, int c2,int c3)
    {
        hackTask[task1].GetComponent<TypeGame>().setCode(code1.ToString(), 1);
        hackTask[task2].GetComponent<TypeGame>().setCode(code2.ToString(), 2);
        hackTask[task3].GetComponent<TypeGame>().setCode(code3.ToString(), 3);

        for (int i = 0; i < hackTask.Length; i++)
        {
            if (i != task1 && i != task2 && i != task3)
            {
                //hackTask[i].GetComponentInParent<MiniGameController>().enabled = false;
                Destroy(hackTask[i].GetComponentInParent<MiniGameController>());
            }
            
        }
        isSet = true;
        //doorTask[0].GetComponent<CodeGame>().code = storageCode;

    }

    [PunRPC]
    public void taskDone(string type)
    {
        if(type == "Memory Game")
        {
            memoryGame++;
        }
        if (type == "Box")
        {
            totalBox++;
        }

        memoryGameText.text = memoryGame.ToString();
        boxText.text = totalBox.ToString();
    }

    [PunRPC]
    public void shuffleTask()
    {
        for (int i = 0; i < hackTask.Length; i++)
        {
            int rnd = Random.Range(0, hackTask.Length);
            GameObject tempGO = hackTask[rnd];
            hackTask[rnd] = hackTask[i];
            hackTask[i] = tempGO;
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

    [PunRPC]
    public void lockedDoorUI()
    {
        lockedDoor.gameObject.SetActive(true);
        isDoorLocked = true;
    }

    [PunRPC]
    public void unlockDoor()
    {
        
        lockedDoor.gameObject.SetActive(false);

        for (int i = 0; i < bigDoor.Length; i++)
        {
            Debug.Log(bigDoor[i]);
            bigDoor[i].GetComponent<PhotonView>().RPC("unlockDoor", RpcTarget.AllBuffered);
        }
    }

    public void visionSabotage()
    {
        GetComponent<PhotonView>().RPC("reduceVision", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void reduceVision()
    {
        for(int i = 0; i < lightStage.Length; i++)
        {
            lightStage[i].GetComponent<Light>().intensity = 0.1f;
            lightStage[i].GetComponent<Light>().shadowStrength = 0f;
        }

        playerViewRadius = radiusOnSabotage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.code1);
            stream.SendNext(this.code2);
            stream.SendNext(this.code3);
            stream.SendNext(this.task1);
            stream.SendNext(this.task2);
            stream.SendNext(this.task3);
        }
        else
        {
            this.code1 = (int)stream.ReceiveNext();
            this.code2 = (int)stream.ReceiveNext();
            this.code3 = (int)stream.ReceiveNext();
            this.task1 = (int)stream.ReceiveNext();
            this.task2 = (int)stream.ReceiveNext();
            this.task3 = (int)stream.ReceiveNext();
        }
    }

}
