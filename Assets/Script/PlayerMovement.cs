using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

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
}
