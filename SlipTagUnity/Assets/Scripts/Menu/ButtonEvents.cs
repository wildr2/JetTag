using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(EventTrigger))]
public class ButtonEvents : MonoBehaviour
{
    protected virtual void Awake()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        // Click 
        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((eventData) => { OnClick(); });
        trigger.triggers.Add(click);

        // Click (keyboard) 
        EventTrigger.Entry click_key = new EventTrigger.Entry();
        click_key.eventID = EventTriggerType.Submit;
        click_key.callback.AddListener((eventData) => { OnClick(); });
        trigger.triggers.Add(click_key);

        // Select
        EventTrigger.Entry select = new EventTrigger.Entry();
        select.eventID = EventTriggerType.PointerEnter;
        select.callback.AddListener((eventData) => { OnSelect(); });
        trigger.triggers.Add(select);

        // Select (keyboard/gamepad)
        EventTrigger.Entry select_key = new EventTrigger.Entry();
        select_key.eventID = EventTriggerType.Select;
        select_key.callback.AddListener((eventData) => { OnSelect(); });
        trigger.triggers.Add(select_key);

        // Deselect
        EventTrigger.Entry deselect = new EventTrigger.Entry();
        deselect.eventID = EventTriggerType.PointerExit;
        deselect.callback.AddListener((eventData) => { OnDeselect(); });
        trigger.triggers.Add(deselect);

        // Deselect (keyboard/gamepad)
        EventTrigger.Entry deselect_key = new EventTrigger.Entry();
        deselect_key.eventID = EventTriggerType.Deselect;
        deselect_key.callback.AddListener((eventData) => { OnDeselect(); });
        trigger.triggers.Add(deselect_key);
    }
    
    protected virtual void OnClick()
    {

    }
    protected virtual void OnSelect()
    {
    }
    protected virtual void OnDeselect()
    {
    }
}
