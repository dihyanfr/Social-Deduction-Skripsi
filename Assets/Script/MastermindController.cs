using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MastermindController : MonoBehaviour
{
    [SerializeField] public GameController gc;


    [SerializeField] private Slider visionCooldown;
    public float visionCooldownTime;
    public bool visionOnCooldown;
    [SerializeField] private Slider targetingCooldown;
    public float targetingCooldownTime;

    void Start()
    {
        visionCooldown.value = 0;
        targetingCooldown.value = 0;

        visionOnCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (visionOnCooldown)
        {
            Debug.Log(visionCooldown.value);

            visionCooldown.value -= visionCooldownTime * Time.deltaTime;

            if(visionCooldown.value <= 0)
            {
                visionOnCooldown = false;
            }
        }
    }

    public void visionSabotage()
    {
        visionCooldown.value = 100;
        visionOnCooldown = true;

        gc.playerViewRadius = 2f;
        gc.isVisionSabotage = true;
    }
}
