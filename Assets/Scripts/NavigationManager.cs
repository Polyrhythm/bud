using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public partial class NavigationManager : Singleton<NavigationManager> {
    public GameObject petPrefab;

    public void CreateNavMesh()
    {
        AstarPath.active.astarData.recastGraph.SnapForceBoundsToScene();
        AstarPath.active.Scan();
    }
}
