using System;
using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _arcHeight    = 5f;
        [SerializeField] private float _flightDuration = 1.5f;

        public bool IsFlying { get; private set; }
        public bool IsScored { get; private set; }

        public static event Action<BallController> OnAnyBallScored;

        public void KickToNearestGoal(Transform[] goals)
        {
            if (IsFlying || IsScored || goals.Length == 0) return;

            Transform nearest  = null;
            float     minDist  = float.MaxValue;
            foreach (var g in goals)
            {
                float d = Vector3.Distance(transform.position, g.position);
                if (d < minDist) { minDist = d; nearest = g; }
            }

            StartCoroutine(FlyRoutine(transform.position, nearest.position));
        }

        private IEnumerator FlyRoutine(Vector3 start, Vector3 end)
        {
            IsFlying = true;
            _rb.isKinematic = true;

            float elapsed = 0f;
            while (elapsed < _flightDuration)
            {
                float t   = elapsed / _flightDuration;
                Vector3 pos = Vector3.Lerp(start, end, t);
                pos.y      += _arcHeight * Mathf.Sin(t * Mathf.PI);
                transform.position = pos;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            IsFlying = false;
            IsScored = true;
            OnAnyBallScored?.Invoke(this);
        }
}
