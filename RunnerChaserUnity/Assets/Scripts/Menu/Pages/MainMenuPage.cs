using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuPage : MonoBehaviour
{
    public Image[] balls;
    private int[] color_indices;
    private Color[] color_options = new Color[] { Color.red, Color.green, Color.blue };

    public Text[] controls_text;
    private ControlScheme[] control_schemes;
   
    private string controls_text_initial = "press start\n" + "or right shift\n" + "or left shift";


    private void Awake()
    {
        color_indices = new int[2];
        control_schemes = new ControlScheme[2];
        for (int i = 0; i < 2; ++i)
        {
            UpdateControlsText(i);
            UpdateBallColor(i);
        }
    }
    private void Update()
    {
        // Controls start
        foreach (ControlScheme cs in Tools.EnumValues(typeof(ControlScheme)))
        {
            if (Input.GetKeyDown(KeyStart(cs)))
                OnControlsStart(cs);
        }

        // Color switching
        for (int i = 0; i < control_schemes.Length; ++i)
        {
            if (Input.GetKeyDown(KeyLeft(control_schemes[i]))) SwitchColor(i, -1);
            if (Input.GetKeyDown(KeyRight(control_schemes[i]))) SwitchColor(i, 1);
        }
    }
    private void OnControlsStart(ControlScheme cs)
    {
        // Expty slot already with this cs
        for (int i = 0; i < control_schemes.Length; ++i)
        {
            if (control_schemes[i] == cs)
            {
                control_schemes[i] = ControlScheme.None;
                UpdateControlsText(i);
                return;
            }
        }

        // Assign cs to empty slot
        for (int i = 0; i < control_schemes.Length; ++i)
        {
            if (control_schemes[i] == ControlScheme.None)
            {
                control_schemes[i] = cs;
                UpdateControlsText(i);
                return;
            }
        }
    }
    private void UpdateControlsText(int player_id)
    {
        if (control_schemes[player_id] == ControlScheme.None)
        {
            controls_text[player_id].text = controls_text_initial;
        }
        else
        {
            controls_text[player_id].text = control_schemes[player_id].ToString();
        }
    }

    private void SwitchColor(int player_id, int index_change)
    {
        color_indices[player_id] = Tools.Mod(color_indices[player_id] + index_change,
            color_options.Length);

        UpdateBallColor(player_id);
    }
    private void UpdateBallColor(int player_id)
    {
        balls[player_id].color = color_options[color_indices[player_id]];
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

}
