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
    public Sprite[] imagesCopBirbe;
    public Image imageArrest;

    [Header("Charity Panel")] public TextMeshProUGUI charityText;
    public TextMeshProUGUI ItemCharityText;
    public int charityValue = 0;
    public Image imageCharity;

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
                "Presiona la tecla 'Espacio' para correr, OJO con la policia",
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
            ItemCharityText.text = "Por favor, me regala una monedita";
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
        peopleInteract.CanBuy = false;
        GameManager.instance.setCashText();
        GameManager.instance.StartGame();
        GameManager.instance.CharityPanel.SetActive(false);
        resetBehaviour();
    }

    private void openBuyDialog()
    {
        if (people != null)
        {
            itemText.text = "Tiene: " + peopleInteract.itemWantToBuy.name;
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
                    BirbeButton.SetActive(false);
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

            peopleInteract.CanBuy = false;
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
            peopleInteract.CanBuy = false;
            resetBehaviour();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Person"))
        {
            peopleInteract = other.GetComponentInParent<InteractBuy>();
            if (peopleInteract.CanBuy)
            {
                people = other.GetComponentInParent<PatrolPeople>();
                people.stopPath();
                isTalking = true;
                SetInteractText("Press 'E' to interact", true);
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
        Debug.Log("Esperando para limpiar el mensaje");
        yield return new WaitForSeconds(5);
        Debug.Log("Listo");
        SetInteractText("",false);
    }
}