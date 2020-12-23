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

    [SerializeField] public GameObject objectInteraction;
    [SerializeField] public Material interactionMaterial;
    public int indexMaterial;
    public Material[] tempMaterial;
    private Material tempMats;


    void Start()
    {
        audioSource = GameObject.FindObjectOfType<AudioSource>();
        tempMaterial = objectInteraction.GetComponent<MeshRenderer>().materials;
        tempMats = tempMaterial[indexMaterial];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {

        //objectInteraction.GetComponent<MeshRenderer>().material.color = Color.yellow;

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentInteract = collision.gameObject;
            startMiniGame();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
        
        if (other.tag == "Player")
        {
            objectInteraction.GetComponent<MeshRenderer>().material = interactionMaterial;
            //tempMaterial[indexMaterial] = interactionMaterial;
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteract = other.gameObject;
                startMiniGame();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            objectInteraction.GetComponent<MeshRenderer>().material = tempMats;
            //objectInteraction.GetComponent<MeshRenderer>().material = tempMaterial[];
            tempMaterial[indexMaterial] = tempMats;
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
