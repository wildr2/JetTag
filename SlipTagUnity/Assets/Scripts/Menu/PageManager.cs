using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PageManager : MonoBehaviour
{
    private EventSystem event_sys;
    private Vector3 mouse_last;
    private bool using_mouse = false;

    public GraphicRaycaster raycaster;
    public MenuPage[] in_on_start_pages;
    public MenuPage[] out_on_start_pages;
    


	// PUBLIC ACCESSORS

    public EventSystem GetEventSystem()
    {
        return event_sys;
    }
    public bool IsUsingMouse()
    {
        return using_mouse;
    }
    public MenuPage GetTopmostPage()
    {
        MenuPage[] pages = transform.GetComponentsInChildren<MenuPage>();
        if (pages.Length == 0) return null;
        return pages[pages.Length - 1];
    }


	// PUBLIC MODIFIERS


	// PRIVATE / PROTECTED MODIFIERS

    private void Awake()
    {
        event_sys = FindObjectOfType<EventSystem>();
        mouse_last = Input.mousePosition;

        DisableMouseControl();

        // Initialize pages
        foreach (MenuPage page in GetComponentsInChildren<MenuPage>(true))
        {
            page.gameObject.SetActive(true);
            page.gameObject.SetActive(false);
            page.Initialize(this);
        }
    }
    private void Start()
    {
        // Starting pages
        foreach (MenuPage page in in_on_start_pages)
        {
            page.SetIn();
        }
        foreach (MenuPage page in out_on_start_pages)
        {
            page.SetIn();
            page.SetOut();
        }
    }
    private void Update()
    {
        // Switch to gamepad / keyboard control
        bool navigation_input = Input.GetButtonDown("Submit") || Input.GetAxisRaw("Vertical") != 0
            || Input.GetAxisRaw("Horizontal") != 0 || Input.GetButtonDown("Cancel");

        if (navigation_input && event_sys.currentSelectedGameObject == null)
        {
            DisableMouseControl();
        }

        // Switch to mouse control
        if ((Input.mousePosition - mouse_last).magnitude > 0.5f)
        {
            EnableMouseControl();
        }
        mouse_last = Input.mousePosition;
    }
    private void EnableMouseControl()
    {
        using_mouse = true;
        raycaster.enabled = true;

        event_sys.SetSelectedGameObject(null);
    }
    private void DisableMouseControl()
    {
        using_mouse = false;
        raycaster.enabled = false;

        MenuPage top_page = GetTopmostPage();
        if (top_page != null && top_page.IsIn())
            event_sys.SetSelectedGameObject(top_page.first_selected);
    }

}
