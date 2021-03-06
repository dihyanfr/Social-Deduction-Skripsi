﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Pun.Simple;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    
    PhotonView pv;

    Vector3 velocity;
    Vector3 targetPos;
    Ray cameraRay;
    RaycastHit cameraRayHit;

    Animator animator;
    SyncAnimator s_animator;
    SyncTransform s_transform;
    NetObject netObj;
    Rigidbody rb;
    public Rigidbody[] rb_ragdoll;
    public Collider[] cl_ragdoll;

    public float moveSpeed = 5;
    public AudioClip[] step;
    public AudioSource audioSource;
    public Speaker speaker;
    public AudioListener audioListener;
    public float fps = 30.0f;

    bool isSpawn = false;
    bool isShotStand = false;
    bool isBringObject = false;

    public GameObject gun;
    public GameObject bullet;
    public AudioClip gunshotSound;
    public GameObject inventory;

    public Transform gunPos;
    public LineRenderer lr;

    
    public Texture2D[] frames;
    public MeshRenderer muzzleRenderer;

    private int frameIndex;

    public string currentRole;
    public Role role;
    public Text roleText;
    public bool isMastermind;
    public CanvasGroup roleReveal;

    public Recorder recorder;

    public Canvas mastermindCanvas;
    public Canvas playerUI;

    public float healthPoint;
    public Slider healthPointSlider;
    public FieldOfView fov;
    public float fieldOfViewRadius;
    

    public Text code;
    public string codeTask;
    public GameController gc;
    public bool isDie;
    public bool alreadyDie = false;
    public Transform dieForcePos;
    public GameObject dieTrigger;

    public GameObject currentTarget;
    public GameObject objectPos;
    public GameObject currentBringObject;

    public GameObject boxIcon;
    public GameObject typeIcon;
    public GameObject memoryIcon;
    public GameObject connectIcon;
    public GameObject killIcon;

    public bool isKillOnCooldown;
    public float killCooldownTimer;
    public float killCooldownValue;
    public Slider killSlider;

    public bool canMove = false;

    bool healing;

    public GameObject arrow;

    public GameObject[] hairstyle;
    public GameObject[] shirt;
    public Color shirtColor;
    public GameObject[] pants;
    public Color pantsColor;
    public GameObject bones;

    public GameObject pause;

    private void Awake()
    {
        this.transform.rotation = Quaternion.identity;
    }
    void Start()
    {
        lr.enabled = false;
        animator = GetComponent<Animator>();
        //s_animator = GetComponent<SyncAnimator>();
        //netObj = GetComponent<NetObject>();
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        inventory = GameObject.Find("Inventory");
        this.transform.rotation = Quaternion.identity;
        gc = FindObjectOfType<GameController>();

        rb_ragdoll = GetComponentsInChildren<Rigidbody>();
        cl_ragdoll = GetComponentsInChildren<Collider>();

        
        if (!pv.IsMine)
        {
            roleReveal.gameObject.SetActive(false);
            code.gameObject.SetActive(false);
            playerUI.gameObject.SetActive(false);
            Destroy(audioListener);
            return;
        }

        if (pv.IsMine)
        {
            GetComponent<PhotonView>().RPC("GetRole", RpcTarget.All);
            roleReveal.gameObject.SetActive(true);
            //code.gameObject.SetActive(true);
            GetComponent<PhotonView>().RPC("seeRole", RpcTarget.All);
        }

        
    }
    void Update()
    {
        

        //codeTask = gc.code1.ToString() + gc.code2.ToString() + gc.code3.ToString() + " " + gc.task1.ToString() + " " + gc.task2.ToString() + " " + gc.task3.ToString();
        if (!pv.IsMine)
        {
            return;
        }

        if (currentTarget != null)
        {
            
        }

        if (!canMove)
        {
            Debug.Log("CANT MOVEEEEE");
            animator.SetBool("walking", false);
            animator.SetFloat("speed", 0);
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 0;
            return;
        }

        if (isKillOnCooldown)
        {
            if (killCooldownValue <= killCooldownTimer)
            {
                killCooldownValue += Time.deltaTime;

                killSlider.value = killCooldownValue / killCooldownTimer;

                Debug.Log("tempLockedTime = " + killCooldownValue);
            }
            else
            {
                isKillOnCooldown = false;
                killCooldownValue = 0;
                killSlider.value = 0;
            }
        }

        healthPointSlider.value = healthPoint;

        if (isDie && !alreadyDie)
        {
            GetComponent<PhotonView>().RPC("die", RpcTarget.All);
        }

        if (!isSpawn)
        {
            this.transform.rotation = Quaternion.identity;
            isSpawn = true;
        }

        if (!isShotStand && canMove)
        {
            if (!isBringObject)
            {
                velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
                if(animator != null)
                {
                    animator.SetBool("walking", true);
                    //Debug.Log(velocity.magnitude);
                    animator.SetFloat("speed", velocity.magnitude, .1f, Time.deltaTime);
                }
                
            }
            else
            {
                velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * (moveSpeed / 2);
                if (animator != null)
                {
                    animator.SetBool("walking", true);
                    animator.SetFloat("speed", velocity.magnitude / 2, .1f, Time.deltaTime);
                }
            }
        }

        //if(inventory.transform.childCount > 0)
        //{
        //    isBringObject = true;
        //}
        //else
        //{
        //    isBringObject = false;
        //}

        if(velocity.magnitude <= 0)
        {
            animator.SetBool("walking", false);
        }

        if (Input.GetMouseButton(1) && !isBringObject)
        {
            Cursor.visible = false;
            
            velocity = new Vector3(0, 0, 0);
            isShotStand = true;
            animator.SetBool("isPistol", true);

            cameraRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

            if (Physics.Raycast(cameraRay, out cameraRayHit))
            {
                targetPos = new Vector3(cameraRayHit.point.x, gunPos.transform.position.y, cameraRayHit.point.z);
                lr.enabled = true;
                lr.SetPosition(0, gunPos.transform.position);
                lr.SetPosition(1, targetPos);
                transform.LookAt(new Vector3(targetPos.x, 0, targetPos.z));
            }
            if (Input.GetMouseButtonDown(0) && !isBringObject)
            {
                animator.SetTrigger("isShooting");
                Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
                GetComponent<PhotonView>().RPC("ShotGun", RpcTarget.AllBuffered);
            }
        }
        else
        {
            lr.enabled = false;
            isShotStand = false;
            animator.SetBool("isPistol", false);
        }
        Cursor.visible = true;

        if (Input.GetKey(KeyCode.C))
        {
            recorder.TransmitEnabled = true;
        }
        else
        {
            recorder.TransmitEnabled = false;

        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (pause.activeSelf)
            {
                pause.SetActive(false);
            }
            else
            {
                pause.SetActive(true);
            }
        }

        if (isMastermind)
        {
            Destroy(dieTrigger);
        }

        if (gc.isEscape)
        {
            arrow.SetActive(true);
            arrow.transform.LookAt(GameObject.FindGameObjectWithTag("Escape").transform);
        }
        else
        {
            arrow.transform.LookAt(GameObject.FindGameObjectWithTag("CP").transform);
        }

        //arrow.transform.LookAt(GameObject.FindGameObjectWithTag("CP").transform);
        //arrow.transform.rotation = Quaternion.Euler(0, arrow.transform.rotation.y, 0);
        //fieldOfViewRadius = gc.playerViewRadius;
        //fov.setRadius(fieldOfViewRadius);
    }
    private void FixedUpdate()
    {
        if (!pv.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
        if (velocity == new Vector3(0, 0, 0))
        {
            //transform.LookAt(GetComponent<Rigidbody>().position + velocity);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 5f);
        }
    }

    public void exitGame()
    {
        Application.Quit();
    }


    private void OnTriggerStay(Collider other)
    {
     

        if (other.gameObject.name == "DieArea" && isMastermind)
        {
            Debug.Log(this.gameObject.name + " entering and stay die area - trigger "+ other.gameObject.name);
            killIcon.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) && !isKillOnCooldown)
            {
                //other.GetComponentInParent<PlayerMovement>().getKilled();
                //other.GetComponentInParent<PhotonView>().GetComponent<PlayerMovement>().getKilled();
                other.GetComponentInParent<PhotonView>().RPC("getKilled", RpcTarget.AllBuffered);
                Debug.Log("Kill");
                isKillOnCooldown = true;
                
           
            }
        }

        if(other.gameObject.tag == "Box")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(currentBringObject == null)
                {
                    other.GetComponentInParent<PhotonView>().RPC("isTake", RpcTarget.AllBuffered, this.objectPos.transform.position, this.objectPos.transform.rotation);
                    currentBringObject = other.transform.gameObject;
                    animator.SetBool("isBring", true);
                    isBringObject = true;

                    arrow.SetActive(true);
                }
                else
                {
                    other.GetComponentInParent<PhotonView>().RPC("Throw", RpcTarget.AllBuffered);
                    currentBringObject = null;
                    animator.SetBool("isBring", false);
                    isBringObject = false;
                    arrow.SetActive(false);
                }
                
            }
        }

        if(other.GetComponent<MiniGameController>() != null || other.GetComponent<BoxController1>() != null)
        {
            if (!pv.IsMine)
            {
                return;
            }
            if (other.gameObject.tag == "MemoryGame")
            {
                memoryIcon.SetActive(true);

                boxIcon.SetActive(false);
                connectIcon.SetActive(false);
                typeIcon.SetActive(false);
            }

            if (other.gameObject.tag == "Box")
            {
                boxIcon.SetActive(true);

                memoryIcon.SetActive(false);
                connectIcon.SetActive(false);
                typeIcon.SetActive(false);
            }

            if (other.gameObject.tag == "ConnectGame")
            {
                connectIcon.SetActive(true);

                memoryIcon.SetActive(false);
                boxIcon.SetActive(false);
                typeIcon.SetActive(false);
            }

            if (other.gameObject.tag == "TypeGame")
            {
                typeIcon.SetActive(true);

                memoryIcon.SetActive(false);
                boxIcon.SetActive(false);
                connectIcon.SetActive(false);
            }

            
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.GetComponent<MiniGameController>().startMiniGame();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!pv.IsMine)
        {
            return;
        }
        memoryIcon.SetActive(false);
        boxIcon.SetActive(false);
        connectIcon.SetActive(false);
        typeIcon.SetActive(false);
        killIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!pv.IsMine)
        {
            return;
        }
        if (other.gameObject.name == "Hand Zombie")
        {
            healthPoint -= 10f;
            if(healthPoint <= 0)
            {
                GetComponent<PhotonView>().RPC("getKilled", RpcTarget.AllBuffered);
            }
        }

        if(other.gameObject.tag == "Escape")
        {
            gc.addEscapeRPC();
        }
    }

    private void Step()
    {
        audioSource.PlayOneShot(step[Random.Range(0, step.Length)]);
    }

    [PunRPC]
    private void ShotGun()
    {
        audioSource.PlayOneShot(gunshotSound);
        GameObject _bullet = Instantiate(bullet, gun.transform.position, Quaternion.identity) as GameObject;
        Debug.Log(_bullet.transform.position);
        Rigidbody rb_bullet = _bullet.GetComponent<Rigidbody>();
        //rb_bullet.AddForce(gun.transform.right * 1000f);
        //rb_bullet.AddForce(new Vector3(targetPos.x, 0, targetPos.z) * 100f);
        Debug.Log(targetPos);
        rb_bullet.AddForce(gunPos.transform.forward * 200f);
        Destroy(_bullet, 1f);
    }


    [PunRPC]
    public void getHealed()
    {
        if (!healing)
        {
            StartCoroutine(heal());
        }
        
    }

    [PunRPC]
    public void GetRole()
    {
        role = FindObjectOfType<Role>();
        currentRole = role.getRole();

        if(currentRole == "Mastermind")
        {
            if(gc.totalMastermind != 1)
            {
                Debug.Log("You are Mastermind");
                isMastermind = true;
                gc.addRole(0);
            }
            else
            {
                Debug.Log("You are Crewmate");
                Destroy(mastermindCanvas);
                gc.addRole(1);
            }
            
        }
        else
        {
            Debug.Log("You are Crewmate");
            Destroy(mastermindCanvas);
            gc.addRole(1);
        }

        gameObject.name = role.totalPlayer.ToString();
    }

    [PunRPC]
    public void seeRole()
    {
        roleText.text = currentRole;
        StartCoroutine(fadeOut(true));
    }

    [PunRPC]
    public void getKilled()
    {
        isDie = true;
        for (int i = 0; i < hairstyle.Length; i++)
        {
            hairstyle[i].transform.parent = null;
        }
        for (int i = 0; i < shirt.Length; i++)
        {
            shirt[i].transform.parent = null;
        }
        for (int i = 0; i < pants.Length; i++)
        {
            pants[i].transform.parent = null;
        }

        rb.isKinematic = false;

        fov.viewAngle = 360;

        bones.transform.parent = null;
        rb.isKinematic = false;
        gc.addDieRPC();
    }

    [PunRPC]
    public void die()
    {
        //netObj.enabled = false;
        animator.enabled = false;

        for (int x = 0; x < rb_ragdoll.Length; x++)
        {
            
            if(x != 0)
            {
                cl_ragdoll[x].isTrigger = false;
                rb_ragdoll[x].isKinematic = false;
            }
            else
            {
                Destroy(cl_ragdoll[x]);
                rb_ragdoll[x].useGravity = false;
                rb_ragdoll[x].isKinematic = true;
            }
            
        }

        Vector3 position = dieForcePos.position;
        Collider[] colliders = Physics.OverlapSphere(position, 5f);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            Debug.Log(hit);
            if (rb != null)
            {
                //rb.AddExplosionForce(1f, position, 2f, 1, ForceMode.Impulse);
            }
        }

        //s_animator.enabled = false;
        //s_transform.enabled = false;
        rb.isKinematic = false;
        alreadyDie = true;
        
    }

    public void lockDoor()
    {
        Debug.Log("LOCKING DOOR!");
        GameObject[] bigDoor = GameObject.FindGameObjectsWithTag("DoorDetection");

        //gc.GetComponent<PhotonView>().RPC("lockedDoorUI",RpcTarget.AllBuffered);

        for (int i = 0; i < bigDoor.Length; i++)
        {
            Debug.Log("LOCKING DOOR!");
            Debug.Log(bigDoor[i]);
            bigDoor[i].GetComponent<PhotonView>().RPC("lockDoor", RpcTarget.AllBuffered);
        }
    }

    public void visionSabotage()
    {
        gc.visionSabotage();
    }


    IEnumerator fadeOut(bool fadeaway)
    {
        yield return new WaitForSeconds(5);

        if (fadeaway)
        {
            for (float i = 3; i >= 0; i -= Time.deltaTime)
            {
                roleReveal.alpha -= 0.05f;
                yield return null;
            }
            canMove = true;
            Destroy(this.roleReveal.gameObject);
        }
        yield return null;
    }

    IEnumerator heal()
    {
        healing = true;

        yield return new WaitForSeconds(3);

        Debug.Log("healing 10f");

        healthPoint += 10f;
        healing = false;

        if (healthPoint > 100f)
        {
            healthPoint = 100f;
        }

        yield return null;
    }
}
