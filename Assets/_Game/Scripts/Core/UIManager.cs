using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _hudPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameOverPanel;

    private readonly Dictionary<string, GameObject> _panels = new();

    protected override void Awake()
    {
        base.Awake();
        RegisterPanels();
    }

    void OnEnable() => GameManager.OnGameStateChanged += HandleStateChanged;
    void OnDisable() => GameManager.OnGameStateChanged -= HandleStateChanged;

    private void RegisterPanels()
    {
        if (_mainMenuPanel) _panels["MainMenu"] = _mainMenuPanel;
        if (_hudPanel) _panels["HUD"] = _hudPanel;
        if (_pausePanel) _panels["Pause"] = _pausePanel;
        if (_gameOverPanel) _panels["GameOver"] = _gameOverPanel;
    }

    private void HandleStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu: ShowOnly("MainMenu"); break;
            case GameState.Playing: ShowOnly("HUD"); break;
            case GameState.Paused: Show("Pause"); break;
            case GameState.GameOver: ShowOnly("GameOver"); break;
        }
    }

    public void Show(string panelName)
    {
        if (_panels.TryGetValue(panelName, out var panel))
            panel.SetActive(true);
    }

    public void Hide(string panelName)
    {
        if (_panels.TryGetValue(panelName, out var panel))
            panel.SetActive(false);
    }

    public void ShowOnly(string panelName)
    {
        foreach (var kvp in _panels)
            kvp.Value.SetActive(kvp.Key == panelName);
    }

    public void HideAll()
    {
        foreach (var kvp in _panels)
            kvp.Value.SetActive(false);
    }
}

