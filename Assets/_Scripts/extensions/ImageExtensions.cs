using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtensions
{
    static public Coroutine FadeIn(this Image in_image, float in_duration)
    {
        return in_image.StartCoroutine(FadeRoutine(in_image, in_duration, 0, 1));
    }
    static public Coroutine FadeOut(this Image in_image, float in_duration)
    {
        return in_image.StartCoroutine(FadeRoutine(in_image, in_duration, 1, 0));
    }
    static public IEnumerator FadeRoutine(Image in_image, float in_duration, float in_start, float in_end)
    {
        in_image.color = in_image.color.SetAlpha(in_start);

        float startedAt = Time.time;
        while (Time.time - startedAt < in_duration)
        {
            float percent = Mathf.Clamp01((Time.time - startedAt) / in_duration);
            float value = in_start + (in_end - in_start) * percent;
            in_image.color = in_image.color.SetAlpha(value);

            yield return new WaitForFixedUpdate();
        }

        in_image.color = in_image.color.SetAlpha(in_end);
    }
    static public Coroutine FadeColour(this Image in_image, Color in_color, float in_duration)
    {
        return in_image.StartCoroutine(FadeColourRoutine(in_image, in_color, in_duration));
    }
    static public IEnumerator FadeColourRoutine(Image in_image, Color in_color, float in_duration)
    {
        Color start = in_image.color;

        float startedAt = Time.time;
        while (Time.time - startedAt < in_duration)
        {
            float percent = Mathf.Clamp01((Time.time - startedAt) / in_duration);
            Color value = start + (in_color - start) * percent;
            in_image.color = value;

            yield return new WaitForFixedUpdate();
        }
        in_image.color = in_color;
    }    
}