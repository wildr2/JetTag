using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class MenuPage : MonoBehaviour
{
    // References
    protected PageManager manager;
    protected CanvasGroup canvas_group;

    // General
    public float deactivate_delay = 1;
    public GameObject first_selected;
    public Button on_cancel_input;
    public bool sound_on_cancel = true;

    // State
    protected bool is_in = false;
    private Coroutine deactivate_routine;

    // Events
    public Action on_in, on_out, on_in_once, on_out_once;


    // PUBLIC ACCESSORS

    public PageManager GetManager()
    {
        return manager;
    }

    public bool IsIn()
    {
        return is_in;
    }
    public bool IsInteractable()
    {
        return canvas_group.interactable;
    }
    public bool IsTopmost()
    {
        return manager.GetTopmostPage() == this;
    }

    public virtual void Initialize(PageManager manager)
    {
        this.manager = manager;
        canvas_group = GetComponent<CanvasGroup>();
    }
    public virtual void SetIn()
    {
        // Make active topmost page
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        if (deactivate_routine != null) StopCoroutine(deactivate_routine);

        // State
        is_in = true;
        SetInteractable(true);

        // Events
        if (on_in != null) on_in();
        if (on_in_once != null)
        {
            on_in_once();
            on_in_once = null;
        }
    }
    public virtual void SetOut()
    {
        // Make bottommost page
        transform.SetAsFirstSibling();

        // State
        is_in = false;
        SetInteractable(false);

        // Deactivation
        if (deactivate_delay <= 0) gameObject.SetActive(false);
        else
        {
            deactivate_routine = StartCoroutine(CoroutineUtil.DoAfterDelay(
                () => gameObject.SetActive(false), deactivate_delay));
        }

        // Events
        if (on_out != null) on_out();
        if (on_out_once != null)
        {
            on_out_once();
            on_out_once = null;
        }
    }
    public void SetInteractable(bool interactable)
    {
        if (interactable)
        {
            StartCoroutine(CoroutineUtil.DoNextFrame(() => { canvas_group.interactable = interactable; }));

            if (!manager.IsUsingMouse() && first_selected != null)
                manager.GetEventSystem().SetSelectedGameObject(first_selected);
        }
        else
        {
            canvas_group.interactable = interactable;
        }
    }


    // PRIVATE MODIFIERS

    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        if (manager == null) Debug.LogError("Missing PageManager");
    }
    protected virtual void Update()
    {
        if (IsInteractable())
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (on_cancel_input != null) on_cancel_input.onClick.Invoke();
                OnInputCancel();
            }
            else if (Input.GetButtonDown("Submit"))
            {
                OnInputSubmit();
            }
        }
    }
    protected virtual void OnInputCancel()
    {
        if (sound_on_cancel)
        {
            SoundManager.PlayClickSound();
        }
    }
    protected virtual void OnInputSubmit()
    {

    }
}
