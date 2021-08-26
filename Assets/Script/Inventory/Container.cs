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
    public Item[] Storage => SerializableContainer.AllItems;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem(test);
        }
    }

    public void AddItem(ItemObject addingItemObject)
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