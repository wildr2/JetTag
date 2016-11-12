using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource music;
    float recent_playtime = 0;


    private void Awake()
    {
        music = GetComponent<AudioSource>();
        GameManager.Instance.on_tag += OnTag;

        music.Play();
        music.Pause();
    }

    private void OnTag(Chara tagger, Chara target)
    {
        StopAllCoroutines();
        StartCoroutine(PlaySnippet());
    }
    private IEnumerator PlaySnippet()
    {
        yield return new WaitForSeconds(0.5f);

        music.UnPause();

        float duration = Mathf.Max(5, 15 - recent_playtime);        

        if (music.time >= 0.5f)
        {
            // Fade in if not at start of song
            while (music.volume < 1)
            {
                music.volume += Time.deltaTime;
                yield return null;
            }
        }

        // Fade out over time - less time if stopped only recently
        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            music.volume = 1 - Mathf.Pow(t, 2);
            yield return null;
        }

        music.Pause();
    }
    private void Update()
    {
        if (music.volume > 0 && music.isPlaying)
        {
            recent_playtime += Time.deltaTime;
        }
        else
        {
            recent_playtime = Mathf.Max(0, recent_playtime - Time.deltaTime);
        }
    }
}
