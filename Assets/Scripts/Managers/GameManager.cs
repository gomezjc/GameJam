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

    [Header("Texto")] public TextMeshProUGUI timeText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI empanadasCountText;
    public TextMeshProUGUI arepasCountText;
    public TextMeshProUGUI TintoCountText;
    
    [Header("GamePlay")] 
    public float time;
    public Image FamilyHealthBar;
    public Transform[] MoveSpots;
    
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
        Debug.Log("start gamemanager");
        GameControl.instance.checkCharity();
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
        nextLevel();
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

    public void nextLevel()
    {
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
            Debug.Log("cant buy");
        }
        else if (item.isInventory)
        {
            GameControl.instance.addMoney(-item.buyingPrice);
            setCashText();
            GameControl.instance.addItemToInventory(item);
            SetInventoryText();
        }
        else if (item.canEat)
        {
            if (GameControl.instance.playerInfo.Health < GameControl.instance.playerInfo.StartingHealth)
            {
                GameControl.instance.addMoney(-item.buyingPrice);
                setCashText();
                GameControl.instance.playerInfo.Health = Mathf.Clamp(
                    GameControl.instance.playerInfo.Health + item.energy, 0,
                    GameControl.instance.playerInfo.StartingHealth);
                HealthFillAmount();
            }
            else
            {
                Debug.Log("dont need");
            }
        }
    }

    public void SellItem(Items item)
    {
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
        GameOverPanel.SetActive(true);
        StopGame();
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
    }

    public void StartLevel()
    {
        StartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}