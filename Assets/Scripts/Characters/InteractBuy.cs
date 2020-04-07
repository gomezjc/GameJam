using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InteractBuy : MonoBehaviour
{
    public Items itemWantToBuy;
    public bool CanBuy;
    public Image imageWantToBuy;
    public GameObject imagePanel;

    private int SuccessPercentage = 50;
    private int[] charityValues = {50, 100, 200, 300, 500, 1000};

    public void WantBuy(bool wantBuy)
    {
        CanBuy = wantBuy;
        imagePanel.SetActive(wantBuy);
    }
    public void setItemWantToBuy(Items item)
    {
        itemWantToBuy = item;
        imageWantToBuy.sprite = itemWantToBuy.icon;
    }

    public int giveCharity()
    {
        int charityValue = 0;
        if (canGiveCharity())
        {
            charityValue = charityValues[Random.Range(0, charityValues.Length)];
        }

        return charityValue;
    }
    
    
    private bool canGiveCharity()
    {
        int random = Random.Range(0, 100);
        return random <= SuccessPercentage;
    }
}
