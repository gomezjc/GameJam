using System;
using Characters;
using TMPro;
using UnityEngine;

public class InteractScript : MonoBehaviour
{
    private PatrolPeople people;
    private InteractBuy peopleInteract;
    private bool isTalking;

    public GameObject interactText;

    public GameObject BuyButton;
    public GameObject CloseButton;
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
                interactText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Person"))
        {
            resetBehaviour();
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
        interactText.SetActive(false);
    }
    
}