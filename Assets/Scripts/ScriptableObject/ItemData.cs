using System;
using UnityEngine;

public enum ItemType
{
    Equipable,
    Consumable,
    Resource
}

public enum ConsumableType 
{
    Health,
    Hunger,
    InfiniteStamina,
    ReduceHungerPassiveValue,
    Fast,
    MaxHealth,
    IncreaseHealthPassiveValue
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType consumableType;
    public float value;
}
[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("info")]
    public string displayName;
    public string description;
    public ItemType itemType;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}