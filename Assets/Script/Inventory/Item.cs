[System.Serializable]
public class Item
{
    public int ID;
    public string Name;
    public int Amount;
    
    public Item(ItemObject itemObject)
    {
        ID = itemObject.Data.ID;
        Name = itemObject.name;
        Amount = 1;
    }
    
    //TODO Добавить конструктор с WorldItem
    
    public Item()
    {
        ID = -1;
        Name = "";
        Amount = 0;
    }

    public static bool IsEmpty(Item item)
    {
        return item.ID == -1;
    }
}
