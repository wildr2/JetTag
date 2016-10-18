using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePage : MenuPage
{
    public PauseController pause_controller;
    public Image runner_img, chaser_img;
    public Text runner_score_txt, chaser_score_txt;

    public override void SetIn()
    {
        base.SetIn();

        GameManager gm = GameManager.Instance;
        Chara chaser = gm.GetChaser();
        Chara runner = gm.GetRunner();

        chaser_img.transform.position = Camera.main.WorldToScreenPoint(chaser.transform.position);
        runner_img.transform.position = Camera.main.WorldToScreenPoint(runner.transform.position);

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
        chaser_score_txt.text = gm.GetScores()[chaser.PlayerID].ToString();
        runner_score_txt.text = gm.GetScores()[runner.PlayerID].ToString();
    }

    public void ButtonResume()
    {
        pause_controller.UnPause();
    }
    public void ButtonRestart()
    {
        LoadGame();
    }
    public void ButtonEndMatch()
    {
        LoadMainMenu();
    }

    private void LoadGame()
    {
        GameManager.Instance.LoadGame();
    }
    private void LoadMainMenu()
    {
        GameManager.Instance.LoadMenu();
    }
}
