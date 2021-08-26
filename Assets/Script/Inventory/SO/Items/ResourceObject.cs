using UnityEngine;

[CreateAssetMenu(fileName = "ResourceObject", menuName = "Inventory/ItemObjects/ResourceObject")]
public class ResourceObject : ItemObject
{
    protected override void Awake()
    {
        base.Awake();
        Type = ItemObjectType.Resources;
        StackAble = true;
    }
}
