using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchUI : MonoBehaviour
{
    public Transform chase_screen;
    public Image chase_arrow;
    public Image chase_background;

    public Transform point_screen;
    public Text point_text;


    public void ShowChaseScreen(Color color, Transform agent)
    {
        chase_screen.gameObject.SetActive(true);
        chase_background.color = color;
        chase_arrow.transform.position = agent.transform.position / 1.5f;
        chase_arrow.transform.rotation = Quaternion.Euler(0, 0, 
            Mathf.Atan2(agent.transform.position.y, agent.transform.position.x) * Mathf.Rad2Deg);
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
