using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public abstract class PageInOutEffect : MonoBehaviour
{
    protected CanvasGroup canvas_group;
    public float in_delay = 0;
    public float out_delay = 0;


    protected virtual void Awake()
    {
        canvas_group = GetComponent<CanvasGroup>();
        MenuPage page = GetComponentInParent<MenuPage>();

        if (page == null) Debug.LogWarning("No page found in parents of " + transform.name);
        else if (page.deactivate_delay <= 0) Debug.LogWarning("Page deactivate delay too short");
        else
        {
            page.on_in += new System.Action(() =>
            {
                StopAllCoroutines();
                StartCoroutine(InRoutine());
            });
            page.on_out += new System.Action(() =>
            {
                StopAllCoroutines();
                StartCoroutine(OutRoutine());
            });
        }
    }
    protected virtual IEnumerator OnIn()
    {
        yield return null;
    }
    protected virtual IEnumerator OnOut()
    {
        yield return null;
    }
    protected virtual void PrewarmIn()
    {

    }
    protected virtual void PrewarmOut()
    {

    }

    private IEnumerator InRoutine()
    {
        PrewarmOut();

        // Delay
        if (in_delay > 0)
        {
            IEnumerator delay = CoroutineUtil.WaitForRealSeconds(in_delay);
            while (delay.MoveNext()) yield return delay.Current;
        }

        StartCoroutine(OnIn());
    }
    private IEnumerator OutRoutine()
    {
        PrewarmIn();

        // Delay
        if (out_delay > 0)
        {
            IEnumerator delay = CoroutineUtil.WaitForRealSeconds(out_delay);
            while (delay.MoveNext()) yield return delay.Current;
        }

        StartCoroutine(OnOut());
    }
}
