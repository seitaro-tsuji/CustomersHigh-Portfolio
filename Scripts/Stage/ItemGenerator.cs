using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemGenerator : MonoBehaviour
{
    //todo ランダム抽選に重みをつける
    public class GenerateItem
    {
        public ItemData data;
        public int weight;
    }

    [BoxGroup("ItemAmount [min, max]"), SerializeField, MinValue(0)] private int minItemAmount = 1;
    [BoxGroup("ItemAmount [min, max]"), SerializeField, MinValue(1)] private int maxItemAmount = 3;

    [SerializeField] private ItemDatabase itemDatabase;

    [SerializeField] private List<ItemData> items;

    private void Awake()
    {
        /*if(itemDatabase == null)
        {
            Debug.LogWarning("item databaseがnullです。");
            return;
        }*/

        //準備エリアに生成されるアイテムprefab一覧
        //List<GameObject> itemPrefabs = itemDatabase.GetItemPrefabsPreparation();
        List<GameObject> itemPrefabs = items.Select(x=>x.Prefab).ToList();

        int itemAmount = Random.Range(minItemAmount, maxItemAmount + 1);
        for(int i = 0; i < itemAmount; i++)
        {
            //子オブジェクトとしてアイテムをランダムに生成する
            GameObject item = Instantiate(
                itemPrefabs[Random.Range(0, itemPrefabs.Count)],
                transform);
        }
    }
}
