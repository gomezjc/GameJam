using System;
using Characters;
using TMPro;
using UnityEngine;

public class InteractScript : MonoBehaviour
{
    private PatrolPeople people;
    private InteractBuy peopleInteract;
    private InteractArrest enemyArrest;
    private bool isTalking;

    public TextMeshProUGUI interactText;

    public GameObject BuyButton;
    public GameObject CloseButton;

    public GameObject BirbeButton;
    public GameObject ArrestButton;
    
    public TextMeshProUGUI itemText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTalking)
        {
            openBuyDialog();
        }
    }

    private void openBuyDialog()
    {
        if (people != null)
        {
            itemText.text = "Tiene: " + peopleInteract.itemWantToBuy.name;
            if (checkInventory())
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
        if (enemyArrest != null)
        {
            if (GameManager.instance.cash < 10000)
            {
                BirbeButton.SetActive(false);
            }
            GameManager.instance.StopGame();
            GameManager.instance.ArrestPanel.SetActive(true);
        }
    }

    public void BribeEnemy()
    {
        if (enemyArrest != null)
        {
            if (enemyArrest.BribeEnemy())
            {
                GameManager.instance.addCash(-10000);
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
            if (checkInventory())
            {
                GameManager.instance.SellItem(peopleInteract.itemWantToBuy);
                GameManager.instance.StartGame();
            }

            peopleInteract.CanBuy = false;
            GameManager.instance.BuyPanel.SetActive(false);
            resetBehaviour();
        }
    }

    private bool checkInventory()
    {
        int countInventory = GameManager.instance.inventory
            .FindAll(x => x.inventoryCode == peopleInteract.itemWantToBuy.inventoryCode).Count;

        return (countInventory >= 1);
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
            enemyArrest = other.GetComponent<InteractArrest>();
            openArrestDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Person"))
        {
            resetBehaviour();
        }else if (other.gameObject.CompareTag("Enemy"))
        {

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
        isTalking = false;
        SetInteractText("",false);
    }
    
    void resetBehaviourEnemy()
    {
        if (people != null && peopleInteract != null)
        {
            people.continuePath();
            people = null;
            peopleInteract = null;
        }
        isTalking = false;
        SetInteractText("",false);
    }

    private void SetInteractText(String text, bool state)
    {
        interactText.text = text;
        interactText.gameObject.SetActive(state);
    }
}