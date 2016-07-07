using UnityEngine;
using HoloToolkit.Unity;

public partial class NameSelect : Singleton<NameSelect> {
    public static string petName = "Edward";
    public TextMesh textChild;
    TouchScreenKeyboard keyboard;

    void Start()
    {
        SetText(petName);
        GameController.Instance.petName = petName;
    }

    void Update()
    {
        if (keyboard != null && keyboard.active == false)
        {
            if (keyboard.done == true)
            {
                petName = keyboard.text.Trim();
                keyboard = null;
                SetText(petName);
                GameController.Instance.petName = petName;
            }
        }
    }

    public void OnNameSelect()
    {
        Debug.Log("hello!");
        keyboard = new TouchScreenKeyboard(petName, TouchScreenKeyboardType.Default, false, false, false, false, "Choose a name!");
    }

    void SetText(string name)
    {
        textChild.text = name + " (tap to choose)";
    }
}
