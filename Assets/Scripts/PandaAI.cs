using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class PandaAI : RichPetAI {
    public override void GoToHooman()
    {
        base.GoToHooman();
        audioManager.PlayEvent("react");       
    }
}
