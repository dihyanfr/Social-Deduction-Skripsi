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

    [SerializeField] public GameObject gc;
    public int indexMaterial;
    public Material[] tempMaterial;
    private Material tempMats;

    public bool isDone;



    void Start()
    {
        audioSource = GameObject.FindObjectOfType<AudioSource>();
        tempMaterial = objectInteraction.GetComponent<MeshRenderer>().materials;
        tempMats = tempMaterial[indexMaterial];
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
        if (other.tag == "Player" && !isDone)
        {
            objectInteraction.GetComponent<MeshRenderer>().material = interactionMaterial;
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
        //audioSource.PlayOneShot(completeSFX);
    }
}
