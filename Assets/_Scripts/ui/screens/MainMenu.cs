public class MainMenu : OverlayScreenBase
{
    public void OnEnable()
    {
        FadeIn();
    }
    public void ClickedKeepGoing()
    {
        game.StartGame();
        gameObject.SetActive(false);
    }
    public void ClickedTryAgain()
    {
        game.ResetGame();
        gameObject.SetActive(false);
    }

    public IGameController game;
}
