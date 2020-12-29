using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FixingGame : MonoBehaviour
{
    [SerializeField] private Slider fixingSlider;
    [SerializeField] private float timer;

    [SerializeField] MiniGameController mgc;

    int currentButton;

    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip buttonPressed;
    [SerializeField] AudioClip correctSFX;
    [SerializeField] AudioClip wrongSFX;

    [SerializeField] Button[] button;
    [SerializeField] Button[] clickedButton = new Button[9];
    int clickedButtonIndex = 0;

    bool isBlinking = true;
    bool isCorrect;

    void Start()
    {
        fixingSlider.value = 0;

        if(currentButton != 9)
        {
            shuffleButton();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Fixing();
    }

    void Fixing()
    {
        fixingSlider.value += 2 * Time.deltaTime;

        if(timer<= 1f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;

            if (isBlinking)
            {
                randomBlinking();
            }
            
        }
    }

    void randomBlinking()
    {
        if(currentButton == button.Length)
        {
            for (int posArray = 0; posArray < button.Length; posArray++)
            {
                button[posArray].interactable = true;
                button[posArray].image.color = Color.white;
                audioSrc.PlayOneShot(buttonPressed);
            }

            isBlinking = false;
            return;
        }

        button[currentButton].image.color = Color.red;
        audioSrc.PlayOneShot(buttonPressed);
        currentButton++;
    }

    void shuffleButton()
    {
        for (int posArray = 0; posArray < button.Length; posArray++)
        {
            Button btn = button[posArray];
            int random = Random.Range(0, posArray);
            button[posArray] = button[random];
            button[random] = btn;
        }
    }

    public void changeColor()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Debug.Log(btn);
        btn.image.color = Color.blue;
        audioSrc.PlayOneShot(buttonPressed);
        btn.interactable = false;
        clickedButton[clickedButtonIndex] = btn;
        clickedButtonIndex++;

        if(clickedButtonIndex == clickedButton.Length)
        {
            checkAnswer();
        }
    }

    void checkAnswer()
    {
        for (int posArray = 0; posArray < button.Length; posArray++)
        {
            if (button[posArray] != clickedButton[posArray])
            {
                isCorrect = false;
                break;
            }
            else
            {
                isCorrect = true;
            }
        }

        if (isCorrect)
        {
            audioSrc.PlayOneShot(correctSFX);
            for (int posArray = 0; posArray < button.Length; posArray++)
            {
                button[posArray].image.color = Color.white;
                clickedButton[posArray] = null;
            }
            currentButton = 0;
            //shuffleButton();
            isBlinking = true;
            clickedButtonIndex = 0;
            mgc.endMiniGame();
        }
        else
        {
            audioSrc.PlayOneShot(wrongSFX);
            for (int posArray = 0; posArray < button.Length; posArray++)
            {
                button[posArray].image.color = Color.white;
                clickedButton[posArray] = null;
            }
            currentButton = 0;
            isBlinking = true;
            clickedButtonIndex = 0;
            Debug.Log("WRONG!");
        }
    }
}
