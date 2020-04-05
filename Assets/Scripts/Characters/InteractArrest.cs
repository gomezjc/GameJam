using UnityEngine;

public class InteractArrest : MonoBehaviour
{
    private int SuccessPercentage = 90;

    public bool BribeEnemy()
    {
        int random = Random.Range(0, 100);
        return random <= SuccessPercentage;
    }
    
}