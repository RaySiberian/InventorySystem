using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Inventory/Database")]
public class Database : ScriptableObject
{
    public ItemObject[] ItemObjects;
    public CraftObject[] CraftObjects;
    public Dictionary<int, ItemObject> GetItemByID;
    private List<ItemObject> tepmList; 
    
    private void OnEnable()
    {
        CheckContainer();
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            ItemObjects[i].Data.ID = i;
            ItemObjects[i].Data.Name = ItemObjects[i].name;
            if (!ItemObjects[i].StackAble)
            {
                ItemObjects[i].MaxStuckSize = -1;
            }
        }

        GetItemByID = new Dictionary<int, ItemObject>();
        
        foreach (var item in ItemObjects)
        {
            GetItemByID.Add(item.Data.ID,item);
        }
        
        SetCraftArray();
    }

    private void SetCraftArray()
    {
        foreach (var t in CraftObjects)
        {
            t.CraftItems = new Item[9];
            for (int j = 0; j < t.CraftItems.Length; j++)
            {
                try
                {
                    t.CraftItems[j] = t.CraftSlot[j].ItemObject.Data;
                    t.CraftItems[j].Amount = t.CraftSlot[j].Amount;
                }
                catch
                {
                    t.CraftItems[j] = new Item();
                }
           
            }
        }
    }
    
    private void CheckContainer()
    {
        foreach (var t in ItemObjects)
        {
            if (t == null)
            {
                ReFillContainer();
            }
        }
    }
    
    private void ReFillContainer()
    {
        tepmList.Clear();
        foreach (var t in ItemObjects)
        {
            if (t != null)
            {
                tepmList.Add(t);
            }
        }

        ItemObjects = new ItemObject[]{};
        ItemObjects = tepmList.ToArray();
    }
}
