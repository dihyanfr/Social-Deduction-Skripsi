using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesController : MonoBehaviour
{
    public GameObject bomb;
    public float power = 10f;
    public float radius = 5f;
    public float force = 1f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        detonate();
    }

    public void detonate()
    {
        Vector3 position = this.transform.position;
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach(Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(power, position, radius, force, ForceMode.Impulse);
            }
        }
    }
}
