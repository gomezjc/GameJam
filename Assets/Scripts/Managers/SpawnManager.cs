using System.Collections.Generic;
using Characters;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public GameObject player;
    public GameObject Police;
    public GameObject People;
    public List<Transform> SpawnPointsPolice;
    public List<Transform> SpawnPointsPeople;

    private Items[] itemsToPick;
    private float SpawnTimePolice = 30f;
    private float SpawnTimePeople = 5f;
    private int PercentageWantToBuy = 60;

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

    private void Start()
    {
        itemsToPick = new Items[3];
        itemsToPick[0] = GameControl.instance.arepa;
        itemsToPick[1] = GameControl.instance.empanada;
        itemsToPick[2] = GameControl.instance.tinto;
        InvokeRepeating("SpawnPolice", 0, SpawnTimePolice);
        InvokeRepeating("SpawnPeople", 0, SpawnTimePeople);
    }

    public void stopSpawn()
    {
        CancelInvoke("SpawnPolice");
        CancelInvoke("SpawnPeople");
    }

    void SpawnPolice()
    {
        int spawnPointIndex = Random.Range(0, SpawnPointsPolice.Count);
        GameObject police = Instantiate(Police, SpawnPointsPolice[spawnPointIndex].position,
            SpawnPointsPolice[spawnPointIndex].rotation);

        // set variables of police
        Patrol policePatrol = police.GetComponent<Patrol>();
        policePatrol.moveSpots = GameManager.instance.MoveSpots;
        policePatrol.playerTarget = player.transform;
        policePatrol.StartPatrol();
    }

    void SpawnPeople()
    {
        int spawnPointIndex = Random.Range(0, SpawnPointsPeople.Count);
        GameObject people = Instantiate(People, SpawnPointsPeople[spawnPointIndex].position,
            SpawnPointsPeople[spawnPointIndex].rotation);

        // set variables of people
        PatrolPeople peoplePatrol = people.GetComponent<PatrolPeople>();
        InteractBuy interactBuy = people.GetComponentInChildren<InteractBuy>();
        interactBuy.setItemWantToBuy(itemsToPick[Random.Range(0, itemsToPick.Length)]);
        
        if (!GameControl.instance.Charity && !WantBuy())
        {
            interactBuy.WantBuy(false);
        } else if (GameControl.instance.Charity)
        {
            interactBuy.WantBuy(false);
            interactBuy.CanBuy = true;
        }

        peoplePatrol.moveSpots = GameManager.instance.MoveSpots;
        peoplePatrol.startPatrol();
    }

    private bool WantBuy()
    {
        int random = Random.Range(0, 100);
        return random <= PercentageWantToBuy;
    }
}