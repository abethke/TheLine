using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayScreenBase : MonoBehaviour
{
    public void FadeIn()
    {        
        foreach (Image image in imagesToFade)
        {
            image.FadeIn(GameConfiguration.instance.fadeDurationInSeconds);
        }
        foreach (TMP_Text text in textToFade)
        {
            text.FadeIn(GameConfiguration.instance.fadeDurationInSeconds);
        }
    }
    public void FadeOut()
    {
        foreach (Image image in imagesToFade)
        {
            image.FadeOut(GameConfiguration.instance.fadeDurationInSeconds);
        }
        foreach (TMP_Text text in textToFade)
        {
            text.FadeOut(GameConfiguration.instance.fadeDurationInSeconds);
        }
    }
    [Header("References")]
    public Image[] imagesToFade = new Image[0];
    public TMP_Text[] textToFade = new TMP_Text[0];
}
