using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ZombieController : MonoBehaviour
{
    [SerializeField] public GameObject zombie;
    [SerializeField] public GameObject spawnPosition;

    [SerializeField] public int totalZombie = 0;
    [SerializeField] public List<GameObject> listZombie = new List<GameObject>();

    [SerializeField] public float max_X;
    [SerializeField] public float max_Z;

    public float rateSpawn;
    private float rate;

    public bool isSpawnable;

    void Start()
    {
        rate = rateSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        rate += Time.deltaTime;

        if (rate >= rateSpawn)
        {
            float randomX = Random.Range(max_X * -1, max_X);
            float ranzomZ = Random.Range(max_Z * -1, max_Z);
            spawnPosition.transform.position = new Vector3(randomX, 1, ranzomZ);

            if (isSpawnable)
            {
                Debug.Log("Spawning");
                //listZombie.Add(Instantiate(zombie, spawnPosition.transform.position, Quaternion.identity));
                if (PhotonNetwork.IsMasterClient)
                {
                    listZombie.Add(PhotonNetwork.InstantiateRoomObject("BH-2 Free", spawnPosition.transform.position, Quaternion.identity, 0, null));
                }
            }
            
            rate = 0;
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
