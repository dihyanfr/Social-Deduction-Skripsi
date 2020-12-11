using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class Controller : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private PhotonView myPV;

    [SerializeField] private Slider healthBar;
    [SerializeField] private Animator animator;

    //[SerializeField] private GameObject[] inventory = new GameObject[4];
    [SerializeField] private List<GameObject> inventory = new List<GameObject>();
    [SerializeField] private int maxInventory;
    [SerializeField] private GameObject[] inventoryUI;
    [SerializeField] private GameObject selectionUI;

    [SerializeField] private GameObject gun;
    [SerializeField] public GameObject bullet;
    GameObject _bullet;


    private int countInventory;
    private int currentSelected;

    private bool isGrounded;
    private bool wasGrounded;

    private bool isRun;

    public float healthPoint = 100f;

    public float moveSpeed = 6;
    public float walkSpeed = 3;

    [SerializeField] private AudioClip[] walkSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunshotSound;

    public Rigidbody rigidbody;
    Camera viewCamera;
    Vector3 velocity;



    Ray cameraRay;
    RaycastHit cameraRayHit;

    bool isScope = false;

    void Start()
    {

        if (!photonView.IsMine)
        {
            //gun.SetActive(false);
            //viewCamera.gameObject.SetActive(false);
            return;
        }

        if (myPV == null)
        {
            myPV = this.gameObject.GetComponent<PhotonView>();
        }

        inventoryUI[0] = GameObject.Find("Tools 1");
        inventoryUI[1] = GameObject.Find("Tools 2");
        inventoryUI[2] = GameObject.Find("Tools 3");
        inventoryUI[3] = GameObject.Find("Tools 4");

        selectionUI = GameObject.Find("Selection");

        gun = GameObject.Find("GunPos");
        //bullet = GameObject.Find("Bullet");
        
        rigidbody = this.GetComponent<Rigidbody>();
        viewCamera = Camera.main;

        //Set HP menjadi 100
        healthBar.value = 100f;
        healthPoint = 100f;
    }

    void Update()
    {
        if(viewCamera == null)
        {
            Debug.Log("Camera Missing!");
            viewCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        }

        if (!photonView.IsMine)
        {
            gun.SetActive(false);
            //viewCamera.gameObject.SetActive(false);
            return;
        }


        healthBar.value = this.healthPoint;

        

        Vector3 mousePos;
        //Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
        //Debug.DrawLine(transform.position, mousePos, Color.red);

        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(cameraRay, out cameraRayHit))
        {

            if (Input.GetKey(KeyCode.Alpha1))
            {
                selectionUI.transform.position = inventoryUI[0].transform.position;
                currentSelected = 0;
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                selectionUI.transform.position = inventoryUI[1].transform.position;
                currentSelected = 1;
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                selectionUI.transform.position = inventoryUI[2].transform.position;
                currentSelected = 2;
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                selectionUI.transform.position = inventoryUI[3].transform.position;
                currentSelected = 3;
            }


            if (Input.GetKey(KeyCode.E)) //Use Item
            {
                if (inventory[currentSelected].GetComponent<Tools>().isMedkit) //Heal if item Medkit
                {
                    Debug.Log("HEAL!");
                    healthPoint += 100f;

                    if(healthPoint > 100f)
                    {
                        healthPoint = 100f;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && myPV.IsMine) //Use Tools
            {

                GetComponent<PhotonView>().RPC("ShotGun", RpcTarget.All);
                //if (inventory[currentSelected].GetComponent<Tools>().isGun)
                //{

                //}
            }


            if (Input.GetMouseButton(1))
            {
                Debug.Log("Right Hold");
                isScope = true;
                //viewCamera.orthographicSize += Time.deltaTime;
                //fov

                Vector3 targetPos = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);

                Debug.Log(targetPos);
                transform.LookAt(targetPos);
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


        if (isScope)
        {

            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * walkSpeed; //Slow speed dibagi 2

            Debug.Log(velocity);

            animator.SetFloat("MoveSpeed", velocity.magnitude / 2, 1f, Time.deltaTime);
        }
        else
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

            //Debug.Log(velocity);

            animator.SetFloat("MoveSpeed", velocity.magnitude, .1f, Time.deltaTime);
        }

       

    }

    void FixedUpdate()
    {

        if (!photonView.IsMine)
        {
            return;
        }


        //Debug.Log("rigidbody pos: " + rigidbody.position);
        //Debug.Log("velocity: " + velocity);
        //Debug.Log("Time delta: " + Time.deltaTime);

        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);

        if (!isScope)
        {
            if (velocity == new Vector3(0, 0, 0))
            {
                transform.LookAt(rigidbody.position + velocity);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 5f);
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

            inventoryUI[countInventory-1].gameObject.GetComponent<Image>().sprite = other.GetComponent<Tools>().toolsIcon;
            
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collision.transform.CompareTag("Bullet"))
        {
            healthPoint = healthPoint - 1f;
            Debug.Log(healthPoint);
            healthBar.value = this.healthPoint;
        }


        if (collision.transform.CompareTag("Ground"))
        {
            Debug.Log("On Ground");

            isGrounded = true;
            animator.SetBool("Grounded", isGrounded);
            wasGrounded = false;
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

    [PunRPC]

    private void ShotGun()
    {
        Vector3 targetPos = new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
        transform.LookAt(targetPos);

        audioSource.PlayOneShot(gunshotSound);
        GameObject _bullet = Instantiate(bullet, gun.transform.position, Quaternion.identity) as GameObject;
        
        //Rigidbody rb_bullet = _bullet.GetComponent<Rigidbody>();
        //rb_bullet.AddForce(gun.transform.forward * 3000f);
        // Destroy(_bullet, 1f);
    }


    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.healthPoint);
            stream.SendNext(this.isGrounded);
            stream.SendNext(this.wasGrounded);
        }
        else
        {
            this.healthPoint = (float)stream.ReceiveNext();
            this.isGrounded = (bool)stream.ReceiveNext();
            this.wasGrounded = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}