using UnityEngine;

[CreateAssetMenu(fileName = "FeetEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Feet")]
public class FeetEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Feet;
    }
}