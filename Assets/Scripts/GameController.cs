using UnityEngine;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity;
using Pathfinding;

public partial class GameController : Singleton<GameController> {
    public enum GameStates { Menu, Scanning, PetSelect, PrePetSpawn, Tutorial, Play };
    public enum PetTypes { Panda, Bear };
    public GameStates state;
    public GameObject voiceManager;
    public GameObject pandaPrefab;
    public GameObject mapAdvicePrefab;
    public GameObject spawnAdvicePrefab;
    public GameObject petSelectMenuPrefab;
    public GameObject tutorialAdvicePrefab;

    [HideInInspector]
    public string petName = "Edward";

    [HideInInspector]
    public bool colToggled;

    private GameObject mapAdvice;
    private GameObject spawnAdvice;
    private GameObject selectPetMenu;
    private GameObject selectedPetPrefab;
    private GameObject tutorialAdvice;
    private float gameStart;
    private bool tutorialPlayed = false;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void Update()
    {
        if (state != GameStates.Tutorial || tutorialPlayed) return;

        if (Time.time - gameStart < 15) return;

        tutorialAdvice = (GameObject)Instantiate(tutorialAdvicePrefab, transform.position, Quaternion.identity);
        tutorialPlayed = true;
    }

    public void OnHereCommand()
    {
        if (!tutorialAdvice || !tutorialPlayed || state != GameStates.Tutorial) return;

        tutorialAdvice.SendMessage("GoAway");
        state = GameStates.Play;
    }

    static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("Scene changed: " + scene.buildIndex + " : " + loadSceneMode);

        switch (scene.buildIndex)
        {
            case 0:
                GameController.Instance.state = GameStates.Menu;
                break;

            case 1:
                GameController.Instance.StartScan();
                break;

            default:
                break;
        }
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void StartScan()
    {
        state = GameStates.Scanning;
        SpatialMappingManager.Instance.StartObserverOrLoadFixture();
        mapAdvice = (GameObject)Instantiate(mapAdvicePrefab, transform.position, Quaternion.identity);
    }

    public void StopScan()
    {
        state = GameStates.PetSelect;
        Destroy(mapAdvice);
        SpatialMappingManager.Instance.StopObserver();
        SpatialProcessingManager.Instance.StartProcessing();

        selectPetMenu = (GameObject)Instantiate(petSelectMenuPrefab, transform.position, Quaternion.identity);
        PlayerController.Instance.StopIntroMusic();
    }

    public void PetSelect(PetTypes petType)
    {
        switch (petType)
        {
            case PetTypes.Panda:
                selectedPetPrefab = pandaPrefab;
                state = GameStates.PrePetSpawn;
                break;

            default:
                break;
        }

        voiceManager.GetComponent<KeywordManager>().StartKeywordRecognizer();
        Destroy(selectPetMenu);

        spawnAdvice = (GameObject)Instantiate(spawnAdvicePrefab, transform.position, Quaternion.identity);
    }

    void OnMeshProcessingComplete()
    {
        NavigationManager.Instance.CreateNavMesh();
    }

    public void SpawnPet(Ray ray)
    {
        if (state == GameStates.Play) return;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            Vector3 up = new Vector3(0, 1, 0);
            GraphNode node = AstarPath.active.GetNearest(hit.point).node;
            GameObject pet = (GameObject)Instantiate(
                selectedPetPrefab,
                (Vector3)node.position + up,
                Quaternion.identity
            );
        }

        state = GameStates.Tutorial;
        gameStart = Time.time;
        spawnAdvice.SendMessage("GoAway");
    }
}
