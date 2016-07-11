using UnityEngine;
using UnityEngine.SceneManagement;
using HoloToolkit.Unity;
using Pathfinding;
using System.Collections;

public partial class GameController : Singleton<GameController> {
    private GameObject[] stateSubscribers;

    public enum GameStates {
        Menu, Scanning, PetSelect, PrePetSpawn,
        CommandTutorial, MenuTutorial, Play, Interaction
    };
    public enum PetTypes { Panda, Bear };
    public GameStates state;
    public GameObject voiceManager;
    public GameObject pandaPrefab;

    // Tooltip prefabs.
    public GameObject mapAdvicePrefab;
    public GameObject spawnAdvicePrefab;
    public GameObject petSelectMenuPrefab;
    public GameObject tutorialAdvicePrefab;
    public GameObject menuAdvicePrefabOne;
    public GameObject menuAdvicePrefabTwo;
    public GameObject foodAdvicePrefab;

    public GameObject ingameMenuPrefab;

    // Stateful shit.
    [HideInInspector]
    public int affinityState = 1;
    [HideInInspector]
    public int hungerState = 4;

    [HideInInspector]
    public string petName = "Edward";

    [HideInInspector]
    public bool colToggled;

    private GameObject mapAdvice;
    private GameObject spawnAdvice;
    private GameObject selectPetMenu;
    private GameObject selectedPetPrefab;
    private GameObject tutorialAdvice;
    private GameObject ingameMenu;
    private GameObject menuAdviceOne;
    private GameObject menuAdviceTwo;
    private GameObject foodAdvice;

    private bool foodTutorialPlayed = false;

    // Time-related vars.
    private float hungerLastTime = -1;

    public void SetState(GameStates newState)
    {
        state = newState;
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void StartPlayState()
    {
        StartCoroutine(CalculateHunger());
    }

    // TODO: Come up with a better system.
    void UpdateState()
    {
        GameObject.Find("IngameMenu(Clone)").BroadcastMessage("UpdateState");
    }

    // Check time and degrade hunger appropriately.
    IEnumerator CalculateHunger()
    {
        while (true)
        {
            // Have we established a base hungerTime yet?
            if (hungerLastTime == -1)
            {
                hungerLastTime = Time.time;
            }
            // Has enough time passed for the pet to degrade hunger state?
            else if (Time.time - hungerLastTime >= 60 * 5)
            {
                hungerLastTime = Time.time;
                if (hungerState > 0) hungerState -= 1;

                if (hungerState <= 2 && !foodTutorialPlayed)
                {
                    // Start the food tutorial.
                    StartCoroutine(BeginFoodTutorial());
                }

                // Broadcast state change to all subscribers.
                UpdateState();
            }

            yield return new WaitForSeconds(4);
        }
    }

    public void OnHereCommand()
    {
        if (!tutorialAdvice || state != GameStates.CommandTutorial) return;

        tutorialAdvice.SendMessage("GoAway");
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

        spawnAdvice.SendMessage("GoAway");
        StartCoroutine(BeginCommandTutorial());
        StartCoroutine(BeginMenuTutorial());
    }

    IEnumerator BeginCommandTutorial()
    {
        state = GameStates.CommandTutorial;
        yield return new WaitForSeconds(15);
        tutorialAdvice = (GameObject)Instantiate(tutorialAdvicePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(10);
        tutorialAdvice.SendMessage("GoAway");
    }

    IEnumerator BeginMenuTutorial()
    {
        yield return new WaitForSeconds(45);
        state = GameStates.MenuTutorial;
        menuAdviceOne = (GameObject)Instantiate(menuAdvicePrefabOne, transform.position, Quaternion.identity);
    }

    public void CloseMenuAdvice()
    {
        menuAdviceOne.SendMessage("GoAway");
        state = GameStates.Play;
        StartPlayState();
    }

    public void SpawnGameMenu()
    {
        ingameMenu = (GameObject)Instantiate(ingameMenuPrefab, Camera.main.transform.position + Camera.main.transform.forward * 2f, Quaternion.identity);
        ingameMenu.SendMessage("OnSelect");
    }

    public IEnumerator MoreMenuAdvice()
    {
        menuAdviceTwo = (GameObject)Instantiate(menuAdvicePrefabTwo, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(8);
        menuAdviceTwo.SendMessage("GoAway");
    }

    IEnumerator BeginFoodTutorial()
    {
        foodAdvice = (GameObject)Instantiate(foodAdvicePrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(8);
        foodAdvice.SendMessage("GoAway");   
    }
}
