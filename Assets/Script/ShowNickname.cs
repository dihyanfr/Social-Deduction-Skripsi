namespace Photon.Pun.Demo.PunBasics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Photon;

    public class ShowNickname : MonoBehaviour
    {
        private string stringNickname;

        [SerializeField] private PhotonView pv;
        
        [SerializeField] private Text fieldNickname;

        [SerializeField] private GameObject namePlaceholder;

        Quaternion fixRot;

        private void Awake()
        {
            fixRot = transform.rotation;
        }

        void Start()
        {
            
            stringNickname = pv.Owner.NickName;
            fieldNickname.text = stringNickname;
        }

        private void Update()
        {
           
        }
    }
}
