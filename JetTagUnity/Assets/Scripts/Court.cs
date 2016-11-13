using UnityEngine;
using System.Collections;

public class Court : MonoBehaviour
{
    public Transform[] spawn_positions;
    [System.NonSerialized] public Waypoints waypoints;

    private void Awake()
    {
        waypoints = GetComponentInChildren<Waypoints>();
    }
}
