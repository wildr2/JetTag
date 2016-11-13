using UnityEngine;
using System.Collections;

public class Waypoints : MonoBehaviour
{
    public Vector2[] Points { get; private set; }
    public bool hide = true;


    public bool HasLOS(Vector2 pos, Vector2 target)
    {
        Vector2 v = target - pos;
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, v.normalized, v.magnitude);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Wall")) return false;
        }
        return true;
    }

    public Vector2 FindPathNextWP(Vector2 pos, Vector2 target)
    {        
        if (HasLOS(pos, target)) return target;

        bool[] inpath = new bool[Points.Length];

        int attempts = 0;
        while (attempts < 50)
        {
            int next_wp_i = ClosestWithLOSToTarget(pos, target, inpath);
            inpath[next_wp_i] = true;
            Vector2 next_wp = Points[next_wp_i];

            Debug.DrawLine(target, next_wp, Color.green, 0.1f);
            if (HasLOS(pos, next_wp))
            {
                Debug.DrawLine(pos, next_wp, Color.blue, 0.1f);
                return next_wp;
            }
            target = next_wp;

            ++attempts;
        }
        return pos;
    }


    private void Awake()
    {
        Points = new Vector2[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            Points[i] = child.position;
            if (hide) child.gameObject.SetActive(false);
        }
    }
    private int ClosestWithLOSToTarget(Vector2 pos, Vector2 target, bool[] inpath)
    {
        int best_wp_i = 0;
        float best_dist = float.MaxValue;

        for (int i = 0; i < Points.Length; ++i)
        {
            Vector2 wp = Points[i];
            if (inpath[i]) continue;
            if (!HasLOS(wp, target)) continue;

            float dist = Vector2.Distance(wp, pos);
            if (dist < best_dist)
            {
                best_wp_i = i;
                best_dist = dist;
            }
        }
        return best_wp_i;
    }  
}
