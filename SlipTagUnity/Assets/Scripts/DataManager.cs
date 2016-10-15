using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum ControlScheme { None, WASD, Arrows, Gamepad1, Gamepad2, AI }
public enum Control { X, Y, Action, Start }

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DataManager>();

                if (_instance == null) Debug.LogError("Missing DataManager");
                else
                {
                    DontDestroyOnLoad(_instance);
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }


    // Players
    public Color[] color_options;
    public int[] player_color_ids = { 0, 0 };

    // Stats
    public List<MatchStats> match_stats = new List<MatchStats>();


    // PUBLIC ACCESSORS

    public bool ValidControlChoices()
    {
        return InputExt.GetPlayerScheme(0) != InputExt.GetPlayerScheme(1)
            || (ControlScheme)InputExt.GetPlayerScheme(0) == ControlScheme.AI;
    }
    public bool ValidColorChoices()
    {
        return player_color_ids[0] != player_color_ids[1];
    }

    public Color GetPlayerColor(int id)
    {
        return color_options[player_color_ids[id]];
    }



    // PUBLIC MODIFIERS


    // PRIVATE / PROTECTED MODIFIERS

    private void Awake()
    {
        // if this is the first instance, make this the singleton
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            Initialize();
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }
    private void Initialize()
    {
        if (player_color_ids.Length != 2)
        {
            player_color_ids = new int[2];
            player_color_ids[0] = UnityEngine.Random.Range(0, color_options.Length);
            player_color_ids[1] = UnityEngine.Random.Range(0, color_options.Length);
            if (!ValidColorChoices())
            {
                player_color_ids[0] = (player_color_ids[0] + 1) % color_options.Length;
            }
        }

        // Controls
        InputExt.RegisterPlayers(2, ControlScheme.None);

        InputExt.AddAxis(ControlScheme.WASD, Control.X, KeyCode.A, KeyCode.D);
        InputExt.AddAxis(ControlScheme.WASD, Control.Y, KeyCode.S, KeyCode.W);
        InputExt.AddKey(ControlScheme.WASD, Control.Action, KeyCode.LeftShift);
        InputExt.AddKey(ControlScheme.WASD, Control.Start, KeyCode.LeftShift);

        InputExt.AddAxis(ControlScheme.Arrows, Control.X, KeyCode.LeftArrow, KeyCode.RightArrow);
        InputExt.AddAxis(ControlScheme.Arrows, Control.Y, KeyCode.DownArrow, KeyCode.UpArrow);
        InputExt.AddKey(ControlScheme.Arrows, Control.Action, KeyCode.RightShift);
        InputExt.AddKey(ControlScheme.Arrows, Control.Start, KeyCode.RightShift);

        InputExt.AddAxis(ControlScheme.Gamepad1, Control.X, "GP1_Horizontal");
        InputExt.AddAxis(ControlScheme.Gamepad1, Control.Y, "GP1_Vertical");
        InputExt.AddKey(ControlScheme.Gamepad1, Control.Action, KeyCode.Joystick1Button0);
        InputExt.AddKey(ControlScheme.Gamepad1, Control.Action, () => Input.GetAxis("GP1_Triggers") != 0);
        InputExt.AddKey(ControlScheme.Gamepad1, Control.Start, KeyCode.Joystick1Button7);

        InputExt.AddAxis(ControlScheme.Gamepad2, Control.X, "GP2_Horizontal");
        InputExt.AddAxis(ControlScheme.Gamepad2, Control.Y, "GP2_Vertical");
        InputExt.AddKey(ControlScheme.Gamepad2, Control.Action, KeyCode.Joystick2Button0);
        InputExt.AddKey(ControlScheme.Gamepad2, Control.Action, () => Input.GetAxis("GP2_Triggers") != 0);
        InputExt.AddKey(ControlScheme.Gamepad2, Control.Start, KeyCode.Joystick2Button7);
    }

}
public class MatchStats
{
    public Color[] colors;
    public int[] scores;
}
