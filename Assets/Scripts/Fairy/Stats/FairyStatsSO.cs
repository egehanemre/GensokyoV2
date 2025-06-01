using UnityEngine;

[CreateAssetMenu(fileName = "FairyStatsSO", menuName = "Scriptable Objects/FairyStatsSO")]
public class FairyStatsSO : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public float defense;
    public FairyType fairyType;
}
