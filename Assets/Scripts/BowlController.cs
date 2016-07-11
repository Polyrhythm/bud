using UnityEngine;
using System.Collections;

public class BowlController : MonoBehaviour {
    void Start()
    {
        transform.gameObject.SendMessage("OnSelect");
    }

    void OnPlace()
    {
        PetSingleton.Instance.SendMessage("BowlPlaced");
    }
}
