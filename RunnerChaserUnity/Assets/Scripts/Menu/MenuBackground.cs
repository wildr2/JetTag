using UnityEngine;
using System.Collections;

public class MenuBackground : MonoBehaviour
{
    private Material mat;
    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
        
    }
    private void Update()
    {
        mat.SetFloat("_Scale", Mathf.Sin(Time.time * 0.3f) * 0.2f + 0.3f);
    }
}
