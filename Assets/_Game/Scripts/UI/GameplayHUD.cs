using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayHUD : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _kickButton;
    [SerializeField] private Button _autoKickButton;
    [SerializeField] private Button _resetButton;

    [Header("References")]
    [SerializeField] private PlayerController _player;
    [SerializeField] private Transform[] _goals;
    [SerializeField] private BallController[] _balls;
    [SerializeField] private GameObject _confettiPrefab;

    [Header("Settings")]
    [SerializeField] private float _kickDetectionRadius = 2f;
    [SerializeField] private float _cameraReturnDelay = 2f;

    [Header("Reset Pulse")]
    [SerializeField] private float _pulseScale = 1.15f;
    [SerializeField] private float _pulseSpeed = 2f;

    private BallController _nearBall;
    private BallController _farBall;
    private bool      _allBallsDone;
    private Coroutine _kickPulseCoroutine;

    void Awake()
    {
        if (_balls == null || _balls.Length == 0)
            _balls = FindObjectsOfType<BallController>();

        _kickButton.gameObject.SetActive(false);
        _kickButton.onClick.AddListener(OnKickClicked);
        _autoKickButton.onClick.AddListener(OnAutoKickClicked);
        _resetButton.onClick.AddListener(OnResetClicked);
    }

    void OnEnable() => BallController.OnAnyBallScored += HandleBallScored;
    void OnDisable() => BallController.OnAnyBallScored -= HandleBallScored;

    void Update()
    {
        if (_allBallsDone) return;

        BallController nearest = GetNearestKickableBall();
        BallController farthest = GetFarthestKickableBall();

        bool kickVisible = nearest != null;
        _kickButton.gameObject.SetActive(kickVisible);
        _autoKickButton.gameObject.SetActive(farthest != null);

        if (kickVisible && _kickPulseCoroutine == null)
            _kickPulseCoroutine = StartCoroutine(PulseButton(_kickButton.transform));
        else if (!kickVisible && _kickPulseCoroutine != null)
        {
            StopCoroutine(_kickPulseCoroutine);
            _kickPulseCoroutine = null;
            _kickButton.transform.localScale = Vector3.one;
        }

        if (nearest != _nearBall)
        {
            if (_nearBall != null) _nearBall.SetNearIndicator(false);
            _nearBall = nearest;
            if (_nearBall != null) _nearBall.SetNearIndicator(true);
        }

        if (farthest != _farBall)
        {
            if (_farBall != null) _farBall.SetFarIndicator(false);
            _farBall = farthest;
            if (_farBall != null) _farBall.SetFarIndicator(true);
        }
    }

    // ── Kick ──────────────────────────────────────────────────────────────

    private void OnKickClicked()
    {
        BallController ball = GetNearestKickableBall();
        if (ball == null) return;
        KickBall(ball);
    }

    private void OnAutoKickClicked()
    {
        BallController ball = GetFarthestKickableBall();
        if (ball == null) return;
        KickBall(ball);
    }

    private void KickBall(BallController ball)
    {
        ball.HideAllIndicators();
        if (_nearBall == ball) _nearBall = null;
        if (_farBall == ball) _farBall = null;
        AudioManager.Instance.PlayShootSFX();
        ball.KickToNearestGoal(_goals);
        CameraController.Instance.FollowTarget(ball.transform);
    }

    // ── Scored ────────────────────────────────────────────────────────────

    private void HandleBallScored(BallController ball)
    {
        if (_confettiPrefab != null)
            Instantiate(_confettiPrefab, ball.transform.position, Quaternion.identity);

        AudioManager.Instance.PlayConfettiSFX();
        CameraController.Instance.ReturnToPlayerAfterDelay(_cameraReturnDelay);

        if (HasAllBallsScored())
            StartCoroutine(OnAllBallsDoneRoutine());
    }

    private IEnumerator OnAllBallsDoneRoutine()
    {
        yield return new WaitForSeconds(_cameraReturnDelay + 0.5f);

        _allBallsDone = true;
        _kickButton.gameObject.SetActive(false);
        _autoKickButton.gameObject.SetActive(false);

        if (_confettiPrefab != null)
        {
            foreach (var ball in _balls)
            {
                Instantiate(_confettiPrefab, ball.transform.position, Quaternion.identity);
            }
            AudioManager.Instance.PlayWinSFX();
            AudioManager.Instance.PlayConfettiSFX();
        }
        yield return new WaitForSeconds(_cameraReturnDelay + 0.5f);
        StartCoroutine(PulseButton(_resetButton.transform));
    }

    private IEnumerator PulseButton(Transform t)
    {
        while (true)
        {
            float s = 1f + (_pulseScale - 1f) * Mathf.Abs(Mathf.Sin(Time.unscaledTime * _pulseSpeed));
            t.localScale = Vector3.one * s;
            yield return null;
        }
    }

    // ── Reset ─────────────────────────────────────────────────────────────

    private void OnResetClicked()
    {
        StopAllCoroutines();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private bool HasAllBallsScored()
    {
        foreach (var ball in _balls)
            if (!ball.IsScored) return false;
        return true;
    }

    private BallController GetNearestKickableBall()
    {
        BallController nearest = null;
        float minDist = _kickDetectionRadius;
        foreach (var ball in _balls)
        {
            if (ball.IsFlying || ball.IsScored) continue;
            float d = Vector3.Distance(_player.transform.position, ball.transform.position);
            if (d < minDist) { minDist = d; nearest = ball; }
        }
        return nearest;
    }

    private BallController GetFarthestKickableBall()
    {
        BallController farthest = null;
        float maxDist = -1f;
        foreach (var ball in _balls)
        {
            if (ball.IsFlying || ball.IsScored) continue;
            float d = Vector3.Distance(_player.transform.position, ball.transform.position);
            if (d > maxDist) { maxDist = d; farthest = ball; }
        }
        return farthest;
    }
}
