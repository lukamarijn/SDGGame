using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    public Text panelText;
    
    void Start()
    {
        panel.gameObject.SetActive(false);
    }
    
    
    // Start is called before the first frame update
    public void SetLoseScreen()
    {
        panel.gameObject.SetActive(true);
        panelText.text = "Je hebt verloren...";

    }

    public void SetWinScreen()
    {
        panel.gameObject.SetActive(true);
        panelText.text = "Je hebt gewonnen!";
    }
}
