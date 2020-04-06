using System;
using System.Collections;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InteractScript : MonoBehaviour
{
    private PatrolPeople people;
    private Patrol enemy;
    private InteractBuy peopleInteract;
    private InteractArrest enemyArrest;
    private bool isTalking;

    [Header("Global")] public GameObject interactPanel;
    public TextMeshProUGUI interactText;
    public Sprite[] images;

    [Header("Buy Panel")] public TextMeshProUGUI itemText;
    public GameObject BuyButton;
    public GameObject CloseButton;
    public Image imageBuy;

    [Header("Arrest Panel")] 
    public GameObject BirbeButton;
    public GameObject ArrestButton;
    public TextMeshProUGUI BirbeText;
    public Sprite[] imagesCopBirbe;
    public Image imageArrest;

    [Header("Charity Panel")] public TextMeshProUGUI charityText;
    public TextMeshProUGUI ItemCharityText;
    public int charityValue = 0;
    public Image imageCharity;

    private String[] RandomBuyWords =
    {
        "Socio, deme _PRODUCTO_",
        "Que gorobeta, vendame _PRODUCTO_",
        "Regaleme _PRODUCTO_",
        "Socito, le queda _PRODUCTO_",
        "Parcero, vendame _PRODUCTO_, todo bien",
        "A mi se me antoja _PRODUCTO_ sumerce",
        "Oiga mano, deme _PRODUCTO_ si es tan amable"
    };
    
    private void Start()
    {
        if (GameControl.instance.Charity)
        {
            SetInteractText(
                "No tienes productos para vender, debes recurrir a la caridad, es dificil pero al menos la policia no te persigue.",
                true,true);
        }
        else
        {
            SetInteractText(
                "Presiona la tecla 'Espacio' para correr, OJO con la tomba",
                true,true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTalking)
        {
            if (GameControl.instance.Charity)
            {
                openCharityDialog();
            }
            else
            {
                openBuyDialog();
            }
        }
    }

    private void openCharityDialog()
    {
        if (people != null && peopleInteract != null)
        {
            ItemCharityText.text = "Por favor, me regala una monedita... cualquier cosita es cariño";
            charityValue = peopleInteract.giveCharity();
            if (charityValue > 0)
            {
                charityText.text = "Tome :" + charityValue.ToString();
            }
            else
            {
                charityText.text = "No tengo";
            }

            isTalking = false;
            imageCharity.sprite = images[Random.Range(0, images.Length)];
            SetInteractText("", false);
            GameManager.instance.StopGame();
            GameManager.instance.CharityPanel.SetActive(true);
        }
    }

    public void ReceiveCharity()
    {
        GameControl.instance.addMoney(charityValue);
        peopleInteract.WantBuy(false);
        GameManager.instance.setCashText();
        GameManager.instance.StartGame();
        GameManager.instance.CharityPanel.SetActive(false);
        resetBehaviour();
    }

    private void openBuyDialog()
    {
        if (people != null && peopleInteract != null)
        {
            String article = peopleInteract.itemWantToBuy.inventoryCode == GameManager.TINTO ? "un" : "una";
            itemText.text = RandomBuyWords[Random.Range(0,RandomBuyWords.Length)].Replace("_PRODUCTO_",
                article +" "+ peopleInteract.itemWantToBuy.name);
            if (GameControl.instance.hasItemInventoryByItem(peopleInteract.itemWantToBuy))
            {
                BuyButton.SetActive(true);
                CloseButton.SetActive(false);
            }
            else
            {
                BuyButton.SetActive(false);
                CloseButton.SetActive(true);
            }

            isTalking = false;
            imageBuy.sprite = images[Random.Range(0, images.Length)];
            SetInteractText("", false);
            GameManager.instance.StopGame();
            GameManager.instance.BuyPanel.SetActive(true);
        }
    }

    private void openArrestDialog()
    {
        if (enemy != null && enemyArrest != null)
        {
            if (enemy.canFollow)
            {
                if (GameControl.instance.playerInfo.Money < 10000)
                {
                    BirbeText.text = "No tienes suficiente dinero para sobornar";
                }

                imageArrest.sprite = imagesCopBirbe[Random.Range(0, imagesCopBirbe.Length)];
                GameManager.instance.StopGame();
                GameManager.instance.ArrestPanel.SetActive(true);
            }
        }
    }

    public void BribeEnemy()
    {
        if (enemy != null && enemyArrest != null)
        {
            if (GameControl.instance.playerInfo.Money >= 10000)
            {
                if (enemyArrest.BribeEnemy())
                {
                    SetInteractText(
                        "Soborno aceptado, sigue trabajando... este policia no te molestara mas",
                        true,true);
                    GameManager.instance.addCash(-10000);
                    GameManager.instance.ArrestPanel.SetActive(false);
                    enemy.canFollow = false;
                    resetBehaviourEnemy();
                    GameManager.instance.StartGame();
                }
                else
                {
                    SetInteractText(
                        "Soborno no aceptado. Ahora no tienes productos y pasas el dia en la UPJ",
                        true);
                    GameManager.instance.ArrestPanel.SetActive(false);
                    GameManager.instance.ClearInventory();
                    GameManager.instance.nextLevel();
                }
            }
        }
    }

    public void getArrested()
    {
        SetInteractText(
            "Ahora no tienes productos y pasas el dia en la UPJ",
            true);
        GameManager.instance.ArrestPanel.SetActive(false);
        GameManager.instance.ClearInventory();
        GameManager.instance.nextLevel();
    }

    public void BuyItem()
    {
        if (people != null)
        {
            if (GameControl.instance.hasItemInventoryByItem(peopleInteract.itemWantToBuy))
            {
                GameManager.instance.SellItem(peopleInteract.itemWantToBuy);
                GameManager.instance.StartGame();
            }

            peopleInteract.WantBuy(false);
            GameManager.instance.BuyPanel.SetActive(false);
            resetBehaviour();
        }
    }

    public void CloseDialog()
    {
        if (people != null)
        {
            GameManager.instance.StartGame();
            GameManager.instance.BuyPanel.SetActive(false);
            peopleInteract.WantBuy(false);
            resetBehaviour();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Person"))
        {
            people = other.GetComponentInParent<PatrolPeople>();
            peopleInteract = other.GetComponentInParent<InteractBuy>();
            people.stopPath();
            if (peopleInteract.CanBuy)
            {
                isTalking = true;
                SetInteractText("Presiona 'E' para interactuar", true);
            }
            else
            {
                SetInteractText("Este no tiene hambre", true);
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            enemy = other.GetComponentInParent<Patrol>();
            if (enemy.canFollow)
            {
                enemyArrest = other.GetComponent<InteractArrest>();
                openArrestDialog();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Person"))
        {
            resetBehaviour();
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            resetBehaviourEnemy();
        }
    }

    void resetBehaviour()
    {
        if (people != null && peopleInteract != null)
        {
            people.continuePath();
            people = null;
            peopleInteract = null;
        }

        charityValue = 0;
        isTalking = false;
        SetInteractText("", false);
    }

    void resetBehaviourEnemy()
    {
        if (enemy != null && enemyArrest != null)
        {
            enemy.cancelFollow();
            enemy = null;
            enemyArrest = null;
        }
    }

    private void SetInteractText(String text, bool state, bool withTime=false)
    {
        interactPanel.SetActive(state);
        interactText.text = text;
        interactText.gameObject.SetActive(state);
        if (withTime)
        {
            StartCoroutine(CleanInteractText());
        }
    }
    
    IEnumerator CleanInteractText()
    {
        //Debug.Log("Esperando para limpiar el mensaje");
        yield return new WaitForSeconds(5);
       // Debug.Log("Listo");
        SetInteractText("",false);
    }
}