using UnityEngine;
using System.Collections;

public class Waypoints : MonoBehaviour
{
    public Vector2[] Points { get; private set; }

    private void Awake()
    {
        Points = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            Points[i] = child.position;
            child.gameObject.SetActive(false);
        }
    }
}
