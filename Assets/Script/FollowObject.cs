using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
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
            this.transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 7f, target.transform.position.z - 5.5f);
        }

        


    }
}
