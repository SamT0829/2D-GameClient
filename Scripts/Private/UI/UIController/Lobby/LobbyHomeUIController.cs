using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyHomeUIController : MonoBehaviour
{
    [Header("Player Prefab")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject weaponEquip;
    [SerializeField] GameObject armorEquip;
    [SerializeField] GameObject shoeEquip;

    [Header("Inventory")]
    private Dictionary<ItemName, SlotItemPrefab> playerInventoryPrefabTable = new Dictionary<ItemName, SlotItemPrefab>();

    public void ClickEquip(EquipName equipType)
    {
        switch (equipType)
        {
            case EquipName.Weapon:
                weaponEquip?.SetActive(true);
                break;
            case EquipName.Armor:
                weaponEquip?.SetActive(true);
                break;
            case EquipName.Shoe:
                weaponEquip?.SetActive(true);
                break;

        }
    }
}
