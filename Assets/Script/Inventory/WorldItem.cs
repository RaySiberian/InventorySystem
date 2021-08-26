using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField]private ItemObject worldItemObject;
    public int ItemId = -1;
    public int Amount;
    public string Name;
    private void Start()
    {
        ItemId = worldItemObject.Data.ID;
        Amount = worldItemObject.Data.Amount;
        Name = worldItemObject.Data.Name;
    }
}
