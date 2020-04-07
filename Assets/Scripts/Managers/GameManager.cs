using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Paneles")] public GameObject BuyPanel;
    public GameObject EndOfDay;
    public GameObject ArrestPanel;
    public GameObject GameOverPanel;
    public GameObject CharityPanel;
    public GameObject InteractPanel;
    public GameObject PausePanel;

    [Header("Texto")] public TextMeshProUGUI timeText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI empanadasCountText;
    public TextMeshProUGUI arepasCountText;
    public TextMeshProUGUI TintoCountText;
    public TextMeshProUGUI InteractText;

    [Header("GameOver")] public TextMeshProUGUI gameOverText;
    
    [Header("GamePlay")] public float time;
    public Image FamilyHealthBar;
    public Transform[] MoveSpots;

    [Header("Misc")] public LevelChanger LevelChanger;

    [HideInInspector] public static int EMPANADA = 0;
    [HideInInspector] public static int TINTO = 1;
    [HideInInspector] public static int AREPA = 2;

    #region Singleton

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        //Debug.Log("start gamemanager");
        GameControl.instance.checkCharity();

        if (GameControl.instance.Charity)
        {
            SoundManager.instance.PlayBackground(SoundManager.instance.sadBackground);
        }
        else
        {
            SoundManager.instance.PlayBackground(SoundManager.instance.gameplayBackground);
        }

        HealthFillAmount();
        setDayText();
        setScoreTimer();
        setCashText();
        SetInventoryText();
        StartCoroutine(addTimer());
    }

    IEnumerator addTimer()
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1f);
            time--;
            setScoreTimer();
        }
        SetInteractText("Ya termino el dia, compra lo necesario para tu negocio y familia", true);
        nextLevel(false);
    }

    private void setScoreTimer()
    {
        timeText.text = string.Format("{0:0}:{1:00}", Mathf.Floor(time / 60), time % 60);
    }

    public void addCash(int amount)
    {
        GameControl.instance.playerInfo.Money += amount;
        setCashText();
    }

    public void setCashText()
    {
        cashText.text = "$ " + GameControl.instance.playerInfo.Money;
    }

    public void nextLevel(bool eraseText = true)
    {
        if (eraseText)
        {
            SetInteractText("", false);
        }
        if (GameControl.instance.playerInfo.Health <= 0)
        {
            GameOVer();
        }
        else
        {
            setFamilyHungry(35);
            GameControl.instance.nextLevel();
            setDayText();
            if (GameControl.instance.playerInfo.Day > 1)
            {
                StopGame();
                EndOfDay.SetActive(true);
            }
        }
    }

    private void setDayText()
    {
        dayText.text = "Dia: " + GameControl.instance.playerInfo.Day;
    }

    public void setFamilyHungry(float amount)
    {
        GameControl.instance.playerInfo.Health -= amount;
        HealthFillAmount();
    }

    public void HealthFillAmount()
    {
        FamilyHealthBar.fillAmount =
            GameControl.instance.playerInfo.Health / GameControl.instance.playerInfo.StartingHealth;
    }

    public void BuyItem(Items item)
    {
        if (GameControl.instance.playerInfo.Money < item.buyingPrice)
        {
            SoundManager.instance.PlaySound("no");
        }
        else if (item.isInventory)
        {
            SoundManager.instance.PlaySound("sell");
            GameControl.instance.addMoney(-item.buyingPrice);
            setCashText();
            GameControl.instance.addItemToInventory(item);
            SetInventoryText();
        }
        else if (item.canEat)
        {
            if (GameControl.instance.playerInfo.Health < GameControl.instance.playerInfo.StartingHealth)
            {
                SoundManager.instance.PlaySound("sell");
                GameControl.instance.addMoney(-item.buyingPrice);
                setCashText();
                GameControl.instance.playerInfo.Health = Mathf.Clamp(
                    GameControl.instance.playerInfo.Health + item.energy, 0,
                    GameControl.instance.playerInfo.StartingHealth);
                HealthFillAmount();
            }
            else
            {
                SoundManager.instance.PlaySound("no");
            }
        }
    }

    public void SellItem(Items item)
    {
        SoundManager.instance.PlaySound("sell");
        GameControl.instance.addMoney(item.sellingPrice);
        setCashText();
        GameControl.instance.removeItemFromInventory(item);
        SetInventoryText();
    }

    private void SetInventoryText()
    {
        empanadasCountText.text = "" + GameControl.instance.CountItemByInventoryCode(EMPANADA);
        arepasCountText.text = "" + GameControl.instance.CountItemByInventoryCode(AREPA);
        TintoCountText.text = "" + GameControl.instance.CountItemByInventoryCode(TINTO);
    }

    public void GameOVer()
    {
        SoundManager.instance.PlayBackground(SoundManager.instance.sadBackground);
        GameOverPanel.SetActive(true);
        gameOverText.text = "llegaste hasta el dia "+GameControl.instance.playerInfo.Day;
        StopGame();
    }

    public void PauseGame()
    {
        PausePanel.SetActive(true);
        SoundManager.instance.volumeBackground(0.1f);
        StopGame();
    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        SoundManager.instance.volumeBackground(0.3f);
        StartGame();
    }
    
    public void HomeScreen()
    {
        StartGame();
        LevelChanger.FadeToLevel(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    public void StopGame()
    {
        Time.timeScale = 0;
    }

    public void ClearInventory()
    {
        GameControl.instance.ClearInventory();
        SetInventoryText();
    }

    public void StartLevel()
    {
        StartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetInteractText(String text, bool state, int time = 0)
    {
        InteractPanel.SetActive(state);
        InteractText.text = text;
        InteractText.gameObject.SetActive(state);

        if (!state)
        {
            StopCoroutine("CleanInteractText");
        }

        if (time > 0)
        {
            StartCoroutine(CleanInteractText(time));
        }
    }

    IEnumerator CleanInteractText(int time)
    {
        //Debug.Log("Esperando para limpiar el mensaje");
        yield return new WaitForSeconds(time);
        //Debug.Log("Listo");
        SetInteractText("", false);
    }
}