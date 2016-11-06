using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MovingObjectAudio : MonoBehaviour 
{
    public float max_move_speed = 10;
    public float max_pitch = 2;
    public float pitch_offset = 0;

    public Rigidbody2D rb;


    public void Awake()
    {
        GetComponent<AudioSource>().volume = 0;
        GetComponent<AudioSource>().Play();
    }
    public void Update()
    {
        float speed_factor = Mathf.Clamp(rb.velocity.magnitude / max_move_speed, 0, 1);

        // louder volume when moving faster
        if (Time.timeScale == 0) GetComponent<AudioSource>().volume = 0;
        else GetComponent<AudioSource>().volume = speed_factor * 0.75f;// * GameSettings.Instance.volume_fx;

        // faster playback when moving faster
        GetComponent<AudioSource>().pitch = pitch_offset + speed_factor * Time.timeScale * max_pitch;
    }
}
