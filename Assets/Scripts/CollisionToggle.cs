using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public partial class CollisionToggle : Singleton<CollisionToggle> {
    public bool colToggled = false;
    public GameObject box;
    private Material boxMat;

    void Awake()
    {
        boxMat = box.GetComponent<MeshRenderer>().material;
    }

    public void OnToggle()
    {
        colToggled = !colToggled;

        UpdateBox();
    }

    void UpdateBox()
    {
        box.SendMessage("OnTap");

        switch (colToggled)
        {
            case true:
                boxMat.color = Color.green;
                break;

            default:
                boxMat.color = Color.white;
                break;
        }

        GameController.Instance.colToggled = colToggled;
    }
}
