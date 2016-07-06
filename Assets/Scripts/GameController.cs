using UnityEngine;
using HoloToolkit.Unity;
using Pathfinding;

public partial class GameController : Singleton<GameController> {
    public enum GameStates { PreScan, Scanning, PrePetSpawn, Play };
    public GameStates state = GameStates.PreScan;
    public GameObject voiceManager;
    public GameObject petPrefab;

    public void StartScan()
    {
        state = GameStates.Scanning;
        SpatialMappingManager.Instance.StartObserverOrLoadFixture();
    }

    public void StopScan()
    {
        state = GameStates.PrePetSpawn;
        SpatialMappingManager.Instance.StopObserver();
        SpatialProcessingManager.Instance.StartProcessing();
    }

    void OnMeshProcessingComplete()
    {
        NavigationManager.Instance.CreateNavMesh();
        voiceManager.GetComponent<KeywordManager>().StartKeywordRecognizer();
    }

    public void SpawnPet(Ray ray)
    {
        if (state == GameStates.Play) return;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            Vector3 up = new Vector3(0, 1, 0);
            GraphNode node = AstarPath.active.GetNearest(hit.point).node;
            GameObject pet = (GameObject)Instantiate(
                petPrefab,
                (Vector3)node.position + up,
                Quaternion.identity
            );
        }

        state = GameStates.Play;
    }
}
