using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private float speed;
    [SerializeField] private bool canBeDefeatedByStomp;

    public string EnemyName => enemyName;
    public float Speed => speed;
    public bool CanBeDefeatedByStomp => canBeDefeatedByStomp;
}
