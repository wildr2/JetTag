using UnityEngine;
using System.Collections;

public enum Power { None, Dash }

public class Pickup : MonoBehaviour
{
    public Transform graphics;
    public Power power = Power.Dash;



    private void Awake()
    {
        GameManager.Instance.on_reset += Reset;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        graphics.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(RespawnRoutine());
    }
    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(10);
        Respawn();
    }
    private void Respawn()
    {
        GetComponent<Collider2D>().enabled = true;
        graphics.gameObject.SetActive(true);
    }
    private void Reset()
    {
        StopAllCoroutines();
        Respawn();
    }
}
