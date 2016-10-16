using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Power { None, Dash, Blink, Swap, Springs, Warp, Cloak, Repel }

public class Pickup : MonoBehaviour
{
    public Text name_text, icon_text;
    public Power power = Power.None;


    private void Awake()
    {
        GameManager.Instance.on_reset += Reset;
        Spawn();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Chara c = collider.GetComponent<Chara>();
        StartCoroutine(OnPickup(c));
    }
    private IEnumerator OnPickup(Chara c)
    {
        GetComponent<Collider2D>().enabled = false;
        icon_text.gameObject.SetActive(false);

        // Respawn
        StartCoroutine(RespawnRoutine());

        // Name text
        string key_name = InputExt.GetControlName(c.PlayerID, Control.Action);
        name_text.text = power.ToString();
        if (key_name != "") name_text.text += " [" + key_name + "]";

        for (int i = 0; i < 5; ++i)
        {
            name_text.gameObject.SetActive(i%2==0);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.3f);
        name_text.gameObject.SetActive(false);
    }
    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(10);
        Spawn();
    }
    private void Spawn()
    {
        power = (Power)Random.Range(1, Tools.EnumLength(typeof(Power)));

        GetComponent<Collider2D>().enabled = true;
        name_text.gameObject.SetActive(false);
        icon_text.gameObject.SetActive(true);
    }
    private void Reset()
    {
        StopAllCoroutines();
        Spawn();
    }
}
