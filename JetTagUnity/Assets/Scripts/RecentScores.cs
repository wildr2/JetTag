using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RecentScores : MonoBehaviour
{
    private Text text;

    private void Start()
    {
        DataManager dm = DataManager.Instance;
        text = GetComponent<Text>();

        if (dm.match_stats.Count == 0)
        {
            text.text = "";
            return;
        }

        text.text = "";
        for (int i = dm.match_stats.Count-1; i >= Mathf.Max(0, dm.match_stats.Count - 10); --i)
        {
            MatchStats ms = dm.match_stats[i];
            string score;
            score = Tools.ColorRichTxt(ms.scores[0].ToString(), ms.colors[0]);
            score += ":";
            score += Tools.ColorRichTxt(ms.scores[1].ToString(), ms.colors[1]);
            if (i > 0) score += "    ";
            text.text += score;
        }
    }
}
