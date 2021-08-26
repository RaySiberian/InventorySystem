using UnityEngine;

[CreateAssetMenu(fileName = "TorsoEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Torso")]
public class TorsoEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Torso;
    }
}