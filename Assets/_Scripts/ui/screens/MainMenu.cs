public class MainMenu : OverlayScreenBase
{
    protected void Start()
    {
        _game = ServiceManager.instance.Get(Services.GAME) as IGameController;
    }
    public void OnEnable()
    {
        FadeIn();
    }
    public void ClickedKeepGoing()
    {
        if (_game.state == GameStates.Paused)
        {
            _game.StartGame();
        }
        gameObject.SetActive(false);
    }
    public void ClickedTryAgain()
    {
        _game.ResetGame();
        gameObject.SetActive(false);
    }

    protected IGameController _game;
}
