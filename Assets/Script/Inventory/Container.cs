using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Container : MonoBehaviour
{
    private string savePath;
    public InventoryDatabase Database;
    public SerializableContainer SerializableContainer;
    public ItemObject test;
    public ItemObject test1;
    public Item[] Storage => SerializableContainer.AllItems;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem(test);
            AddItem(test1);
        }
    }

    public void AddItem(ItemObject addingItemObject)
    {
        if (!CheckItemInInventory(addingItemObject) || !addingItemObject.StackAble)
        {
            for (int i = 0; i < Storage.Length; i++)
            {
                if (IsSlotFree(Storage[i]))
                {
                    Storage[i] = new Item(addingItemObject);
                    break;
                }
            }
        }
        else
        {
            Item itemForStack = FindObjectInInventory(addingItemObject);
            AddAmount(itemForStack,addingItemObject);
        }
    }

    public bool CheckForStacking(ItemObject addingItemObject)
    {
        return addingItemObject.StackAble;
    }

    public bool CheckItemInInventory(ItemObject itemObject)
    {
        for (int i = 0; i < Storage.Length; i++)
        {
            if (Storage[i].ID == itemObject.Data.ID)
            {
                return true;
            }
        }
        return false;
    }

    public Item FindObjectInInventory(ItemObject itemObject)
    {
        for (int i = 0; i < Storage.Length; i++)
        {
            if (Storage[i].ID == itemObject.Data.ID)
            {
                return Storage[i];
            }
        }
        return null;
    }

    public void AddAmount(Item addToItem,ItemObject itemObject)
    {
        addToItem.Amount += itemObject.Data.Amount;
    }
    
    public bool IsSlotFree(Item itemToCheck)
    {
        return itemToCheck.ID == -1;
    }
    
    
    [ContextMenu("Save")]
    public void Save()
    {
        savePath = string.Concat(gameObject.name, "Inventory.Save");
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        bf.Serialize(file,saveData);
        file.Close();
        
    }


    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath),FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(),this);
            file.Close();
        }
    }
}