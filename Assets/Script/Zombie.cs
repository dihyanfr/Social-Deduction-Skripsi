using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public ZombieController zombieController;
    public float wanderRadius;
    public float wanderTimer;
    public float wanderSpeed;
    public float followSpeed;
    public float hp;

    

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Animator am;

    public List<Transform> visibleTargets = new List<Transform>();

    Rigidbody rb;
    public Rigidbody[] rb_ragdoll;
    public Collider[] cl_ragdoll;

    public AudioSource audioSource;
    public AudioClip[] zombieStepSFX;

    public GameObject hand;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        StartCoroutine("FindTargetsWithDelay", .2f);
        am = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        rb_ragdoll = GetComponentsInChildren<Rigidbody>();
        cl_ragdoll = GetComponentsInChildren<Collider>();
        zombieController = FindObjectOfType<ZombieController>();

        for (int x = 0; x < rb_ragdoll.Length; x++)
        {

            if (x != 0)
            {
                cl_ragdoll[x].isTrigger = true;
                rb_ragdoll[x].isKinematic = true;
            }
            else
            {
                //Destroy(cl_ragdoll[x]);
                rb_ragdoll[x].useGravity = true;
                rb_ragdoll[x].isKinematic = false;
            }

        }

        if (!agent.isOnNavMesh)
        {
            //Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        am.SetFloat("speed", agent.speed);
        timer += Time.deltaTime;
        
        if(timer >= wanderTimer)
        {
            agent.speed = wanderSpeed;
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
            wanderTimer = Random.Range(1f, 5f);
        }

        


        if (visibleTargets.Count >= 1)
        {
            agent.speed = followSpeed;
            Transform currentTarget = visibleTargets[visibleTargets.Count - 1];

            float distance = Vector3.Distance(this.transform.position, currentTarget.position);

            if(distance <= 1)
            {
                am.SetTrigger("attack");
            }
            else
            {
                agent.SetDestination(visibleTargets[visibleTargets.Count - 1].transform.position);
            }
            
        }

        if(hp <= 0)
        {
            agent.speed = 0f;
            die();
            Destroy(this.gameObject, 3f);
            
        }

    }

    public void ZombieStep()
    {
        audioSource.PlayOneShot(zombieStepSFX[Random.Range(0, zombieStepSFX.Length)]);
    }

    public void StartAttack()
    {
        hand.SetActive(true);
    }

    public void EndAttack()
    {
        hand.SetActive(false);
    }

    public static Vector3 RandomNavSphere(Vector3 origin,float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {

                    if(target != this.transform)
                    {
                        if (target.transform.tag == "Zombie")
                        {
                            //Debug.Log("Thats zombie");
                        }
                        else if(target.transform.tag == "Player")
                        {
                            visibleTargets.Add(target);
                        }
                    }
                }
            }
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.GetComponent<NavMeshAgent>().enabled = true;
        if (collision.transform.tag == "Bullet")
        {
            Debug.Log("HIT");
            hp -= Random.Range(50f, 105f);

        }

        if (collision.transform.tag == "ZC")
        {
            Destroy(this.gameObject);
            zombieController.temptotalZombie--;
            zombieController.listZombie.RemoveAt(zombieController.listZombie.Count - 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            Debug.Log("HIT");
            hp -= Random.Range(50f, 105f);
            am.SetTrigger("damage");
        }
        
    }

    public void die()
    {
        //netObj.enabled = false;
        am.enabled = false;

        for (int x = 0; x < rb_ragdoll.Length; x++)
        {

            if (x != 0)
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

        //Vector3 position = dieForcePos.position;
        //Collider[] colliders = Physics.OverlapSphere(position, 5f);

        //foreach (Collider hit in colliders)
        //{
        //    Rigidbody rb = hit.GetComponent<Rigidbody>();

        //    Debug.Log(hit);
        //    if (rb != null)
        //    {
        //        //rb.AddExplosionForce(1f, position, 2f, 1, ForceMode.Impulse);

        //    }
        //}

        //s_animator.enabled = false;
        //s_transform.enabled = false;
        //alreadyDie = true;

    }
}
