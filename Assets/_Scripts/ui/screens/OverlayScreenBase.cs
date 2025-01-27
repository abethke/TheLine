using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayScreenBase : MonoBehaviour
{
    public void FadeIn()
    {        
        foreach (Image image in imagesToFade)
        {
            image.FadeIn(Constants.FADE_DURATION_IN_SECONDS);
        }
        foreach (TMP_Text text in textToFade)
        {
            text.FadeIn(Constants.FADE_DURATION_IN_SECONDS);
        }
    }
    public void FadeOut()
    {
        foreach (Image image in imagesToFade)
        {
            image.FadeOut(Constants.FADE_DURATION_IN_SECONDS);
        }
        foreach (TMP_Text text in textToFade)
        {
            text.FadeOut(Constants.FADE_DURATION_IN_SECONDS);
        }
    }
    [Header("References")]
    public Image[] imagesToFade = new Image[0];
    public TMP_Text[] textToFade = new TMP_Text[0];
}
