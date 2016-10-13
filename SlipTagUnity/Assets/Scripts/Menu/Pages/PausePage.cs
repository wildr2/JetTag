using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePage : MenuPage
{
    public PauseController pause_controller;
    public Image runner_img, chaser_img;

    public override void SetIn()
    {
        base.SetIn();

        GameManager gm = GameManager.Instance;
        Chara chaser = gm.GetChaser();
        Chara runner = gm.GetRunner();

        chaser_img.color = chaser.PlayerColor;
        chaser_img.transform.position = Camera.main.WorldToScreenPoint(chaser.transform.position);
        runner_img.transform.position = Camera.main.WorldToScreenPoint(runner.transform.position);
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
        //pause_controller.UnPause();
        Time.timeScale = 1;
        GameManager.Instance.LoadGame();
    }
    private void LoadMainMenu()
    {
        //pause_controller.UnPause();
        Time.timeScale = 1;
        GameManager.Instance.LoadMenu();
    }
}
