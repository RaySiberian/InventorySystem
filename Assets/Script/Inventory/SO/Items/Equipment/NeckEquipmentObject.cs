using UnityEngine;

[CreateAssetMenu(fileName = "NeckEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Neck")]
public class NeckEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Neck;
    }
}