using UnityEngine;
[CreateAssetMenu(fileName = "CraftObject", menuName = "Inventory/CraftObject")]
public class CraftObject : ScriptableObject
{
    public CraftSlot[] CraftItemObjects;
    public ItemObject CraftedItemObject;
    public Item[] CraftItems;
}
