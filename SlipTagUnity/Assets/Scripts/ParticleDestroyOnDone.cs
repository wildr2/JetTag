using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDestroyOnDone : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (!ps.IsAlive()) Destroy(gameObject);
    }
}
