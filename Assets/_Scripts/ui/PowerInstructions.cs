using TMPro;
using UnityEngine;

public class PowerInstructions : MonoBehaviour
{
    void Start()
    {
        _game = ServiceManager.instance.Get(Services.GAME) as IGameController;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        _scaleFactorForResolution = canvasRect.rect.width / Screen.width;
    }
    void Update()
    {
        if (_game.state != GameStates.ActiveGame)
            return;

        Vector3 worldToScreenPosition = Camera.main.WorldToScreenPoint(_player.transform.position) * _scaleFactorForResolution;
        float newX = worldToScreenPosition.x - Screen.width * _scaleFactorForResolution * 0.5f;
        _display.rectTransform.anchoredPosition = _display.rectTransform.anchoredPosition.SetX(newX);
    }
    public void SetText(string in_text)
    {
        _display.text = in_text;
    }
    public void SetTextColor(Color in_color)
    {
        _display.color = in_color;
    }

    protected float _scaleFactorForResolution;

    protected IGameController _game;

    [Header("References")]
    [SerializeField]
    protected Canvas _canvas;
    [SerializeField]
    protected Player _player;
    [SerializeField]
    protected TMP_Text _display;

}
