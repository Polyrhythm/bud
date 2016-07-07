using UnityEngine;

public class CommandTutorial : MonoBehaviour {
    void Start()
    {
        GetComponent<TextMesh>().text += "Say " + GameController.Instance.petName + ", come here!";
    }
}
