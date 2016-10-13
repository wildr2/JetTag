using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public ControlScheme[] player_controls;
    public int[] player_color_ids = { 0, 0 };

    // Stats
    public List<MatchStats> match_stats = new List<MatchStats>();


    // PUBLIC ACCESSORS

    public bool ValidControlChoices()
    {
        return player_controls[0] != player_controls[1]
            || player_controls[0] == ControlScheme.Gamepad
            || player_controls[0] == ControlScheme.AI;
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
        }
        else
        {
            // destroy other instances that are not the already existing singleton
            if (this != _instance)
                Destroy(this.gameObject);
        }

        Initialize();
    }
    private void Initialize()
    {
        if (player_controls.Length != 2)
        {
            player_controls = new ControlScheme[2];
            player_controls[0] = ControlScheme.None;
            player_controls[1] = ControlScheme.None;
        }
        if (player_color_ids.Length != 2)
        {
            player_color_ids = new int[2];
            player_color_ids[0] = Random.Range(0, color_options.Length);
            player_color_ids[1] = Random.Range(0, color_options.Length);
            if (!ValidColorChoices())
            {
                player_color_ids[0] = (player_color_ids[0] + 1) % color_options.Length;
            }
        }   
    }

}


public class MatchStats
{
    public Color[] colors;
    public int[] scores;
}