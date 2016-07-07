using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class tapReact : MonoBehaviour {
    [Tooltip("Scale factor to use for tap reaction")]
    public float scaleFactor = 0.8f;

    [Tooltip("Time in seconds to play the hit and recovery animation")]
    public float hitTime = 0.5f;

    private Interpolator interp;
    private Vector3 origScale;

    void Awake()
    {
        interp = GetComponent<Interpolator>();
    }

    void Start()
    {
        origScale = transform.localScale;
    }

    void OnTap()
    {
        StartCoroutine(React());
    }

    IEnumerator React()
    {
        WaitForSeconds delay = new WaitForSeconds(hitTime);
        interp.SetTargetLocalScale(origScale * scaleFactor);
        yield return delay;
        interp.SetTargetLocalScale(origScale);
    }
}
