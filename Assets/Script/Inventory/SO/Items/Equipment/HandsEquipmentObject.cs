using UnityEngine;

[CreateAssetMenu(fileName = "HandsEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Hands")]
public class HandsEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Hands;
    }
}