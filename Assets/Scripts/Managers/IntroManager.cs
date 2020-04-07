using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{

    public GameObject panelStart;
    public GameObject panelCredits;
    // Start is called before the first frame update

    public LevelChanger LevelChanger;

    private void Start()
    {
        if (GameControl.instance != null)
        {
            Destroy(GameControl.instance.gameObject);
        }
    }

    public void StartPanelIntro()
    {
        panelCredits.SetActive(false);
        panelStart.SetActive(true);
    }

    public void offPanels()
    {
        panelCredits.SetActive(false);
        panelStart.SetActive(false);
    }
    
    public void StartGame()
    {
        LevelChanger.FadeToLevel(1);
    }

    public void ShowCredits()
    {
        panelCredits.SetActive(true);
        panelStart.SetActive(false);
    }

    public void HowToPlay()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
