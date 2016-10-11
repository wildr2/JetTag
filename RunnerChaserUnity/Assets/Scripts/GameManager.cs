using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null) Tools.Log("GameManager not found");
            }
            return _instance;
        }
    }

    public Color[] player_colors;
    public Chara[] charas;
    public MatchUI match_ui;

    private Coroutine rounds_coroutine;
    private float round_start_time;
    private int round_num = 0;
    private int[] scores;

    public Action on_reset;


	// PUBLIC ACCESSORS

	// PUBLIC MODIFIERS

	// PRIVATE / PROTECTED MODIFIERS

    private void Awake()
    {
        // Singleton
        if (this != _instance)
        {
            _instance = this;
            return;
        }
    }
    private void Start()
    {
        // Characters
        for (int i = 0; i < charas.Length; ++i)
        {
            charas[i].Initialize(i, (ControlScheme)i, player_colors[i]);
            charas[i].on_tag += OnTag;
        }

        // Scores
        scores = new int[charas.Length];

        // Rounds
        rounds_coroutine = StartCoroutine(UpdateRounds());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
    }
    private IEnumerator UpdateRounds()
    {
        int chaser_i = round_num % charas.Length;
        while (true)
        {
            // Set chara roles
            charas[chaser_i].SetChaser();
            for (int i = 0; i < charas.Length; ++i)
            {
                if (i != chaser_i) charas[i].SetRunner();
            }

            // Flash color
            Time.timeScale = 0;
            match_ui.ShowChaseScreen(charas[chaser_i].PlayerColor, charas[1 - chaser_i].transform);
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));
            match_ui.HideChaseScreen();
            Time.timeScale = 1;

            // Wait for round end
            round_start_time = Time.time;
            yield return new WaitForSeconds(10);

            // Next round
            chaser_i = (chaser_i + 1) % charas.Length;
            ++round_num;
        }
    }
    private void OnTag(Chara tagger, Chara target)
    {
        StartCoroutine(OnPointRoutine(tagger));
    }

    private IEnumerator OnPointRoutine(Chara winner)
    {
        StopCoroutine(rounds_coroutine);

        // Score
        ++scores[winner.PlayerID];

        // Show UI
        Time.timeScale = 0;
        match_ui.ShowPointScreen(winner, scores);

        // Wait
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;

        // Hide UI
        match_ui.HidePointScreen();
        Time.timeScale = 1;

        // Reset
        if (on_reset != null) on_reset();

        // Next round
        ++round_num;
        rounds_coroutine = StartCoroutine(UpdateRounds());
    }
}
