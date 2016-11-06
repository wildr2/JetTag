using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class ClearPSOnReset : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.on_reset += Clear;
    }
    private void OnDestroy()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null) gm.on_reset -= Clear;
    }
    private void Clear()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        ps.Clear();
    }
}
