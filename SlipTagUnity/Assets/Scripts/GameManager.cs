using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public enum CamShakeType { Strong, StrongNoF, VeryStrong }
public enum MatchState { InPlay, Tagged, TurnChange }

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

    public Chara[] charas;
    public MatchUI match_ui;
    public Transform court;
    private static UID ui_timescale_id = new UID();

    public MatchState State { get; private set; }
    private float turn_start_time;
    private float turn_length = 10;
    private int turn_num = 0;
    private int[] scores;

    public Action on_reset;


    // PUBLIC ACCESSORS

    public Chara GetChaser()
    {
        return charas[0].IsChaser() ? charas[0] : charas[1];
    }
    public Chara GetRunner()
    {
        return charas[0].IsChaser() ? charas[1] : charas[0];
    }
    public int[] GetScores()
    {
        return scores;
    }


    // PUBLIC MODIFIERS

    public void LoadGame()
    {
        EndMatch();
        SceneManager.LoadScene("Game");
    }
    public void LoadMenu()
    {
        EndMatch();
        SceneManager.LoadScene("Menu");
    }
    public void EndMatch()
    {
        MatchStats stats = new MatchStats();
        stats.colors = new Color[] { charas[0].PlayerColor, charas[1].PlayerColor };
        stats.scores = new int[] { scores[0], scores[1] };
        DataManager.Instance.match_stats.Add(stats);
    }


    // PRIVATE / PROTECTED MODIFIERS

    private void Awake()
    {
        // Singleton
        if (this != _instance)
        {
            _instance = this; // new instance becomes the singleton
            return;
        }
    }
    private void Start()
    {
        if (this != _instance) return;

        DataManager dm = DataManager.Instance;

        // Cam Shake
        CameraShake camshake = Camera.main.GetComponent<CameraShake>();
        camshake.DefineShakeType(CamShakeType.Strong, new CamShakeParams(0.1f, 3, 1, 4));
        camshake.DefineShakeType(CamShakeType.StrongNoF, new CamShakeParams(0.15f, 4, 1, 0));
        camshake.DefineShakeType(CamShakeType.VeryStrong, new CamShakeParams(0.4f, 6, 1, 5));

        // Characters
        for (int i = 0; i < charas.Length; ++i)
        {
            charas[i].Initialize(i, dm.GetPlayerColor(i));
            charas[i].on_tag += OnTag;
        }

        // Scores
        scores = new int[charas.Length];

        // turns
        StartCoroutine(StartTurn());
    }
    private void Update()
    {
        if (State == MatchState.InPlay)
        {
            if (Time.timeSinceLevelLoad - turn_start_time >= turn_length)
            {
                StartNextTurn();
            }
        }
    }

    private void StartNextTurn()
    {
        ++turn_num;
        StartCoroutine(StartTurn());
    }
    private IEnumerator StartTurn()
    {
        State = MatchState.TurnChange;

        int chaser_i = turn_num % 2;
        //Tools.Log(string.Format("turn {0}: chaser is p{1}", turn_num, chaser_i));

        // Set chara roles
        charas[chaser_i].SetChaser();
        for (int i = 0; i < charas.Length; ++i)
        {
            if (i != chaser_i) charas[i].SetRunner();
        }

        // Flash color
        TimeScaleManager.SetFactor(0, ui_timescale_id);
        match_ui.ShowChaseScreen(charas[chaser_i], charas[1 - chaser_i]);
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));
        match_ui.HideChaseScreen();
        TimeScaleManager.SetFactor(1, ui_timescale_id);

        turn_start_time = Time.timeSinceLevelLoad;
        State = MatchState.InPlay;
    }
    private void OnTag(Chara tagger, Chara target)
    {
        StartCoroutine(OnTagRoutine(tagger));
    }

    private IEnumerator OnTagRoutine(Chara winner)
    {
        // Wait for end of turn change if happened on same frame as turn change
        while (State == MatchState.TurnChange) yield return null;

        // State and score
        State = MatchState.Tagged;
        ++scores[winner.PlayerID];

        
        court.gameObject.SetActive(false);
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));

        // Show UI
        yield return null; // let collision adjustment happen
        match_ui.ShowTagScreen(winner, scores);
        //yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));

        charas[0].ShowReplay();
        charas[1].ShowReplay();

        // Wait
        if ((ControlScheme)InputExt.GetPlayerScheme(winner.PlayerID) == ControlScheme.AI)
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(4f));
        else
            while (!InputExt.GetKeyDown(winner.PlayerID, Control.Action)) yield return null;

        // Hide UI
        court.gameObject.SetActive(true);
        match_ui.HideTagScreen();

        // Reset
        if (on_reset != null) on_reset();

        // Next turn
        StartNextTurn();
    }
}
