using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
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

    public List<Transform> visibleTargets = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        StartCoroutine("FindTargetsWithDelay", .2f);
        
    }

    // Update is called once per frame
    void Update()
    {
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

            agent.SetDestination(visibleTargets[visibleTargets.Count - 1].transform.position);
        }

        if(hp <= 0)
        {
            agent.speed = 0f;
            Destroy(this.gameObject, 3f);
        }

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
                            Debug.Log("Thats zombie");
                           
                        }
                        else
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
        if(collision.transform.tag == "Bullet")
        {
            Debug.Log("HIT");
            hp -= Random.Range(50f, 105f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            Debug.Log("HIT");
            hp -= Random.Range(50f, 105f);
        }
    }
}
