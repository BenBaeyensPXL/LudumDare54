using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    public float speed = 1f;
    public float scale = 1f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        float s = curve.Evaluate(t) * scale;
        transform.localScale = originalScale + Vector3.one * s;
    }
}
