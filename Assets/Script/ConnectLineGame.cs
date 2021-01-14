using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectLineGame : MonoBehaviour
{
    public MiniGameController mgc;
    public LineRenderer lrBlue;
    public LineRenderer lrRed;
    public Material lineMaterial;
    public float lineWidth;
    public Camera camera;

    public EventSystem es;

    public GameObject current;

    public int currentIndex;
    bool drawingBlue;
    bool drawingRed;

    bool blueDone;
    bool redDone;

    bool isRandom;

    public GameObject start;

    public GameObject buttonHolder;
    public Button[] button;
    public Button resetButton;
    public AudioClip buttonSFX;
    public AudioSource audioSource;

    int startBlue = 0;
    int endBlue = 0;
    int startRed = 0;
    int endRed = 0;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        currentIndex = 0;
        resetButton.gameObject.SetActive(false);
        button = buttonHolder.GetComponentsInChildren<Button>();//FindObjectsOfType<Button>();
        resetButton.gameObject.SetActive(true);
        setRandomStart();

        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
        }
        camera = Camera.main;
        camera.transform.rotation = Quaternion.identity;
    }

    public void setStart()
    {
        
        if (!drawingRed && !drawingBlue)
        {
            if(es.currentSelectedGameObject.GetComponent<Button>() == button[startBlue])
            {
                Debug.Log("START BLUE");
                drawingBlue = true;
                current = es.currentSelectedGameObject;
                lrBlue = button[startBlue].gameObject.AddComponent<LineRenderer>();

                lrBlue.material = lineMaterial;
                lrBlue.alignment = LineAlignment.TransformZ;
                lrBlue.startWidth = lineWidth;
                lrBlue.endWidth = lineWidth;
                lrBlue.startColor = Color.blue;
                lrBlue.endColor = Color.blue;
                lrBlue.positionCount = 0;

            }
            else if(es.currentSelectedGameObject.GetComponent<Button>() == button[startRed])
            {
                Debug.Log("START RED");
                drawingRed = true;
                current = es.currentSelectedGameObject;
                lrRed = button[startRed].gameObject.AddComponent<LineRenderer>();

                lrRed.material = lineMaterial;
                lrRed.alignment = LineAlignment.TransformZ;
                lrRed.startWidth = lineWidth;
                lrRed.endWidth = lineWidth;
                lrRed.startColor = Color.red;
                lrRed.endColor = Color.red;
                lrRed.positionCount = 0;
            }
        }

        if (drawingBlue)
        {
            float distance = Vector3.Distance(current.transform.position, es.currentSelectedGameObject.transform.position);
            Debug.Log(distance);
            if(current.transform.position.x == es.currentSelectedGameObject.transform.position.x || current.transform.position.y == es.currentSelectedGameObject.transform.position.y)
            {
                if(distance < 2.5f)
                {
                    lrBlue.positionCount++;
                    lrBlue.SetPosition(currentIndex, new Vector3(es.currentSelectedGameObject.transform.position.x, es.currentSelectedGameObject.transform.position.y, 1));
                    current = es.currentSelectedGameObject;
                    es.currentSelectedGameObject.GetComponent<Image>().color = Color.blue;
                    es.currentSelectedGameObject.GetComponent<Button>().interactable = false;
                    currentIndex++;
                    audioSource.PlayOneShot(buttonSFX);

                    if (current.GetComponent<Button>() == button[endBlue])
                    {
                        drawingBlue = false;
                        Debug.Log("Blue Finish");
                        currentIndex = 0;

                        blueDone = true;

                        if (redDone)
                        {
                            Finish();
                        }
                    }
                }
                
            }
        }

        if (drawingRed)
        {
            if ((current.transform.position.x == es.currentSelectedGameObject.transform.position.x || current.transform.position.y == es.currentSelectedGameObject.transform.position.y))
            {
                lrRed.positionCount++;
                lrRed.SetPosition(currentIndex, new Vector3(es.currentSelectedGameObject.transform.position.x, es.currentSelectedGameObject.transform.position.y, 1));
                current = es.currentSelectedGameObject;
                es.currentSelectedGameObject.GetComponent<Image>().color = Color.red;
                es.currentSelectedGameObject.GetComponent<Button>().interactable = false;
                currentIndex++;
                audioSource.PlayOneShot(buttonSFX);

                if (current.GetComponent<Button>() == button[endRed])
                {
                    drawingRed = false;
                    currentIndex = 0;
                }

                if (current.GetComponent<Button>() == button[endRed])
                {
                    drawingRed = false;
                    Debug.Log("Red Finish");
                    currentIndex = 0;

                    redDone = true;

                    if (blueDone)
                    {
                        Finish();
                    }
                }
            }
        }
    }

    public void setRandomStart()
    {
        for(int i = 0; i < button.Length; i++)
        {
            button[i].GetComponent<Image>().color = Color.white;
            button[i].GetComponent<Button>().interactable = true;
        }

        startBlue = 0;
        startRed = button.Length - 1;

        if (!isRandom)
        {
            endBlue = Random.Range(1, button.Length - 9);

            while(endBlue == 5 || endBlue == 2)
            {
                endBlue = Random.Range(1, button.Length - 9);
            }

            endRed = Random.Range(9, button.Length - 2);

            while (endRed == 10 || endRed == 13)
            {
                endRed = Random.Range(9, button.Length - 2);
            }
            isRandom = true;
        }
       
        button[startBlue].GetComponent<Image>().color = Color.blue;
        button[endBlue].GetComponent<Image>().color = Color.blue;
        button[startRed].GetComponent<Image>().color = Color.red;
        button[endRed].GetComponent<Image>().color = Color.red;

        Debug.Log(startBlue + " " + endBlue + " " + startRed + " " + endRed);
    }

    public void Reset()
    {
        Destroy(lrBlue);
        Destroy(lrRed);
        drawingBlue = false;
        drawingRed = false;
        setRandomStart();
        currentIndex = 0;
    }

    public void Finish()
    {
        camera.transform.rotation = Quaternion.Euler(45, 0, 0);
        mgc.endMiniGame();
    }
}
