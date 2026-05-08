using UnityEngine;

public class PlayerController : MonoBehaviour
{
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 15f;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        // Blend: 0 = Idle, 0.25 = Walk, 0.6 = Run
        private static readonly int _blendParam = Animator.StringToHash("Blend");
        private const float BLEND_RUN  = 0.6f;
        private const float BLEND_IDLE = 0f;
        private const float BLEND_DAMP = 0.1f;

        [SerializeField] private Rigidbody _rb;

        private Vector3 _moveDir;

        void Awake()
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;

            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            _moveDir = new Vector3(h, 0f, v).normalized;

            HandleRotation();
            HandleAnimation();
        }

        void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + _moveDir * (_moveSpeed * Time.fixedDeltaTime));
        }

        private void HandleRotation()
        {
            if (Vector3.zero == _moveDir) return;

            Quaternion targetRot = Quaternion.LookRotation(_moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                _rotationSpeed * Time.deltaTime
            );
        }

        private void HandleAnimation()
        {
            float targetBlend = Vector3.zero != _moveDir ? BLEND_RUN : BLEND_IDLE;
            _animator.SetFloat(_blendParam, targetBlend, BLEND_DAMP, Time.deltaTime);
        }
}
