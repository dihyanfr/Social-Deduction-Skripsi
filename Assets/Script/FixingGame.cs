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
    int currentIndex = 1;

    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip buttonPressed;
    [SerializeField] AudioClip correctSFX;
    [SerializeField] AudioClip wrongSFX;

    [SerializeField] Button[] button;
    [SerializeField] Button[] clickedButton = new Button[9];
    int clickedButtonIndex = 0;

    bool isBlinking = true;
    bool isCorrect;

    float speedBlinking;

    void Start()
    {
        fixingSlider.value = 0;

        if(currentButton != 9)
        {
            shuffleButton();
        }

        currentIndex = 1;

    }

    private void OnEnable()
    {
        fixingSlider.value = 0;

        if (currentButton != 9)
        {
            shuffleButton();
        }

        currentIndex = 1;
        speedBlinking = 0;
        currentIndex = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Fixing();
    }

    void Fixing()
    {
        fixingSlider.value += speedBlinking * Time.deltaTime;

        if(timer<= 1f)
        {
            timer += Time.deltaTime + speedBlinking;
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
        if (currentButton == currentIndex)
        {
            for (int posArray = 0; posArray < button.Length; posArray++)
            {
                button[posArray].interactable = true;
                button[posArray].image.color = Color.white;
                //audioSrc.PlayOneShot(buttonPressed);
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

        if(clickedButtonIndex == currentIndex)//clickedButton.Length)
        {
            Debug.Log("current index" + currentIndex);
            //checkAnswer();
            StartCoroutine(CheckAnswer());
        }
    }

    void checkAnswer()
    {
        for (int posArray = 0; posArray != currentIndex; posArray++)
        {
            Debug.Log("current index" + currentIndex);
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
            currentIndex++;
            audioSrc.PlayOneShot(correctSFX);
            for (int posArray = 0; posArray < button.Length; posArray++)
            {
                button[posArray].image.color = Color.white;
                clickedButton[posArray] = null;
                button[posArray].interactable = false;
            }
            currentButton = 0;
            //shuffleButton();
            isBlinking = true;
            clickedButtonIndex = 0;
            Debug.Log("Correct");
            if(currentIndex - 1 == button.Length)
            {
                mgc.endMiniGame();
                return;
            }
            speedBlinking += 0.005f;
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
    
    IEnumerator CheckAnswer()
    {
        yield return new WaitForSeconds(0.5f);

        for (int posArray = 0; posArray != currentIndex; posArray++)
        {
            Debug.Log("current index" + currentIndex);
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
            currentIndex++;
            audioSrc.PlayOneShot(correctSFX);
            for (int posArray = 0; posArray < button.Length; posArray++)
            {
                button[posArray].image.color = Color.white;
                clickedButton[posArray] = null;
                button[posArray].interactable = false;
            }
            currentButton = 0;
            //shuffleButton();
            isBlinking = true;
            clickedButtonIndex = 0;
            Debug.Log("Correct");
            if (currentIndex - 1 == button.Length)
            {
                mgc.endMiniGame();
                yield return new WaitForSeconds(1);
            }
            speedBlinking += 0.005f;
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
