using UnityEngine;

public class ToySelector : MonoBehaviour {
    void OnSelect()
    {
        GameController.Instance.SetState(GameController.GameStates.Interaction);
        Camera.main.SendMessage("OnReady");
    }
}
