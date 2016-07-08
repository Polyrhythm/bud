using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class PandaAI : RichPetAI {
    public override void GoToHooman()
    {
        base.GoToHooman();
        audioManager.PlayEvent("react");       
    }

    void OnToyThrown(GameObject toy)
    {
        GoToShinyObject(toy.transform.position);
        audioManager.PlayEvent("react");
    }
}
