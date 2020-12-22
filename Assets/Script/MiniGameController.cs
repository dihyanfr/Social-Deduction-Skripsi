using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    [SerializeField] GameObject minigameCanvas;
    private AudioSource audioSource;
    [SerializeField] private AudioClip startSFX;
    [SerializeField] private AudioClip completeSFX;

    [SerializeField] private GameObject currentInteract;

    void Start()
    {
        audioSource = GameObject.FindObjectOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentInteract = collision.gameObject;
            startMiniGame();
        }
    }

    public void startMiniGame()
    {
        //currentInteract.GetComponent<PlayerController>().canMove = false;
        minigameCanvas.SetActive(true);
        //audioSource.PlayOneShot(startSFX);
    }

    public void endMiniGame()
    {
        //currentInteract.GetComponent<PlayerController>().canMove = true;
        minigameCanvas.SetActive(false);
        //audioSource.PlayOneShot(completeSFX);
    }
}
