using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    [SerializeField] public Text FPS;
    [SerializeField] public Text averageFPS;
    [SerializeField] private float hudRefreshRate = 1f;
    [SerializeField] public int total;
    [SerializeField] public int totalFPS;

    private float timer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            totalFPS += fps;
            total++;
            int average_fps = totalFPS / total;
            FPS.text = "FPS: " + fps;
            averageFPS.text = "Average FPS: " + average_fps.ToString();
            timer = Time.unscaledTime + hudRefreshRate;
        }
    }
}
