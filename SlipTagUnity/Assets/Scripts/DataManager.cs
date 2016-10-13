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
    [System.NonSerialized] public ControlScheme[] player_controls;
    [System.NonSerialized] public int[] player_color_ids = { 0, 0 };
    

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
        player_controls = new ControlScheme[2];
        player_controls[0] = ControlScheme.None;
        player_controls[1] = ControlScheme.None;

        player_color_ids = new int[2];
        player_color_ids[0] = Random.Range(0, color_options.Length);
        player_color_ids[1] = Random.Range(0, color_options.Length);
    }

}
