using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NameSubmitController : MonoBehaviour {
    public InputField input;

    void OnSelect()
    {
        NameSelect.Instance.OnSubmit(input.text.Trim());
    }
}
