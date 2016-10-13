using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour
{
    public MenuPage pause_page;
    public AudioSource pause_sound, resume_sound;
    private bool paused = false;
    //private static UID timescale_id = new UID();


    public bool IsPaused()
    {
        return paused;
    }

    public void Update()
    {
        // Pause input
        bool pause_input = Input.GetButtonDown("Pause");
        if (pause_input)
        {
            if (paused)
            {
                // unpause input only when dialog not open
                if (pause_page.IsTopmost()) UnPause(); 
            }
            else Pause();
        }
    }
    public void Pause()
    {
        paused = true;
        //TimeScaleManager.Instance.SetFactor(0, timescale_id);
        Time.timeScale = 0;
        pause_page.SetIn();
        if (pause_sound != null) pause_sound.Play();
    }
    public void UnPause()
    {
        paused = false;
        //TimeScaleManager.Instance.SetFactor(1, timescale_id);
        Time.timeScale = 1;
        pause_page.SetOut();
        if (resume_sound != null) resume_sound.Play();
    }
    public void TogglePause()
    {
        if (paused) UnPause();
        else Pause();
    }

}
