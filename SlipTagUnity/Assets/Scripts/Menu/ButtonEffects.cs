using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonEffects : ButtonEvents
{
    private Text text;
    private string label;

    private void Awake()
    {
        text = GetComponent<Text>();
        if (text != null) label = text.text;
    }

    protected override void OnClick()
    {
        base.OnClick();
        if (text != null)
        {
            text.text = label;
        }
    }
    protected override void OnSelect()
    {
        base.OnSelect();

        if (text != null)
        {
            label = text.text;
            text.text = "> " + label + " <";
        }   
    }
    protected override void OnDeselect()
    {
        base.OnDeselect();

        if (text != null)
        {
            text.text = label;
        }
    }

}
