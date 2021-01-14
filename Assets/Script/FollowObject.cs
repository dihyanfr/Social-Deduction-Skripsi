using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] public float offsetX;
    [SerializeField] public float offsetY;
    [SerializeField] public float offsetZ;

    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 6f, target.transform.position.z - 5.5f);
        }
    }

    void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            this.transform.position = new Vector3(target.transform.position.x + offsetX, target.transform.position.y + offsetY, target.transform.position.z - offsetZ);
            //this.transform.LookAt(target.transform);
        }

    }
}
