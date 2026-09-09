using UnityEngine;

public class PreparationSection : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    public Vector3 StartPoint => _startPoint.position;

    [SerializeField] private Transform _endPoint;
    public Vector3 EndPoint => _endPoint.position;
}
