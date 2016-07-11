using UnityEngine;
using System.Collections;

public class IngameMenuController : MonoBehaviour {
    public Material affinityEnabled;
    public Material affinityDisabled;

    public Material hungerEnabled;
    public Material hungerDisabled;

    private GameObject[] affinityMarkers;
    private GameObject[] hungerMarkers;

    private Color32 hungerColorFull = new Color32(99, 244, 142, 255);
    private Color32 hungerColorMed = new Color32(247, 249, 112, 255);
    private Color32 hungerColorLow = new Color32(249, 128, 112, 255);

    void OnPlace() {
        StartCoroutine(GameController.Instance.MoreMenuAdvice());
        UpdateState();
    }

    void UpdateState()
    {
        UpdateAffinity();
        UpdateHunger();
    }

    void UpdateAffinity()
    {
        affinityMarkers = GameObject.FindGameObjectsWithTag("affinityMarkers");
        for (int i = 0; i < GameController.Instance.affinityState; i++)
        {
            affinityMarkers[i].GetComponent<MeshRenderer>().material = affinityEnabled;
        }
    }

    void UpdateHunger()
    {
        hungerMarkers = GameObject.FindGameObjectsWithTag("hungerMarkers");

        switch (GameController.Instance.hungerState)
        {
            case 2:
                hungerDisabled.color = hungerColorMed;
                hungerEnabled.color = hungerColorMed;
                break;

            case 1:
            case 0:
                hungerDisabled.color = hungerColorLow;
                hungerEnabled.color = hungerColorLow;
                break;

            default:
                hungerDisabled.color = hungerColorFull;
                hungerEnabled.color = hungerColorFull;
                break;
        }

        // Have to clear all the markers first since this one goes down with time.
        for (int i = 0; i < hungerMarkers.Length; i++)
        {
            hungerMarkers[i].GetComponent<MeshRenderer>().material = hungerDisabled;
        }

        for (int i = 0; i < GameController.Instance.hungerState; i++)
        {
            hungerMarkers[i].GetComponent<MeshRenderer>().material = hungerEnabled;
        }
    }
}
