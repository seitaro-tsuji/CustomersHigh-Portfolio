using UnityEngine;

public class MainStageRoot : MonoBehaviour
{
    [SerializeField] private int stageNum;
    [SerializeField] private Transform startPoint;

    public Vector3 StartPointPos => startPoint.position;
}
