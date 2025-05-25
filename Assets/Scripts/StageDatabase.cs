using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "Game/Stage Database", order = 2)]
public class StageDatabase : ScriptableObject
{
    public List<StageData> stages;
}