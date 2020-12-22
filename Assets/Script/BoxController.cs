using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Simple;

public class BoxController : MonoBehaviour
{

    public InventoryContactReactors icr;
    public BoxCollider bc;

    void Start()
    {
        icr = GetComponent<InventoryContactReactors>();
    }

    // Update is called once per frame
    void Update()
    {
    
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "CollectionPoint")
        {
            Destroy(icr);
        }
    }
}
