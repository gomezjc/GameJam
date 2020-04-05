using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items",menuName = "Cositas")]
public class Items : ScriptableObject
{
    public new string name;
    public int sellingPrice;
    public int buyingPrice;
    public Sprite icon;
    public string cantBuyMessage;
    public bool isInventory;
    public int inventoryCode;
    public bool canEat;
    public float energy;
}
