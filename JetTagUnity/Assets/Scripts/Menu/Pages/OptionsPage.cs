using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class OptionsPage : MenuPage
{
    // Sound Settings
    public Text music_text;
    public Text sound_text;

    private SoundManager sm;

    private static UID volume_id = new UID();


    public override void Initialize(PageManager manager)
    {
        base.Initialize(manager);

        sm = SoundManager.Instance;

        UpdateMusicBtnUI();
        UpdateSoundBtnUI();
    }
    public void ButtonMusic()
    {
        bool turn_on = sm.MusicVolume.GetFactor(volume_id) == 0;
        sm.MusicVolume.SetFactor(turn_on ? 1 : 0, volume_id);
        UpdateMusicBtnUI();
    }
    public void ButtonSound()
    {
        bool turn_on = sm.WorldVolume.GetFactor(volume_id) == 0;
        sm.WorldVolume.SetFactor(turn_on ? 1 : 0, volume_id);
        sm.UIVolume.SetFactor(turn_on ? 1 : 0, volume_id);
        UpdateSoundBtnUI();
    }


    private void UpdateMusicBtnUI()
    {
        bool on = sm.MusicVolume.GetFactor(volume_id) == 1;

        bool arrows = ButtonEffects.HasArrows(music_text.text);
        music_text.text = "Music " + (on ? "On" : "Off");
        if (arrows) music_text.text = ButtonEffects.AddArrows(music_text.text);
    }
    private void UpdateSoundBtnUI()
    {
        bool on = sm.WorldVolume.GetFactor(volume_id) == 1;

        bool arrows = ButtonEffects.HasArrows(sound_text.text);
        sound_text.text = "Sound " + (on ? "On" : "Off");
        if (arrows) sound_text.text = ButtonEffects.AddArrows(sound_text.text);
    }
}
