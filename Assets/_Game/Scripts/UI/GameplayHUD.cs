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
        _kickButton.gameObject.SetActive(GetNearestKickableBall() != null);
    }

    // ── Kick 

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
        ball.KickToNearestGoal(_goals);
        CameraController.Instance.FollowTarget(ball.transform);
    }

    // ── Scored 

    private void HandleBallScored(BallController ball)
    {
        if (_confettiPrefab != null)
            Instantiate(_confettiPrefab, ball.transform.position, Quaternion.identity);

        CameraController.Instance.ReturnToPlayerAfterDelay(_cameraReturnDelay);
    }

    // ── Reset 

    private void OnResetClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Helpers 

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
