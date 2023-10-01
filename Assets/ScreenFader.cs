using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Image image;


    private void Awake()
    {
        instance = this;
        image = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        StartCoroutine(FadeIn(1f));
    }

    public IEnumerator FadeIn(float time)
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / time;
            float a = curve.Evaluate(t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
    }

    public IEnumerator FadeOut(float time)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float a = curve.Evaluate(t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOutToScene(scene));
    }

    public IEnumerator FadeOutToScene(string scene)
    {
        yield return FadeOut(1f);
        SceneManager.LoadScene(scene);
    }

    public void FadeToNextScene()
    {
        StartCoroutine(FadeOutToNextScene());
    }

    public IEnumerator FadeOutToNextScene()
    {
        yield return FadeOut(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
