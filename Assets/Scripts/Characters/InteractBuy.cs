using UnityEngine;

public class InteractBuy : MonoBehaviour
{
    public Items itemWantToBuy;
    public bool CanBuy;

    private int SuccessPercentage = 99;
    private int[] charityValues = {50, 100, 200, 300, 500, 1000};
    
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
