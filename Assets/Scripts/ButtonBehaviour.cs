using UnityEngine;
using HoloToolkit.Unity;

public class ButtonBehaviour : MonoBehaviour {
    [Tooltip("Factor by which to scale the button on gaze.")]
    public float scaleFactor = 1.2f;

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

    void OnGazeEnter()
    {
        interp.SetTargetLocalScale(origScale * scaleFactor);
    }

    void OnGazeLeave()
    {
        interp.SetTargetLocalScale(origScale * (1f - (scaleFactor - 1f)));
    }
}
