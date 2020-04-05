using System;
using Characters;
using TMPro;
using UnityEngine;

public class InteractScript : MonoBehaviour
{
    private PatrolPeople people;
    private Patrol enemy;
    private InteractBuy peopleInteract;
    private InteractArrest enemyArrest;
    private bool isTalking;

    [Header("Global")]
    public TextMeshProUGUI interactText;

    [Header("Buy Panel")]
    public TextMeshProUGUI itemText;
    public GameObject BuyButton;
    public GameObject CloseButton;

    [Header("Arrest Panel")]
    public GameObject BirbeButton;
    public GameObject ArrestButton;

    [Header("Charity Panel")] 
    public TextMeshProUGUI charityText;
    public int charityValue = 0;

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
            itemText.text = "Me regala una moneda joven";
            charityValue = peopleInteract.giveCharity();
            if (charityValue > 0)
            {
                charityText.text = "Tome :" + charityValue.ToString();
            }
            else
            {
                charityText.text = "No tengo mi perro";
            }
            SetInteractText("",false); 
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
                GameManager.instance.addCash(-10000);
                GameManager.instance.ArrestPanel.SetActive(false);
                enemy.canFollow = false;
                resetBehaviourEnemy();
                GameManager.instance.StartGame();
            }
            else
            {
                getArrested();
            }
        }
    }

    public void getArrested()
    {
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
                SetInteractText("Press 'E' to interact",true);
            }
        }else if (other.gameObject.CompareTag("Enemy"))
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
        }else if (other.gameObject.CompareTag("Enemy"))
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
        SetInteractText("",false);
    }
    
    void resetBehaviourEnemy()
    {
        if (enemy != null && enemyArrest != null)
        {
            enemy.cancelFollow();
            enemy = null;
            enemyArrest = null;
        }
        SetInteractText("",false);
    }

    private void SetInteractText(String text, bool state)
    {
        interactText.text = text;
        interactText.gameObject.SetActive(state);
    }
}