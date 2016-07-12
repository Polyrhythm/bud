using UnityEngine;
using System.Collections;

public class BowlController : MonoBehaviour {
    public GameObject refillButton;

    void Start()
    {
        transform.gameObject.SendMessage("OnSelect");
    }

    void OnGazeEnter()
    {
        refillButton.GetComponent<Renderer>().enabled = true;
        refillButton.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
    }

    void OnGazeLeave()
    {
        refillButton.GetComponent<Renderer>().enabled = false;
        refillButton.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
    }

    void OnPlace()
    {
        PetSingleton.Instance.GetComponent<RichPetAI>().BowlPlaced(gameObject);
    }
}
