using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuPage : MenuPage
{
    public Image[] balls;
    public Text[] controls_text;
    private string controls_text_initial = "press start\n" + "or right shift\n" + "or left shift";
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
    }
    protected override void Update()
    {
        base.Update();

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
    private void OnInputStart(ControlScheme cs)
    {
        // Slot already with this cs
        for (int i = 0; i < 2; ++i)
        {
            if ((ControlScheme)InputExt.GetPlayerScheme(i) == cs)
            {
                InputExt.SetPlayerControlScheme(i, ControlScheme.None);
                UpdateControlsText(i);
                return;
            }
        }

        // Assign cs to empty slot
        for (int i = 0; i < 2; ++i)
        {
            if ((ControlScheme)InputExt.GetPlayerScheme(i) == ControlScheme.None)
            {
                InputExt.SetPlayerControlScheme(i, cs);
                UpdateControlsText(i);
                return;
            }
        }
    }
    private void UpdateControlsText(int player_id)
    {
        if ((ControlScheme)InputExt.GetPlayerScheme(player_id) == ControlScheme.None)
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
        int i = dm.player_color_ids[player_id];
        i = Tools.Mod(i + index_change, dm.color_options.Length);
        dm.player_color_ids[player_id] = i;

        UpdateBallColor(player_id);
    }
    private void UpdateBallColor(int player_id)
    {
        balls[player_id].color = dm.color_options[dm.player_color_ids[player_id]];
    }

}
