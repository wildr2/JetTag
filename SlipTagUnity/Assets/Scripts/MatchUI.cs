using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MatchUI : MonoBehaviour
{
    public Transform chase_screen;
    public Image chase_arrow, chase_runner, chase_chaser;
    public Image chase_background;

    public Transform tag_screen;
    public Image tag_runner, tag_chaser;
    public Text tag_text, tag_continue_text;

    public Text runner_score_txt, chaser_score_txt;


    public void ShowChaseScreen(Chara chaser, Chara runner)
    {
        chase_screen.gameObject.SetActive(true);
        chase_background.color = chaser.PlayerColor;

        // Arrow
        Vector2 dif = runner.transform.position - chaser.transform.position;
        Vector2 pos = (Vector2)runner.transform.position - dif * 0.25f;

        chase_arrow.transform.position = Camera.main.WorldToScreenPoint(pos);
        chase_arrow.transform.rotation = Quaternion.Euler(0, 0, 
            Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg);

        // Balls
        chase_chaser.color = chaser.PlayerColor;
        chase_chaser.transform.position = Camera.main.WorldToScreenPoint(chaser.transform.position);
        chase_runner.transform.position = Camera.main.WorldToScreenPoint(runner.transform.position);
    }
    public void HideChaseScreen()
    {
        chase_screen.gameObject.SetActive(false);
    }
    
    public void ShowTagScreen(Chara winner, int[] scores)
    {
        GameManager gm = GameManager.Instance;
        Chara chaser = gm.GetChaser();
        Chara runner = gm.GetRunner();

        tag_screen.gameObject.SetActive(true);
        tag_text.color = chaser.PlayerColor;

        // Score
        if (chaser.PlayerID == 0)
        {
            chaser_score_txt.rectTransform.localPosition = new Vector3(-500, 0, 0);
            runner_score_txt.rectTransform.localPosition = new Vector3(500, 0, 0);
        }
        else
        {
            chaser_score_txt.rectTransform.localPosition = new Vector3(500, 0, 0);
            runner_score_txt.rectTransform.localPosition = new Vector3(-500, 0, 0);
        }
        chaser_score_txt.color = chaser.PlayerColor;
        chaser_score_txt.text = gm.GetScores()[chaser.PlayerID].ToString();
        runner_score_txt.text = gm.GetScores()[runner.PlayerID].ToString();


        // Continue text
        tag_continue_text.color = winner.PlayerColor;
        string control = InputExt.GetControlName(winner.PlayerID, Control.Action);
        if (control != "")
            tag_continue_text.text = "press " + control;
        else
            tag_continue_text.text = "";
    }
    public void HideTagScreen()
    {
        tag_screen.gameObject.SetActive(false);
    }
}
