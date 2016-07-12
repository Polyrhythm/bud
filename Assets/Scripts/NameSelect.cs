using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

public partial class NameSelect : Singleton<NameSelect> {
    public static string petName = "Edward";
    public TextMesh textChild;
    public GameObject keyboardPrefab;
    public InputField textInput;

    private GameObject keyboard;

    void Start()
    {
        SetText(petName);
        GameController.Instance.petName = petName;
    }

    public void OnNameSelect()
    {
        keyboard = (GameObject)Instantiate(keyboardPrefab, transform.position, Quaternion.identity);
    }

    public void OnSubmit()
    {
        petName = textInput.text.Trim();
    }

    void SetText(string name)
    {
        textChild.text = name + " (tap to choose)";
    }
}
