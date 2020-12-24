using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    PhotonView pv;

    Vector3 velocity;
    Vector3 targetPos;
    Ray cameraRay;
    RaycastHit cameraRayHit;

    Animator animator;
    Rigidbody rb;

    public float moveSpeed = 5;
    public float fps = 30.0f;

    bool isSpawn = false;
    bool isShotStand = false;
    bool isBringObject = false;

    public GameObject gun;
    public GameObject bullet;
    public GameObject inventory;

    public Transform gunPos;
    public LineRenderer lr;

    
    public Texture2D[] frames;
    public MeshRenderer muzzleRenderer;

    private int frameIndex;

    public string currentRole;
    public Role role;
    public Text roleText;
    public CanvasGroup roleReveal;

    public Text code;
    public GameController gc;

    private void Awake()
    {
        this.transform.rotation = Quaternion.identity;
    }
    void Start()
    {
        lr.enabled = false;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
        inventory = GameObject.Find("Inventory");
        this.transform.rotation = Quaternion.identity;
        gc = FindObjectOfType<GameController>();

        
        GetComponent<PhotonView>().RPC("GetRole", RpcTarget.All);
        if (!pv.IsMine)
        {
            roleReveal.gameObject.SetActive(false);
            code.gameObject.SetActive(false);
            return;
        }

        if (pv.IsMine)
        {
            roleReveal.gameObject.SetActive(true);
            code.gameObject.SetActive(true);
            GetComponent<PhotonView>().RPC("seeRole", RpcTarget.All);
        }

        
        

    }

    // Update is called once per frame
    void Update()
    {

        code.text = gc.code1.ToString() + gc.code2.ToString() + gc.code3.ToString() + " " + gc.task1.ToString() + " " + gc.task2.ToString() + " " + gc.task3.ToString();


        if (!isSpawn)
        {
            this.transform.rotation = Quaternion.identity;
            isSpawn = true;
        }

        if (!isShotStand)
        {
            if (!isBringObject)
            {
                velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
                animator.SetBool("walking", true);
                animator.SetFloat("speed", velocity.magnitude / moveSpeed, .1f, Time.deltaTime);
            }
            else
            {
                velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * (moveSpeed / 2);
                animator.SetBool("walking", true);
                animator.SetFloat("speed", velocity.magnitude / (moveSpeed / 2), .1f, Time.deltaTime);
            }
            
        }

        if(inventory.transform.childCount > 0)
        {
            isBringObject = true;
        }
        else
        {
            isBringObject = false;
        }

        if(velocity.magnitude <= 0)
        {
            animator.SetBool("walking", false);
        }


        if (Input.GetMouseButton(1))
        {
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
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<PhotonView>().RPC("ShotGun", RpcTarget.AllBuffered);
                animator.SetTrigger("isShooting");
            }
        }
        else
        {
            lr.enabled = false;
            isShotStand = false;
            animator.SetBool("isPistol", false);
        }
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

    


    [PunRPC]
    private void ShotGun()
    {
        //audioSource.PlayOneShot(gunshotSound);
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
    public void GetRole()
    {
        role = FindObjectOfType<Role>();
        currentRole = role.getRole();
        gameObject.name = role.totalPlayer.ToString();
    }

    [PunRPC]
    public void seeRole()
    {
        roleText.text = currentRole;
        StartCoroutine(fadeOut(true));
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
            Destroy(this.roleReveal.gameObject);
        }
        yield return null;
    }
}
