using UnityEngine;

[CreateAssetMenu(fileName = "FingersEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Fingers")]
public class FingersEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Fingers;
    }
}