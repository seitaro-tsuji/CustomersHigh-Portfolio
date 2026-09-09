using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> itemDatas;

    //準備エリアに生成されるアイテムprefabリストを返す
    public List<GameObject> GetItemPrefabsPreparation()
    {
        return itemDatas
            .Where(item => item.GeneretedInPreparationErea)
            .Select(item=>item.Prefab)
            .ToList();
    }
}
