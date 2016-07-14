using UnityEngine;
using HoloToolkit.Unity;

public partial class PetSingleton : Singleton<PetSingleton> {
    public GameObject heartPrefab;

    private RichPetAI ai;
    private GameObject icon;

    void Awake()
    {
        ai = GetComponent<RichPetAI>();
    }

    public void PlusOne()
    {
        icon = (GameObject)Instantiate(heartPrefab, transform.position, Quaternion.identity);
        icon.GetComponent<IconController>().target = gameObject;
        
        ai.affinity += 1;
    }

    public void GoToHooman()
    {
        ai.GoToHooman();
    }
}
