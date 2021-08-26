using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Inventory/Database")]
public class InventoryDatabase : ScriptableObject
{
    public ItemObject[] ItemObjects;
    private List<ItemObject> tepmList; 
    
    private void OnEnable()
    {
        CheckContainer();
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            ItemObjects[i].Data.ID = i;
            if (!ItemObjects[i].StackAble)
            {
                ItemObjects[i].MaxStuckSize = -1;
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
