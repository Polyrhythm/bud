using UnityEngine;
using HoloToolkit.Unity;
using System.Collections;

public class IconController : MonoBehaviour {
    public float amplitude = 0.5f;
    public float frequency = 0.5f;
    public float displayTime = 5f;
    public GameObject target;

    private Interpolator interp;

	// Use this for initialization
	void Awake () {
        interp = GetComponent<Interpolator>(); 
	}

    void Start()
    {
        Vector3 origScale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        interp.SetTargetLocalScale(origScale);

        StartCoroutine(GoAway());
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = target.transform.position + target.transform.up * 0.5f;
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
	}

    IEnumerator GoAway()
    {
        yield return new WaitForSeconds(displayTime);
        interp.SetTargetLocalScale(new Vector3(0, 0, 0));
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
