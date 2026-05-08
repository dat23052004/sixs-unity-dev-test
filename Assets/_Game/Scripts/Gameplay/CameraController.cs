using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Transform _playerTarget;

        public static CameraController Instance { get; private set; }

        void Awake() => Instance = this;

        public void FollowPlayer()
        {
            CancelInvoke(nameof(FollowPlayer));
            _virtualCamera.Follow = _playerTarget;
        }

        public void FollowTarget(Transform target)
        {
            CancelInvoke(nameof(FollowPlayer));
            _virtualCamera.Follow = target;
        }

        public void ReturnToPlayerAfterDelay(float delay)
        {
            CancelInvoke(nameof(FollowPlayer));
            Invoke(nameof(FollowPlayer), delay);
        }
    }

