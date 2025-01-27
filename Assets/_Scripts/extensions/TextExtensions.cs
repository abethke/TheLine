using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public static class TextExtensions
{
    static public Coroutine FadeIn(this TMP_Text in_text, float in_duration)
    {
        if (!in_text.gameObject.activeInHierarchy)
            return null;

        return in_text.StartCoroutine(FadeRoutine(in_text, in_duration, 0, 1, 0));
    }
    static public Coroutine FadeOut(this TMP_Text in_text, float in_duration)
    {
        if (!in_text.gameObject.activeInHierarchy)
            return null;

        return in_text.StartCoroutine(FadeRoutine(in_text, in_duration, 1, 0, 0));
    }
    static public Coroutine FadeOut(this TMP_Text in_text, float in_duration, float in_delay)
    {
        if (!in_text.gameObject.activeInHierarchy)
            return null;

            return in_text.StartCoroutine(FadeRoutine(in_text, in_duration, 1, 0, in_delay));
    }
    static public IEnumerator FadeRoutine(TMP_Text in_text, float in_duration, float in_start, float in_end, float in_delay)
    {
        in_text.color = in_text.color.SetAlpha(in_start);

        float startedAt = Time.time + in_delay;
        while (Time.time - startedAt < in_duration)
        {
            float percent = Mathf.Clamp01((Time.time - startedAt) / in_duration);
            float value = in_start + (in_end - in_start) * percent;
            in_text.color = in_text.color.SetAlpha(value);

            yield return new WaitForFixedUpdate();
        }

        in_text.color = in_text.color.SetAlpha(in_end);
    }
}
