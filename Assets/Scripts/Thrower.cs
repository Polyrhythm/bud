using UnityEngine;
using HoloToolkit.Unity;

public class Thrower : MonoBehaviour {
    public GameObject throwablePrefab;
    public float force = 0.5f;
    private GameObject throwable;

    void OnReady()
    {
        if (throwable) return;

        throwable = (GameObject)Instantiate(throwablePrefab, transform.position, Quaternion.identity);
    }

    void OnInteract()
    {
        throwable.SendMessage("Release");
        throwable.GetComponent<Rigidbody>().isKinematic = false;
        throwable.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
        PetSingleton.Instance.SendMessage("OnToyThrown", throwable);
        throwable = null;
    }
}
