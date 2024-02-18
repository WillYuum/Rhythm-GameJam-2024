using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrefabManager;
using PrefabManager.Configs;

[RequireComponent(typeof(GridController))]
public class RoomSpawnManager : MonoBehaviour
{
    [SerializeField] public PrefabConfig DoorExitPrefab;
    [SerializeField] public PrefabConfig EnemyPrefab;
    [SerializeField] public PrefabConfig ObstaclePrefab;
}
