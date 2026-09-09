using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool generetedInPreparationErea;

    public string DisplayName => displayName;
    public GameObject Prefab => prefab;
    public bool GeneretedInPreparationErea => generetedInPreparationErea;
}
