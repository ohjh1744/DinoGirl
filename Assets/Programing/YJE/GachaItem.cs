using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaItem : MonoBehaviour
{
    private int itemId;
    public int ItemId { get { return itemId; } set { itemId = value; } }
    private string itemName;
    public string ItemName { get { return itemName; } set { itemName = value; } }
    private int amount;
    public int Amount { get { return amount; } set { amount = value; } }
    private Sprite itemImage;
    public Sprite ItemImage { get { return itemImage; } set { itemImage = value; } }

}
