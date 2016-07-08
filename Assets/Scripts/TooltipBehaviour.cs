using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class TooltipBehaviour : MonoBehaviour {
    private Vector3 origPos;
    private Interpolator interp;
    private UAudioManager audioManager;

	// Use this for initialization
	void Start () {
        interp = GetComponent<Interpolator>();
        origPos = transform.position;
        transform.position = transform.position - (Vector3.up * 10);
        interp.SetTargetPosition(origPos);
        audioManager = GetComponent<UAudioManager>();
        audioManager.PlayEvent("pop");
	}

    void GoAway()
    {
        StartCoroutine(SlideDown());
    }

    IEnumerator SlideDown()
    {
        WaitForSeconds delay = new WaitForSeconds(1f);
        interp.SetTargetPosition(transform.position - Vector3.up * 2f);
        yield return delay;
        Destroy(gameObject);
    }
}
