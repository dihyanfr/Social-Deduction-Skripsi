using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieController : MonoBehaviour
{
    [SerializeField] public GameObject zombie;
    
    [SerializeField] public GameObject spawnPosition;

    [SerializeField] public int totalZombie = 0;
    public int temptotalZombie;
    [SerializeField] public List<GameObject> listZombie = new List<GameObject>();

    [SerializeField] public float max_X;
    [SerializeField] public float max_Z;
    [SerializeField] public bool isConnect;

    public float rateSpawn;
    private float rate;

    public bool isSpawnable;

    public bool isHorde;

    void Start()
    {
        rate = rateSpawn;
    }

    private void OnEnable()
    {
        isHorde = true;
        rate = rateSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHorde)
        {
            
            rate += Time.deltaTime;

            if (rate >= rateSpawn)
            {
                float randomX = Random.Range(max_X * -1, max_X);
                float randomZ = Random.Range(max_Z * -1, max_Z);
                spawnPosition.transform.position = new Vector3(randomX, this.transform.position.y, randomZ);

                if (isSpawnable)
                {
                    Debug.Log("Spawning");

                    if (isConnect)
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            listZombie.Add(PhotonNetwork.InstantiateRoomObject(zombie.name,spawnPosition.transform.position, Quaternion.identity, 0, null));
                            temptotalZombie++;
                        }
                    }
                    else
                    {
                        listZombie.Add(Instantiate(zombie, spawnPosition.transform.position, Quaternion.identity));
                        temptotalZombie++;
                    }
                }

                rate = 0;
            }

            if(temptotalZombie == totalZombie)
            {
                isHorde = false;
                temptotalZombie = 0;
                this.gameObject.GetComponent<ZombieController>().enabled = false;
            }
        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isSpawnable = false;
    }

    private void OnTriggerExit(Collider other)
    {
        isSpawnable = true;
    }
}
