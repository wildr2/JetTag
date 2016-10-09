using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Color[] player_colors;
    private Chara[] charas;
    public Transform chaser_win_ui, switch_ui;

    private float round_start_time;


	// PUBLIC ACCESSORS

	// PUBLIC MODIFIERS

	// PRIVATE / PROTECTED MODIFIERS

    private void Awake()
    {
        charas = GetComponentsInChildren<Chara>();
        for (int i = 0; i < charas.Length; ++i)
        {
            charas[i].Initialize(i, (ControlScheme)i, player_colors[i]);
            charas[i].on_collide_chara += OnCharaCollide;
        }
    }
    private void Start()
    {
        //StartCoroutine(UpdateLive());
        StartCoroutine(UpdateRounds());
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
        int round_i = 0;
        int chaser_i = 0;
        while (true)
        {
            // Set chara roles
            charas[chaser_i].SetChaser();
            for (int i = 0; i < charas.Length; ++i)
            {
                if (i != chaser_i) charas[i].SetRunner();
            }

            // Flash color
            switch_ui.gameObject.SetActive(true);
            switch_ui.GetComponentInChildren<Image>().color = charas[chaser_i].GetPlayerColor();
            switch_ui.GetComponentInChildren<Text>().text = round_i == 0 ? "CHASE" : "SWITCH";
            yield return new WaitForSeconds(0.35f);
            switch_ui.gameObject.SetActive(false);

            // Wait for round end
            round_start_time = Time.time;
            yield return new WaitForSeconds(10);

            // Next round
            chaser_i = (chaser_i + 1) % charas.Length;
            ++round_i;
        }
    }   
    private IEnumerator UpdateLive()
    {
        while (true)
        {
            foreach (Chara c in charas) c.SetLive(true);
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            foreach (Chara c in charas) c.SetLive(false);
            yield return new WaitForSeconds(Random.Range(2, 6));
        }
        //foreach (Chara c in charas) c.SetLive(true);
        //yield return new WaitForSeconds(5);
        //foreach (Chara c in charas) c.SetLive(false);
    }

    private void OnCharaCollide(Chara c)
    {
        if (c.IsChaser())
        {
            // Chaser win
            Camera.main.backgroundColor = Color.white; //GetComponent<SpriteRenderer>().color;
            chaser_win_ui.gameObject.SetActive(true);
            chaser_win_ui.GetComponentInChildren<Text>().color = c.GetPlayerColor();
            Time.timeScale = 0;
        }
    }
}
