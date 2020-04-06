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
    
    private float SpawnTimePolice = 60f;
    private float SpawnTimePeople = 10f;
    
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
        peoplePatrol.moveSpots = GameManager.instance.MoveSpots;
        peoplePatrol.startPatrol();
    }
}