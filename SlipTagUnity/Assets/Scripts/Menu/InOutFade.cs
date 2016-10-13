using UnityEngine;
using System.Collections;

public class InOutFade : PageInOutEffect
{
    public float in_duration = 1;
    public float out_duration = 1;

    protected override IEnumerator OnIn()
    {
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += Time.unscaledDeltaTime / in_duration;
            canvas_group.alpha = 1 - Mathf.Pow(1 - t, 2);
        }
        canvas_group.alpha = 1;
    }
    protected override IEnumerator OnOut()
    {
        float t = 0;
        while (t < 1)
        {
            yield return null;
            t += Time.unscaledDeltaTime / out_duration;
            canvas_group.alpha = 1 - Mathf.Pow(t, 2);
        }
        canvas_group.alpha = 0;
    }
    protected override void PrewarmIn()
    {
        canvas_group.alpha = 1;
    }
    protected override void PrewarmOut()
    {
        canvas_group.alpha = 0;
    }
}
