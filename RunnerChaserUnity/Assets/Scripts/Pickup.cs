using UnityEngine;
using System.Collections;

public enum Power { None, Dash }

public class Pickup : MonoBehaviour
{
    public Transform graphics;
    public Power power = Power.Dash;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        graphics.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Respawn());
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(10);
        GetComponent<Collider2D>().enabled = true;
        graphics.gameObject.SetActive(true);
    }
}
