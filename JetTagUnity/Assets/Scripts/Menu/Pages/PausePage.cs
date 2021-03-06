﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePage : MenuPage
{
    public PauseController pause_controller;
    public Image runner_img, chaser_img;
    public Text runner_score_txt, chaser_score_txt;
    public CanvasGroup tag_screen;

    private Color original_background_color;


    public override void SetIn()
    {
        base.SetIn();

        GameManager gm = GameManager.Instance;
        Chara chaser = gm.GetChaser();
        Chara runner = gm.GetRunner();

        gm.HideCourt();

        // Balls
        chaser.SetStyle(Color.black, null, Color.black);
        runner.SetStyle(Color.white, Color.black, Color.black);

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

        // Hide UI behind this page
        tag_screen.alpha = 0;
    }
    public override void SetOut()
    {
        GameManager gm = GameManager.Instance;
        Chara chaser = gm.GetChaser();
        Chara runner = gm.GetRunner();

        // Balls
        chaser.SetStyle(chaser.PlayerColor);
        runner.SetStyle(Color.white);

        // Unhide UI behind this page
        tag_screen.alpha = 1;
        if (gm.State == MatchState.InPlay) gm.ShowCourt();

        base.SetOut();
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
