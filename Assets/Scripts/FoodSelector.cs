using UnityEngine;

public class FoodSelector : MonoBehaviour
{
    public GameObject bowlPrefab;
    private GameObject bowl;

    void OnSelect()
    {
        bowl = (GameObject)Instantiate(bowlPrefab, transform.position, Quaternion.identity); 
    }
}

