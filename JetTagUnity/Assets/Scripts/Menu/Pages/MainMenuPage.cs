﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuPage : MenuPage
{
    public AudioSource player_change_sound;

    public Text court_btn_text;

    public Image[] balls;
    public Text[] controls_text;
    private string controls_text_initial =
        "AI\n\n" +
        "press start\n" +
        "or right shift\n" +
        "or left shift\n\n";
    private ControlScheme[] human_controls = new ControlScheme[]
    { ControlScheme.Arrows, ControlScheme.WASD, ControlScheme.Gamepad1, ControlScheme.Gamepad2 };

    private DataManager dm;


    // PUBLIC MODIFIERS

    public void OnButtonStart()
    {
        if (!dm.ValidColorChoices())
        {
            int player_id = Random.Range(0, 2);
            dm.player_color_ids[player_id] = Random.Range(0, dm.color_options.Length);
            if (!dm.ValidColorChoices())
                dm.player_color_ids[player_id] = (dm.player_color_ids[player_id] + 1) % dm.color_options.Length;

            UpdateBallColor(player_id);

            return;
        }

        if (dm.ValidControlChoices())
            SceneManager.LoadScene("Game");
    }


    // PRIVATE / PROTECTED MODIFIERS

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

        dm = DataManager.Instance;
        for (int i = 0; i < 2; ++i)
        {
            UpdateControlsText(i);
            UpdateBallColor(i);
        }
        UpdateCourtButton();
    }
    protected override void Update()
    {
        base.Update();

        if (IsInteractable())
        {
            // Controls start
            foreach (ControlScheme cs in human_controls)
            {
                if (InputExt.GetKeyDownCS(cs, Control.Start))
                    OnInputStart(cs);
            }

            // Color switching
            for (int i = 0; i < 2; ++i)
            {
                SwitchColor(i, InputExt.GetAxisOnce(i, Control.X, true));
            }
        }
    }

    private void OnInputStart(ControlScheme cs)
    {
        // Slot already with this cs
        for (int i = 0; i < 2; ++i)
        {
            if ((ControlScheme)InputExt.GetPlayerScheme(i) == cs)
            {
                InputExt.SetPlayerControlScheme(i, ControlScheme.AI);
                //SoundManager.PlayClickSound();
                player_change_sound.Play();
                UpdateControlsText(i);
                return;
            }
        }

        // Assign cs to empty slot
        for (int i = 0; i < 2; ++i)
        {
            if ((ControlScheme)InputExt.GetPlayerScheme(i) == ControlScheme.AI)
            {
                InputExt.SetPlayerControlScheme(i, cs);
                //SoundManager.PlayClickSound();
                player_change_sound.Play();
                UpdateControlsText(i);
                return;
            }
        }
    }
    private void UpdateControlsText(int player_id)
    {
        if ((ControlScheme)InputExt.GetPlayerScheme(player_id) == ControlScheme.AI)
        {
            controls_text[player_id].text = controls_text_initial;
        }
        else
        {
            controls_text[player_id].text = InputExt.GetPlayerScheme(player_id).ToString();
        }
    }

    private void SwitchColor(int player_id, int index_change)
    {
        if (index_change == 0) return;

        SoundManager.PlaySelectSound();

        int i = dm.player_color_ids[player_id];
        i = Tools.Mod(i + index_change, dm.color_options.Length);
        dm.player_color_ids[player_id] = i;

        UpdateBallColor(player_id);
    }
    private void UpdateBallColor(int player_id)
    {
        balls[player_id].color = dm.color_options[dm.player_color_ids[player_id]];
    }

    public void OnButtonCourt()
    {
        dm.court_type = Tools.NextEnumValue(dm.court_type);
        UpdateCourtButton();
    }
    private void UpdateCourtButton()
    {
        court_btn_text.text = dm.court_names[(int)dm.court_type];
    }

}
