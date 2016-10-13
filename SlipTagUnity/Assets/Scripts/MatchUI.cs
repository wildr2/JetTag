using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchUI : MonoBehaviour
{
    public Transform chase_screen;
    public Image chase_arrow, chase_runner, chase_chaser;
    public Image chase_background;

    public Transform point_screen;
    public Text point_text;


    public void ShowChaseScreen(Chara chaser, Chara runner)
    {
        chase_screen.gameObject.SetActive(true);
        chase_background.color = chaser.PlayerColor;
        chase_arrow.transform.position = Camera.main.WorldToScreenPoint(runner.transform.position / 1.5f);
        chase_arrow.transform.rotation = Quaternion.Euler(0, 0, 
            Mathf.Atan2(runner.transform.position.y, runner.transform.position.x) * Mathf.Rad2Deg);

        chase_chaser.color = chaser.PlayerColor;
        chase_chaser.transform.position = Camera.main.WorldToScreenPoint(chaser.transform.position);
        chase_runner.transform.position = Camera.main.WorldToScreenPoint(runner.transform.position);
    }
    public void HideChaseScreen()
    {
        chase_screen.gameObject.SetActive(false);
    }
    
    public void ShowPointScreen(Chara winner, int[] scores)
    {
        GameManager gm = GameManager.Instance;

        point_screen.gameObject.SetActive(true);
        point_text.color = winner.PlayerColor;
        point_text.text = "GAME ";

        for (int i = 0; i < scores.Length; ++i)
        {
            point_text.text += scores[i] + ":";    
        }

        point_text.text = point_text.text.TrimEnd(':');
        if (winner.PlayerID == 0) point_text.text = "<-  " + point_text.text;
        else point_text.text = point_text.text = point_text.text + "  ->";
    }
    public void HidePointScreen()
    {
        point_screen.gameObject.SetActive(false);
    }
}
