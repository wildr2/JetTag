using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FullscreenButton : Button
{
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(UpdateEnabled());
    }
    private IEnumerator UpdateEnabled()
    {
        while (true)
        {
            if (!Screen.fullScreen && !interactable)
            {
                interactable = true;
            }
            else if (Screen.fullScreen && interactable)
            {
                interactable = false;
            }
            yield return null;
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        // Must be on pointer down for browsers to allow
        Screen.fullScreen = true;

        base.OnPointerDown(eventData);
    }
}
