using UnityEngine;

[CreateAssetMenu(fileName = "HeadEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Head")]
public class HeadEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Head;
    }
}