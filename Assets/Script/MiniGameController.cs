using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MiniGameController : MonoBehaviour
{
    [SerializeField] public string minigameName;
    [SerializeField] GameObject minigameCanvas;
    private AudioSource audioSource;
    [SerializeField] private AudioClip startSFX;
    [SerializeField] private AudioClip completeSFX;

    [SerializeField] private GameObject currentInteract;

    [SerializeField] public GameObject objectInteraction;
    [SerializeField] public Material interactionMaterial;
    [SerializeField] public BoxCollider triggerCollider;
    [SerializeField] public GameObject activePlane;

    [SerializeField] public GameObject gc;

    [SerializeField] public GameObject hint;
    [SerializeField] public GameObject[] hintPage;
    [SerializeField] public GameObject nextHintButton;
    [SerializeField] public GameObject beforeHintButton;
    [SerializeField] public int currentHintPage;


    public int indexMaterial;
    public Material[] tempMaterial;
    private Material tempMats;

    public bool isDone;



    void Start()
    {
        currentHintPage = 0;
        audioSource = GameObject.FindObjectOfType<AudioSource>();
        tempMaterial = objectInteraction.GetComponent<MeshRenderer>().materials;
        tempMats = tempMaterial[indexMaterial];

        setHintPage();
    }

    // Update is called once per frame
    void Update()
    {

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 5f);

        //Debug.Log(hitColliders[0]);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.tag == "Player")
            {
                //objectInteraction.GetComponent<MeshRenderer>().material = interactionMaterial;
            }
            else
            {
                //objectInteraction.GetComponent<MeshRenderer>().material = tempMats;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, 5);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player" && !isDone)
        {

            //tempMaterial[indexMaterial] = interactionMaterial;
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteract = collision.gameObject;
                startMiniGame();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //currentInteract = other.gameObject;
        if (other.tag == "Player" && !isDone)
        {
            activePlane.SetActive(true);
            currentInteract = other.gameObject;
            objectInteraction.GetComponent<MeshRenderer>().materials[0] = interactionMaterial;
            if (Input.GetKeyDown(KeyCode.E))
            {
                //currentInteract = other.gameObject;
                //startMiniGame();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        currentInteract = null;
        if (other.tag == "Player")
        {
            activePlane.SetActive(false);
            objectInteraction.GetComponent<MeshRenderer>().materials[0] = tempMats;
            //objectInteraction.GetComponent<MeshRenderer>().material = tempMaterial[];
            tempMaterial[indexMaterial] = tempMats;
        }
    }

    public void startMiniGame()
    {
        currentInteract.GetComponent<PlayerMovement>().canMove = false;
        minigameCanvas.SetActive(true);
        //audioSource.PlayOneShot(startSFX);
    }

    public void endMiniGame()
    {
        currentInteract.GetComponent<PlayerMovement>().canMove = true;
        minigameCanvas.SetActive(false);
        isDone = true;
        gc.GetComponent<PhotonView>().RPC("taskDone", RpcTarget.AllBuffered, minigameName);
        Destroy(triggerCollider);
        //audioSource.PlayOneShot(completeSFX);
    }

    public void stopMiniGame()
    {
        currentInteract.GetComponent<PlayerMovement>().canMove = true;
        Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
        minigameCanvas.SetActive(false);
    }

    public void setHintPage()
    {
        for(int i = 0; i < hintPage.Length; i++)
        {
            hintPage[i].SetActive(false);
        }

        hintPage[currentHintPage].SetActive(true);

        if (currentHintPage == 0)
        {
            beforeHintButton.SetActive(false);
            nextHintButton.SetActive(true);
        }
        if (currentHintPage > 0 && currentHintPage < hintPage.Length)
        {
            beforeHintButton.SetActive(true);
            nextHintButton.SetActive(true);
        }
        if (currentHintPage == hintPage.Length - 1)
        {
            beforeHintButton.SetActive(true);
            nextHintButton.SetActive(false);
        }
    }

    public void nextHint()
    {
        currentHintPage++;
        setHintPage();
    }

    public void beforeHint()
    {
        currentHintPage--;
        setHintPage();
    }
}
