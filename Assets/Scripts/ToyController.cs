using UnityEngine;

public class ToyController : MonoBehaviour {
    private bool stickOnPlayer = true;

    void Update()
    {
        if (!stickOnPlayer) return;

        transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized;
    }

    void OnSelect()
    {
        Destroy(transform.gameObject);
    }

    public void Release()
    {
        stickOnPlayer = false; 
    }
}
