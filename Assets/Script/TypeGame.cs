﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class TypeGame : MonoBehaviourPun, IPunObservable
{
    //public Text wordOutput = null;

    private AudioSource audioSource;
    [SerializeField] private AudioClip correctSFX;
    [SerializeField] private AudioClip wrongSFX;

    private MiniGameController mgc;
    public GameObject mgc_gb;

    private string remainingWord = string.Empty;
    private string currentWord = "TESTING";

    private float score;
    private int streak;

    public string[] listWord;

    [SerializeField] private Text correctWord;
    [SerializeField] private InputField typeWord;
    [SerializeField] private Text testWord;

    [SerializeField] string code;
    [SerializeField] int codePos;
    [SerializeField] GameObject codeText;

    public bool isHacked;

    public Slider progressBar;

    private string currentType;
    private string currentCheckWord;

    void Start()
    {
        isHacked = false;
    }

    private void OnEnable()
    {
        if (isHacked)
        {
            testWord.text = "";
            codeText.SetActive(true);
            return;
        }

        mgc = mgc_gb.GetComponent<MiniGameController>();
        audioSource = GameObject.FindObjectOfType<AudioSource>();
        currentWord = getWord();
        correctWord.text = currentWord;
        progressBar.value = 0;
        score = 0f;
        streak = 0;
    }


    void Update()
    {
        if (code != null)
        {
            codeText.GetComponent<Text>().text = code;
        }

        if (isHacked)
        {
            testWord.text = "";
            codeText.SetActive(true);
            return;
        }

        typeWord.ActivateInputField();
        typeWord.text = typeWord.text.ToUpper();
        currentType = typeWord.text;

        currentCheckWord = currentWord.Substring(0, currentType.Length);

        if (currentType != currentCheckWord)
        {
            typeWord.text = string.Empty;
            score -= 5f;
            progressBar.value = score;
            audioSource.PlayOneShot(wrongSFX);
            streak = 0;
        }

        testWord.text = "<color=white>" + currentCheckWord + "</color>" + "<color=grey>" + currentWord.Substring(currentCheckWord.Length) + "</color>";

        if (currentType == currentWord && Input.GetKeyDown(KeyCode.Return))
        {
            streak += 1;
            score += 10f + 5 * streak;
            Debug.Log(score);
            typeWord.text = string.Empty;
            currentWord = getWord();
            correctWord.text = currentWord;
            progressBar.value = score;


            if (score >= 100f)
            {
                mgc.endMiniGame();
                //codeText.SetActive(true);
                //GetComponent<PhotonView>().RPC("setHacked", RpcTarget.All);
                //isHacked = true;
                return;
            }

            audioSource.PlayOneShot(correctSFX);
        }

        progressBar.value -= 2f * Time.deltaTime;
        score = progressBar.value;
    }

    private string getWord()
    {
        int ran = Random.Range(0, listWord.Length);
        return listWord[ran];
    }

    public void close()
    {
        //this.gameObject.SetActive(false);
        mgc.endMiniGame();
    }

    public void setCode(string _code, int _pos)
    {
        code = _code;
        codePos = _pos;
    }

    [PunRPC]
    public void setHacked()
    {
        Debug.Log("HACKED");
        this.isHacked = true;
        isHacked = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.isHacked);
        }
        else
        {
            this.isHacked = (bool)stream.ReceiveNext();
        }
    }
}
