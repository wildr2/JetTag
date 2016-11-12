using UnityEngine;
using System.Collections;

public enum Power { None, Dash, Blink, Swap, Springs, Warp, Cloak, Repel }

public class PickupManager : MonoBehaviour
{
    public bool debug = false;
    private Vector2[] power_centers;
    private int hw = 16, hh = 9;

    public void SetPickupPower(Pickup pu)
    {
        pu.power = (Power)ClosestPowerIndex(pu.transform.position);
    }

    private void Awake()
    {
        Power[] pows = (Power[])Tools.EnumValues(typeof(Power));
        power_centers = new Vector2[pows.Length];
        for (int i = 1; i < pows.Length; ++i)
        {
            power_centers[i] = new Vector2(Random.Range(-hw, hw), Random.Range(-hh, hh));
        }

        if (debug)
        {
            foreach (Vector2 pc in power_centers)
            {
                Tools.DebugDrawPlus(pc, Color.red, 0.5f, 1000);
            }
            for (float x = -hw; x < hw; x += 0.2f)
            {
                for (float y = -hh; y < hh; y += 0.2f)
                {
                    Vector2 pos = new Vector2(x, y);
                    int i = ClosestPowerIndex(pos);
                    Tools.DebugDrawPlus(pos, DataManager.Instance.color_options[i], 0.2f, 1000);
                }
            }

        }
    }
    private int ClosestPowerIndex(Vector2 pos)
    {
        int index = 0;
        float min_dist = float.MaxValue;
        for (int i = 1; i < power_centers.Length; ++i)
        {
            float dist = Vector2.Distance(pos, power_centers[i]);
            if (dist <= min_dist)
            {
                min_dist = dist;
                index = i;
            }
        }

        return index;
    }
}
