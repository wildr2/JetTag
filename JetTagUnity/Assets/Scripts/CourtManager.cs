using UnityEngine;
using System.Collections;

public enum CourtType { A, B, C, Random }

public class CourtManager : MonoBehaviour
{
    public static Court court;

    private void Awake()
    {
        CourtType court_type = DataManager.Instance.court_type;
        int court_num = (int)court_type;
        if (court_type == CourtType.Random)
        {
            int n = Tools.EnumLength(typeof(CourtType));
            int r = Random.Range(1, n);
            court_num = (court_num + r) % n;
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (i == court_num)
            {
                court = transform.GetChild(i).gameObject.GetComponent<Court>();
                court.gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
