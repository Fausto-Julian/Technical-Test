using UnityEngine;

[CreateAssetMenu(fileName = "TargetData", menuName = "TargetData", order = 0)]
public class TargetDataSo : ScriptableObject
{
    [SerializeField] private TargetType targetType;
    [SerializeField] private int pointsAdd;
    [SerializeField] private int pointsRemove;
    [SerializeField] private int numberClicksToDestroy;
    [SerializeField] private float timeIsAlive;

    public TargetType TargetType => targetType;
    public int PointsAdd => pointsAdd;
    public int PointsRemove => pointsRemove;
    public int NumberClicksToDestroy => numberClicksToDestroy;
    public float TimeIsAlive => timeIsAlive;
}