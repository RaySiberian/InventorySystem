using UnityEngine;

[CreateAssetMenu(fileName = "FoodObject", menuName = "Inventory/ItemObjects/FoodObject")]
public class FoodObject : ItemObject
{
    protected override void Awake()
    {
        base.Awake();
        Type = ItemObjectType.Food;
        StackAble = true;
    }
}
