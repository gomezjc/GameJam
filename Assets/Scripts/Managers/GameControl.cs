using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public static GameControl instance;

    public bool Charity = false;

    public Items empanada;
    public Items tinto;
    public Items arepa;

    public PlayerData playerInfo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("start Gamecontrol");
        playerInfo.Health = playerInfo.StartingHealth;
        playerInfo.Money = 10000;
        playerInfo.Inventory.Add(instance.arepa);
        playerInfo.Inventory.Add(instance.arepa);
        playerInfo.Inventory.Add(instance.arepa);
        playerInfo.Inventory.Add(instance.empanada);
        playerInfo.Inventory.Add(instance.empanada);
        playerInfo.Inventory.Add(instance.empanada);
        playerInfo.Inventory.Add(instance.tinto);
        playerInfo.Inventory.Add(instance.tinto);
        playerInfo.Inventory.Add(instance.tinto);
    }

    [System.Serializable]
    public class PlayerData
    {
        public int Day;
        public int Money;
        public float StartingHealth = 100;
        public float Health;
        public List<Items> Inventory = new List<Items>();
    }

    public void addMoney(int money)
    {
        playerInfo.Money += money;
    }

    public void nextLevel()
    {
        playerInfo.Day += 1;
    }

    public bool hasItemInventoryByItem(Items item)
    {
        int countInventory = playerInfo.Inventory
            .FindAll(x => x.inventoryCode == item.inventoryCode).Count;

        return (countInventory >= 1);
    }

    public int CountItemByInventoryCode(int inventoryCode)
    {
        int countInventory = playerInfo.Inventory
            .FindAll(x => x.inventoryCode == inventoryCode).Count;

        return countInventory;
    }

    public void addItemToInventory(Items item)
    {
        playerInfo.Inventory.Add(item);
    }

    public void removeItemFromInventory(Items item)
    {
        int index = playerInfo.Inventory.FindIndex(x => x.inventoryCode == item.inventoryCode);
        playerInfo.Inventory.RemoveAt(index);
    }

    public void ClearInventory()
    {
        playerInfo.Inventory.Clear();
    }

    public void checkCharity()
    {
        instance.Charity = playerInfo.Inventory.Count == 0;
    }
}