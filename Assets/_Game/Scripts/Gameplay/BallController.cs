using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    [Header("Arc")]
    [SerializeField] private float _arcHeightMin   = 3f;
    [SerializeField] private float _arcHeightMax   = 7f;
    [SerializeField] private float _flightDuration = 1.5f;

    [Header("Landing Scatter")]
    [SerializeField] private float _landingScatter = 0.6f;

    [Header("Indicators")]
    [SerializeField] private GameObject _nearIndicator;
    [SerializeField] private GameObject _farIndicator;

    [Header("Trail")]
    [SerializeField] private TrailRenderer _trail;

    public bool IsFlying { get; private set; }
    public bool IsScored { get; private set; }

    public void SetNearIndicator(bool show)
    {
        if (_nearIndicator != null) _nearIndicator.SetActive(show);
    }

    public void SetFarIndicator(bool show)
    {
        if (_farIndicator != null) _farIndicator.SetActive(show);
    }

    public void HideAllIndicators()
    {
        SetNearIndicator(false);
        SetFarIndicator(false);
    }

    public static event Action<BallController> OnAnyBallScored;

    public void KickToNearestGoal(Transform[] goals)
    {
        if (IsFlying || IsScored || goals.Length == 0) return;

        Transform nearest = null;
        float     minDist = float.MaxValue;
        foreach (var g in goals)
        {
            float d = Vector3.Distance(transform.position, g.position);
            if (d < minDist) { minDist = d; nearest = g; }
        }

        Vector2 scatter   = UnityEngine.Random.insideUnitCircle * _landingScatter;
        Vector3 landPos   = nearest.position + new Vector3(scatter.x, 0f, scatter.y);
        float   arcHeight = UnityEngine.Random.Range(_arcHeightMin, _arcHeightMax);

        StartCoroutine(FlyRoutine(transform.position, landPos, arcHeight));
    }

    private IEnumerator FlyRoutine(Vector3 start, Vector3 end, float arcHeight)
    {
        IsFlying = true;
        _rb.isKinematic = true;
        if (_trail != null) _trail.enabled = true;

        float elapsed = 0f;
        while (elapsed < _flightDuration)
        {
            float   t   = elapsed / _flightDuration;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y      += arcHeight * Mathf.Sin(t * Mathf.PI);
            transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        if (_trail != null) _trail.enabled = false;
        IsFlying = false;
        IsScored = true;
        OnAnyBallScored?.Invoke(this);
    }
}
