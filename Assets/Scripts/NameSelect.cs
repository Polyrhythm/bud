using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

public partial class NameSelect : Singleton<NameSelect> {
    public static string petName = "Edward";
    public TextMesh textChild;
    public GameObject keyboardPrefab;

    private GameObject keyboard;

    void Start()
    {
        SetText(petName);
        GameController.Instance.petName = petName;
    }

    public void OnNameSelect()
    {
        keyboard = (GameObject)Instantiate(keyboardPrefab, Camera.main.transform.position + Camera.main.transform.forward * 1.5f, Quaternion.identity);
        transform.parent.GetComponent<Tagalong>().enabled = false;
        transform.parent.GetComponent<Interpolator>().SetTargetPosition(transform.position - transform.up);
    }

    public void OnSubmit(string text)
    {
        petName = text;
        GameController.Instance.petName = petName;
        SetText(petName);
        Destroy(keyboard);
        transform.parent.GetComponent<Tagalong>().enabled = true;
        transform.parent.GetComponent<Interpolator>().SetTargetPosition(transform.position + transform.up);
    }

    void SetText(string name)
    {
        textChild.text = name + " (tap to change name)";
    }
}
