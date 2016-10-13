using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuBackground : MonoBehaviour
{
    //private Material mat;
    private SpriteRenderer sr;
    private void Awake()
    {
        //mat = GetComponent<SpriteRenderer>().material;   
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        //mat.SetFloat("_Scale", Mathf.Sin(Time.time) * 0.025f + 0.225f);
        sr.color = Color.Lerp(Color.white, Color.black, 0.1f + Mathf.Sin(Time.time*2f) * 0.1f);
    }
}
