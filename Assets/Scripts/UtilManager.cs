using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public partial class UtilManager : Singleton<UtilManager> {
    private const int GROUND_LAYER = 8;

    public Vector3 GetGroundPosition(GameObject obj)
    {
        RaycastHit hit;
        Ray ray = new Ray(obj.transform.position, Vector3.up * -1);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.layer == GROUND_LAYER)
            {
                return hit.point; 
            }
        }

        return obj.transform.position;
    }
}
