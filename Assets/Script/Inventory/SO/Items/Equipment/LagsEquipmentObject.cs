using UnityEngine;

[CreateAssetMenu(fileName = "LagsEquipmentObject", menuName = "Inventory/ItemObjects/Equipment/Lags")]
public class LagsEquipmentObject : EquipmentObject
{
    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Lags;
    }
}