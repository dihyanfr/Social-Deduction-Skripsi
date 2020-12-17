using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView myPV;

    public bool canMove = true;

    [SerializeField] public GameController gc;

    [SerializeField] private bool isMastermind;
    [SerializeField] private bool isHelper;
    [SerializeField] private bool isCrewmate;
    [SerializeField] private bool haveRole = false;

    [SerializeField] private Slider healthBar;
    [SerializeField] private Animator animator;

    //[SerializeField] private GameObject[] inventory = new GameObject[4];
    [SerializeField] List<GameObject> inventory = new List<GameObject>();
    [SerializeField] private int maxInventory;
    [SerializeField] GameObject[] inventoryUI;
    [SerializeField] GameObject[] selectionUI;

    [SerializeField] private GameObject gun;
    [SerializeField] public GameObject bullet;

    [SerializeField] public FieldOfView fov;

    [SerializeField] private CanvasGroup roleReveal;
    [SerializeField] private Text roleText;
    [SerializeField] private GameObject mastermindUI;
    [SerializeField] private GameObject playerUI;

    private Vector3 gunPos;
    private Quaternion gunRot;

    private int countInventory;
    private int currentSelected;

    private bool isGrounded;
    private bool wasGrounded;

    private bool isRun;
    private bool isGrab;

    private GameObject currentGrabObject;

    public float healthPoint = 100f;

    public float moveSpeed = 6;
    public float walkSpeed = 3;

    [SerializeField] AudioClip[] walkSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip gunshotSound;

    [SerializeField] private float ratefire = 0f;

    public Rigidbody rb;
    Camera viewCamera;
    Vector3 velocity;
    Vector3 targetPos;

    Ray cameraRay;
    RaycastHit cameraRayHit;

    bool isScope = false;

    bool isTarget = false;

    void Start()
    {
        //Set semua bool
        //haveRole = false;
        canMove = true;
        isGrab = false;

        

        //Set game component
        gc = FindObjectOfType<GameController>();
        rb = this.GetComponent<Rigidbody>();
        fov.setRadius(gc.playerViewRadius);
        //Set Camerea
        viewCamera = Camera.main;
        
        //Set HP menjadi 100
        healthBar.value = 100f;
        healthPoint = 100f;

        if (myPV == null)
        {
            myPV = this.gameObject.GetComponent<PhotonView>();
            Debug.Log("nama " + myPV.Owner.NickName);
            this.gameObject.name = myPV.Owner.NickName;
        }

        if (!myPV.IsMine)
        {
            //gun.SetActive(false);
            //Destroy(viewCamera);
            //viewCamera.gameObject.SetActive(false);
            return;
        }

        

        //inventoryUI[0] = GameObject.Find("Tools 1");
        //inventoryUI[1] = GameObject.Find("Tools 2");
        //inventoryUI[2] = GameObject.Find("Tools 3");
        //inventoryUI[3] = GameObject.Find("Tools 4");

        //selectionUI = GameObject.Find("Selection");

        //gun = GameObject.Find("GunPos");
        //bullet = GameObject.Find("Bullet");

        
    }


    void Update()
    {

        fov.setRadius(gc.playerViewRadius);

        

        if (!photonView.IsMine)
        {
            //viewCamera.gameObject.SetActive(false);
            return;
        }

        if (!haveRole)
        {
            int rolesID = gc.getRoles();
            Debug.Log("Roles ID" + rolesID);

            if (rolesID == 1)
            {
                isMastermind = true;
                mastermindUI.SetActive(true);
                haveRole = true;
                roleText.text = "EVIL MASTERMIND";
                StartCoroutine(fadeOut(true));
            }
            else
            {
                isCrewmate = true;
                haveRole = true;
                roleText.text = "CREWMATE";
                StartCoroutine(fadeOut(true));
            }
            //PhotonNetwork.Destroy(this.roleReveal.gameObject);
        }
        
        if (ratefire > 0f)
        {
            ratefire = ratefire - Time.deltaTime;
        }

        gunPos = gun.transform.position;
        gunRot = gun.transform.rotation;

        healthBar.value = this.healthPoint;

        //Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        //Debug.DrawLine(transform.position, mousePos, Color.red);

        //cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        cameraRay = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,transform.position.z));


        if (Physics.Raycast(cameraRay, out cameraRayHit))
        {

            if (Input.GetKey(KeyCode.Alpha1))
            {
                //selectionUI.transform.position = inventoryUI[0].transform.position;
                currentSelected = 0;
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                //selectionUI.transform.position = inventoryUI[1].transform.position;
                currentSelected = 1;
            }

            if (Input.GetKey(KeyCode.Q)) //Use Item
            {
                if (isGrab)
                {
                    currentGrabObject.GetComponent<Rigidbody>().isKinematic = false;
                    currentGrabObject.transform.parent = null;
                    isGrab = false;
                }
            }



            if (Input.GetMouseButton(1))
            {
                isScope = true;
                //viewCamera.orthographicSize += Time.deltaTime;
                //fov

                targetPos = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);

                transform.LookAt(targetPos);

                if (Input.GetMouseButtonDown(0) && myPV.IsMine) //Use Tools
                {
                    if (ratefire <= 0f)
                    {
                        targetPos = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
                        transform.LookAt(targetPos);


                        GetComponent<PhotonView>().RPC("ShotGun", RpcTarget.AllBuffered);
                        ratefire = 0.5f;
                    }
                }
                //if (viewCamera.orthographicSize >= 10f)
                //{
                //viewCamera.orthographicSize = 10f;
                //}


            }

            else
            {
                isScope = false;
                //viewCamera.orthographicSize -= Time.deltaTime;
                //if (viewCamera.orthographicSize <= 8f)
                //{
                //viewCamera.orthographicSize = 8f;
                //}
            }

        }


        if (isScope || isGrab)
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * walkSpeed; //Slow speed dibagi 2
            animator.SetFloat("MoveSpeed", velocity.magnitude / 2, 1f, Time.deltaTime);
        }
        else
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
            animator.SetFloat("MoveSpeed", velocity.magnitude, .1f, Time.deltaTime);
        }



    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (canMove)
        {
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
            if (!isScope)
            {
                if (velocity == new Vector3(0, 0, 0))
                {
                    transform.LookAt(GetComponent<Rigidbody>().position + velocity);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 5f);
                }
            }
        }
        

        
        JumpingAndLanding();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.CompareTag("Tools"))
        {
            countInventory++;
            inventory.Add(other.gameObject);
            inventoryUI[countInventory - 1].gameObject.GetComponent<Image>().sprite = other.GetComponent<Tools>().toolsIcon;
        }

        if (other.CompareTag("Bullet"))
        {
            GetComponent<PhotonView>().RPC("ReduceHealth", RpcTarget.AllBuffered);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collision.transform.CompareTag("Ground"))
        {
            Debug.Log("On Ground");

            isGrounded = true;
            animator.SetBool("Grounded", isGrounded);
            wasGrounded = false;
        }

        if (collision.transform.CompareTag("Zombie"))
        {
            Debug.Log("HIT");

            GetComponent<PhotonView>().RPC("ReduceHealth", RpcTarget.AllBuffered);
        }

        if (collision.transform.CompareTag("Interactable"))
        {
            if (!isGrab)
            {
                collision.gameObject.GetComponent<Grab>().grabBox();
                currentGrabObject = collision.gameObject;
                isGrab = true;
            }
        }
    }

    private void Step()
    {
        audioSource.PlayOneShot(GetRandomSound());
    }

    private AudioClip GetRandomSound()
    {
        return walkSound[UnityEngine.Random.Range(0, walkSound.Length)];
    }

    private void JumpingAndLanding()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
        }

        if (isGrounded && wasGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }


    public void gotHit()
    {

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
            PhotonNetwork.Destroy(this.roleReveal.gameObject);
        }
        yield return null;
    }

    [PunRPC]

    private void ShotGun()
    {
        audioSource.PlayOneShot(gunshotSound);
        GameObject _bullet = Instantiate(bullet, gun.transform.position, Quaternion.identity) as GameObject;
        Debug.Log(_bullet.transform.position);
        Debug.Log(targetPos);
        Rigidbody rb_bullet = _bullet.GetComponent<Rigidbody>();
        rb_bullet.AddForce(gun.transform.forward * 1000f);
        //rb_bullet.AddForce(targetPos * 100f);
        Destroy(_bullet,1f);
    }

    [PunRPC]

    private void ReduceHealth()
    {
        healthPoint = healthPoint - Random.Range(30f, 50f);
        healthBar.value = this.healthPoint;
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.healthPoint);
            stream.SendNext(this.isGrounded);
            stream.SendNext(this.wasGrounded);
            stream.SendNext(this.gunPos);
            stream.SendNext(this.gunRot);
            stream.SendNext(this.targetPos);
            stream.SendNext(this.haveRole);
            
        }
        else
        {
            this.healthPoint = (float)stream.ReceiveNext();
            this.isGrounded = (bool)stream.ReceiveNext();
            this.wasGrounded = (bool)stream.ReceiveNext();
            this.gunPos = (Vector3)stream.ReceiveNext();
            this.gunRot = (Quaternion)stream.ReceiveNext();
            this.targetPos = (Vector3)stream.ReceiveNext();
            this.haveRole = (bool)stream.ReceiveNext();
            
        }
    }

    #endregion
}
