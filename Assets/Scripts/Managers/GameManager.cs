using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Paneles")] 
    public GameObject BuyPanel;
    public GameObject EndOfDay;

    [Header("Texto")] public TextMeshProUGUI timeText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI empanadasCountText;
    public TextMeshProUGUI arepasCountText;
    public TextMeshProUGUI TintoCountText;

    [Header("Gameplay")] public float time;
    public int day = 0;
    public int cash;

    [Header("GamePlay")] public float StartingHealth;
    public float CurrentHealth;
    public Image FamilyHealthBar;

    [HideInInspector] public static int EMPANADA = 0;
    [HideInInspector] public static int TINTO = 1;
    [HideInInspector] public static int AREPA = 2;

    [HideInInspector] public List<Items> inventory;

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
        inventory = new List<Items>();

        CurrentHealth = StartingHealth;

        // OJO BORRAR!
        setFamilyHungry(30);

        nextLevel();
        setScoreTimer();
        setCashText();
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
    }

    private void setScoreTimer()
    {
        timeText.text = string.Format("{0:0}:{1:00}", Mathf.Floor(time / 60), time % 60);
    }

    public void addCash(int amount)
    {
        cash += amount;
        setCashText();
    }

    private void setCashText()
    {
        cashText.text = "$ " + cash;
    }

    public void nextLevel()
    {
        day++;
        dayText.text = "Dia: " + day;
    }

    public void setFamilyHungry(float amount)
    {
        CurrentHealth -= amount;
        FamilyHealthBar.fillAmount = CurrentHealth / StartingHealth;
    }

    public void BuyItem(Items item)
    {
        if (cash < item.buyingPrice)
        {
            Debug.Log("cant buy");
        }
        else if (item.isInventory)
        {
            cash -= item.buyingPrice;
            setCashText();
            inventory.Add(item);
            SetInventoryText();
        }
        else if (item.canEat)
        {
            if (CurrentHealth < StartingHealth)
            {
                cash -= item.buyingPrice;
                setCashText();
                CurrentHealth = Mathf.Clamp(CurrentHealth + item.energy, 0, StartingHealth);
                FamilyHealthBar.fillAmount = CurrentHealth / StartingHealth;
            }
            else
            {
                Debug.Log("dont need");
            }
        }
    }

    private void SetInventoryText()
    {
        empanadasCountText.text = "Empanadas: " + inventory.FindAll(x => x.inventoryCode == EMPANADA).Count;
        arepasCountText.text = "Arepas: " + inventory.FindAll(x => x.inventoryCode == AREPA).Count;
        TintoCountText.text = "Tintos: " + inventory.FindAll(x => x.inventoryCode == TINTO).Count;
    }
}