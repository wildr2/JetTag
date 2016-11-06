using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Power { None, Dash, Blink, Swap, Springs, Warp, Cloak, Repel }

public class Pickup : MonoBehaviour
{
    private static Power[] order;
    private static int order_i;

    public Text name_text, icon_text;
    public Power power = Power.None;

    private Chara user;


    private void Awake()
    {
        if (order == null)
        {
            Power[] powers = (Power[])Tools.EnumValues(typeof(Power));
            order = new Power[powers.Length - 1];
            System.Array.Copy(powers, 1, order, 0, order.Length);
            order = Tools.ShuffleArray(order);
            order_i = 0;
        }

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

        // User
        user = c;
        user.on_pickup += OnUserPickupNew;
        user.on_use_power += Respawn;

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
    private void OnUserPickupNew(Pickup pu)
    {
        if (pu != this) Respawn();
    }
    private void OnDrop()
    {
        transform.position = user.transform.position;

        user.on_use_power -= Respawn;
        user.on_pickup -= OnUserPickupNew;
        user = null;
    }
    private void Respawn()
    {
        OnDrop();
        StartCoroutine(CoroutineUtil.DoAfterDelay(Spawn, 3));
    }
    private void Spawn()
    {
        //power = (Power)Random.Range(1, Tools.EnumLength(typeof(Power)));
        power = order[order_i];
        order_i = (order_i + 1) % order.Length;

        GetComponent<Collider2D>().enabled = true;
        name_text.gameObject.SetActive(false);
        icon_text.gameObject.SetActive(true);
    }
    private void Reset()
    {
        StopAllCoroutines();

        if (user != null) OnDrop();
        Spawn();
    }
}
