using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CodeGame : MonoBehaviour
{
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;
    [SerializeField] Button button4;
    [SerializeField] Button button5;
    [SerializeField] Button button6;
    [SerializeField] Button button7;
    [SerializeField] Button button8;
    [SerializeField] Button button9;

    [SerializeField] Button buttonC;
    [SerializeField] Button button0;
    [SerializeField] Button buttonE;

    [SerializeField] Text codeText;

    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioClip buttonSFX;
    [SerializeField] AudioClip correctSFX;
    [SerializeField] AudioClip wrongSFX;


    [SerializeField] public string code;

    private MiniGameController mgc;
    [SerializeField] public GameObject mgc_gb;

    [SerializeField] GameObject door;

    bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        codeText.text = "";
        mgc = mgc_gb.GetComponent<MiniGameController>();
    }

    private void OnEnable()
    {
        codeText.text = "";
        mgc = mgc_gb.GetComponent<MiniGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            button1.interactable = false;
            button2.interactable = false;
            button3.interactable = false;
            button4.interactable = false;
            button5.interactable = false;
            button6.interactable = false;
            button7.interactable = false;
            button8.interactable = false;
            button9.interactable = false;
            button0.interactable = false;
            buttonC.interactable = false;
            buttonE.interactable = false;

            codeText.text = "OPEN";
        }
    }

    public void buttonPress()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        audioSrc.PlayOneShot(buttonSFX);

        if(btn.name == "Clear")
        {
            codeText.text = codeText.text.Remove(codeText.text.Length - 1);
        }
        else if(btn.name == "Enter")
        {
            
            if (codeText.text == code)
            {
                audioSrc.PlayOneShot(correctSFX);
                StartCoroutine(delay(0.2f, "OPEN"));
                isOpen = true;
                codeText.text = "OPEN";
                door.SetActive(false);
                button1.interactable = false;
                button2.interactable = false;
                button3.interactable = false;
                button4.interactable = false;
                button5.interactable = false;
                button6.interactable = false;
                button7.interactable = false;
                button8.interactable = false;
                button9.interactable = false;
                button0.interactable = false;
                buttonC.interactable = false;
                buttonE.interactable = false;
            }
            else
            {
                audioSrc.PlayOneShot(wrongSFX);
                StartCoroutine(delay(0.2f, "WRONG"));
            }
        }
        else
        {
            if (codeText.text.Length != 3)
            {
                codeText.text += btn.name;
            }
        }

    }

    IEnumerator delay(float rate, string _text)
    {
        codeText.text = _text;
        yield return new WaitForSeconds(rate);
        codeText.text = "";
        yield return new WaitForSeconds(rate);
        codeText.text = _text;
        yield return new WaitForSeconds(rate);
        codeText.text = "";
        yield return new WaitForSeconds(rate);
        codeText.text = _text;
        yield return new WaitForSeconds(rate);
        codeText.text = "";
        yield return new WaitForSeconds(rate);
        codeText.text = "";

        button1.interactable = true;
        button2.interactable = true;
        button3.interactable = true;
        button4.interactable = true;
        button5.interactable = true;
        button6.interactable = true;
        button7.interactable = true;
        button8.interactable = true;
        button9.interactable = true;
        button0.interactable = true;
        buttonC.interactable = true;
        buttonE.interactable = true;
    }

    public void close()
    {
        //this.gameObject.SetActive(false);
        mgc.endMiniGame();
    }
}
