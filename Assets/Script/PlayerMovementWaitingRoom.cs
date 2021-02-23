using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Pun.Simple;
using UnityEngine.UI;

public class PlayerMovementWaitingRoom : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Animator animator;
    [SerializeField] public float moveSpeed = 5;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public PhotonView pv;
    [SerializeField] public Recorder recorder;
    [SerializeField] public GameObject pause;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();

        if (!pv.IsMine)
        {
            return;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }


        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        animator.SetBool("walking", true);
        //Debug.Log(velocity.magnitude);
        animator.SetFloat("speed", velocity.magnitude, .1f, Time.deltaTime);

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

}
