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
        if (text != null) if (text != null) text.text = RemoveArrows(text.text);
    }
    protected override void OnSelect()
    {
        base.OnSelect();
        if (text != null) if (text != null) text.text = AddArrows(text.text);
    }
    protected override void OnDeselect()
    {
        base.OnDeselect();
        if (text != null) text.text = RemoveArrows(text.text);
    }

    private string AddArrows(string s)
    {
        return "> " + s.Trim(new char[] { '>', '<', ' ' }) + " <";
    }
    private string RemoveArrows(string s)
    {
        return s.Trim(new char[] { '>', '<', ' ' });
    }

}
