using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreditsPage : MenuPage
{
    public ScrollRect scroll;

    protected override void Update()
    {
        float h = scroll.horizontalNormalizedPosition;
        float input = Input.GetAxisRaw("Horizontal");

        scroll.horizontalNormalizedPosition = 
            Mathf.Clamp01(h + input * Time.deltaTime);

        base.Update();
    }
}
