using UnityEngine;
using System.Collections;

public class MovingCam : MonoBehaviour
{
    // Zoom
    private float ortho_size_zoomed_out;
    private float ortho_size_zoomed_in;
    private bool zoomed_in = false;
    private IEnumerator zoom_coroutine = null;
    private const float zoom_speed = 3f;

    // Follow
    public Transform follow_target;
    private const float follow_speed = 5f;


    public void SetZoomLevels(float ortho_size_in, float ortho_size_out)
    {
        this.ortho_size_zoomed_in = ortho_size_in;
        this.ortho_size_zoomed_out = ortho_size_out;
    }
    public void SetZoom(bool zoomed_in)
    {
        this.zoomed_in = zoomed_in;
        Camera.main.orthographicSize = zoomed_in ? ortho_size_zoomed_in : ortho_size_zoomed_out;
    }

    private void Update()
    {
        // zoom
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleZoomMode();
        }

        // follow
        if (!zoomed_in)
        {
            Vector2 pos = transform.position;
            pos = Vector2.Lerp(pos, Vector2.zero, Time.deltaTime * follow_speed);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
        else if (follow_target != null)
        {
            Vector2 pos = transform.position;
            pos = Vector2.Lerp(pos, follow_target.transform.position, Time.deltaTime * follow_speed);
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }
    }
    private void ToggleZoomMode()
    {
        zoomed_in = !zoomed_in;
        if (zoom_coroutine != null) StopCoroutine(zoom_coroutine);
        zoom_coroutine = Zoom(zoomed_in ? ortho_size_zoomed_in : ortho_size_zoomed_out);
        StartCoroutine(zoom_coroutine);
    }
    private IEnumerator Zoom(float target_ortho_size)
    {
        float initial_ortho = Camera.main.orthographicSize;
        float dist = 1f / (Mathf.Abs(target_ortho_size - initial_ortho) / (ortho_size_zoomed_out - ortho_size_zoomed_in));

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * dist * zoom_speed;
            Camera.main.orthographicSize = Mathf.Lerp(initial_ortho, target_ortho_size, 1 - Mathf.Pow(1-t, 2f));
            yield return null;
        }
        Camera.main.orthographicSize = target_ortho_size;
    }
}
