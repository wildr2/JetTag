using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonEffects : ButtonEvents
{
    private Text text;

    protected override void Awake()
    {
        text = GetComponent<Text>();
        base.Awake();
    }

    protected override void OnClick()
    {
        base.OnClick();
        SoundManager.PlayClickSound();
        //if (text != null) if (text != null) text.text = RemoveArrows(text.text);
    }
    protected override void OnSelect()
    {
        base.OnSelect();
        SoundManager.PlaySelectSound();
        if (text != null) if (text != null) text.text = AddArrows(text.text);
    }
    protected override void OnDeselect()
    {
        base.OnDeselect();
        if (text != null) text.text = RemoveArrows(text.text);
    }

    public static string AddArrows(string s)
    {
        return "> " + s.Trim(new char[] { '>', '<', ' ' }) + " <";
    }
    public static string RemoveArrows(string s)
    {
        return s.Trim(new char[] { '>', '<', ' ' });
    }
    public static bool HasArrows(string s)
    {
        return s != "" && s[0] == '>' && s[s.Length-1] == '<';
    }

}
