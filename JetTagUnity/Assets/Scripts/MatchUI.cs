﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MatchUI : MonoBehaviour
{
    public Transform chase_screen;
    public Image chase_arrow, chase_background;
    public AudioSource chase_sound;

    public Transform tag_screen;
    public Text tag_text, tag_continue_text;

    public Text score_left, score_right;


    public void ShowChaseScreen(Chara chaser, Chara runner)
    {
        GameManager.Instance.HideCourt();
        chase_screen.gameObject.SetActive(true);
        chase_background.color = chaser.PlayerColor;

        // Sound
        chase_sound.Play();

        // Arrow
        Vector2 dif = runner.transform.position - chaser.transform.position;
        Vector2 pos = (Vector2)runner.transform.position - dif * 0.25f;

        chase_arrow.transform.position = pos;
        chase_arrow.transform.rotation = Quaternion.Euler(0, 0, 
            Mathf.Atan2(dif.y, dif.x) * Mathf.Rad2Deg);

        // Balls
        chaser.SetStyle(chaser.PlayerColor, Color.white, Color.white);
        runner.SetStyle(Color.white, null, Color.white);
    }
    public void HideChaseScreen(Chara chaser, Chara runner)
    {
        GameManager.Instance.ShowCourt();
        chase_screen.gameObject.SetActive(false);

        // Balls
        chaser.SetStyle(chaser.PlayerColor);
        runner.SetStyle(Color.white);
    }
    
    public void ShowTagScreen(Chara winner, int[] scores)
    {
        GameManager gm = GameManager.Instance;
        Chara chaser = gm.GetChaser();
        Chara runner = gm.GetRunner();

        gm.HideCourt();

        tag_screen.gameObject.SetActive(true);
        tag_text.color = chaser.PlayerColor;

        // Score
        score_left.color = chaser.PlayerID == 0 ? chaser.PlayerColor : Color.white;
        score_right.color = chaser.PlayerID == 1 ? chaser.PlayerColor : Color.white;
        score_left.text = gm.GetScores()[0].ToString();
        score_right.text = gm.GetScores()[1].ToString();

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
        GameManager.Instance.ShowCourt();
        tag_screen.gameObject.SetActive(false);
    }
}
