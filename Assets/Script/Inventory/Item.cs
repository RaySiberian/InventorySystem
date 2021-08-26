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
    }

    public Item(int id)
    {
        ID = id;
    }
    
    public Item()
    {
        ID = -1;
        Name = "";
    }
}
