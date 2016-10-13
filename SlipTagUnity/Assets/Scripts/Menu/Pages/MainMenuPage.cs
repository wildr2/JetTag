using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuPage : MenuPage
{
    public Image[] balls;
    public Text[] controls_text;

    private DataManager dm;

    private string controls_text_initial = "press start\n" + "or right shift\n" + "or left shift";


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
        foreach (ControlScheme cs in Tools.EnumValues(typeof(ControlScheme)))
        {
            if (Input.GetKeyDown(KeyStart(cs)))
                OnControlsStart(cs);
        }

        // Color switching
        for (int i = 0; i < dm.player_controls.Length; ++i)
        {
            if (Input.GetKeyDown(KeyLeft(dm.player_controls[i]))) SwitchColor(i, -1);
            if (Input.GetKeyDown(KeyRight(dm.player_controls[i]))) SwitchColor(i, 1);
        }
    }
    private void OnControlsStart(ControlScheme cs)
    {
        // Expty slot already with this cs
        for (int i = 0; i < dm.player_controls.Length; ++i)
        {
            if (dm.player_controls[i] == cs)
            {
                dm.player_controls[i] = ControlScheme.None;
                UpdateControlsText(i);
                return;
            }
        }

        // Assign cs to empty slot
        for (int i = 0; i < dm.player_controls.Length; ++i)
        {
            if (dm.player_controls[i] == ControlScheme.None)
            {
                dm.player_controls[i] = cs;
                UpdateControlsText(i);
                return;
            }
        }
    }
    private void UpdateControlsText(int player_id)
    {
        if (dm.player_controls[player_id] == ControlScheme.None)
        {
            controls_text[player_id].text = controls_text_initial;
        }
        else
        {
            controls_text[player_id].text = dm.player_controls[player_id].ToString();
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

    private KeyCode KeyStart(ControlScheme cs)
    {
        switch (cs)
        {
            case ControlScheme.Arrows: return KeyCode.RightShift;
            case ControlScheme.WASD: return KeyCode.LeftShift;
            default: return KeyCode.None;
        }
    }
    private KeyCode KeyLeft(ControlScheme cs)
    {
        switch (cs)
        {
            case ControlScheme.Arrows: return KeyCode.LeftArrow;
            case ControlScheme.WASD: return KeyCode.A;
            default: return KeyCode.None;
        }
    }
    private KeyCode KeyRight(ControlScheme cs)
    {
        switch (cs)
        {
            case ControlScheme.Arrows: return KeyCode.RightArrow;
            case ControlScheme.WASD: return KeyCode.D;
            default: return KeyCode.None;
        }
    }

    //private IEnumerator BeginMatch()
    //{
    //    SetOut();

    //    //yield return new WaitForSeconds(0.4f);

    //    //for (int i = 0; i < balls.Length; ++i)
    //    //    balls[i].transform.parent.gameObject.SetActive(false);

    //    //yield return new WaitForSeconds(0.4f);

    //    SceneManager.LoadScene("Game");
    //}

}
