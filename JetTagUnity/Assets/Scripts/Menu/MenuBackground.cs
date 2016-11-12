using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuBackground : MonoBehaviour
{
    public SpriteRenderer title;
    public Transform bg1, bg2;

    private void Awake()
    {
    }
    private void Update()
    {
        title.color = Color.Lerp(Color.white, Color.black, 0.1f + Mathf.Sin(Time.time*2f) * 0.1f);

        bg1.transform.position = new Vector3(4*Mathf.Cos(Time.time * 0.3f), 0, 10);
        bg2.transform.position = new Vector3(4*Mathf.Sin(Time.time * 0.3f), 0, 10);

        bg1.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 0.15f) * 60);
        bg2.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 0.1f) * 20);
    }
}
