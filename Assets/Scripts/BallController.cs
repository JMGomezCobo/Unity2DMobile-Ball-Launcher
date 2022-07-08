using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace BallLauncher
{
    public class BallController : MonoBehaviour
    {
        [Header("Prefab Bindings")]
        [SerializeField] private GameObject _ballPrefab;
        
        [Header("Component Bindings")]
        [SerializeField] private Rigidbody2D _pivot;

        [Header("Settings")] 
        [SerializeField] private float _respawnDelay = 2f;
        [SerializeField] private float _detachDelay  = 1f;

        private bool isDragging;

        private Camera _mainCamera;
        private Rigidbody2D   _currentBallRigidbody;
        private SpringJoint2D _currentBallSprintJoint;

        #region • Unity methods (4)

        private void Start()
        {
            _mainCamera = Camera.main;

            SpawnBall();
        }

        private void Update()
        {
            if (_currentBallRigidbody == null) { return; }

            if (Touch.activeTouches.Count == 0)
            {
                if (isDragging) LaunchBall();

                isDragging = false;

                return;
            }

            isDragging = true;
            _currentBallRigidbody.isKinematic = true;

            var worldPosition = HandleInput();

            _currentBallRigidbody.position = worldPosition;
        }

        private void OnEnable() => EnhancedTouchSupport.Enable();

        private void OnDisable() => EnhancedTouchSupport.Disable();

        #endregion

        #region • Custom methods (4)

        private Vector3 HandleInput()
        {
            Vector2 touchPosition = new Vector2();

            touchPosition = Touch.activeTouches.Aggregate(touchPosition, (current, touch) => current + touch.screenPosition);
            touchPosition /= Touch.activeTouches.Count;

            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
            
            return worldPosition;
        }

        private void SpawnBall()
        {
            GameObject ballInstance = Instantiate(_ballPrefab, _pivot.position, Quaternion.identity);

            _currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
            _currentBallSprintJoint = ballInstance.GetComponent<SpringJoint2D>();

            _currentBallSprintJoint.connectedBody = _pivot;
        }

        private void LaunchBall()
        {
            _currentBallRigidbody.isKinematic = false;
            _currentBallRigidbody = null;

            Invoke(nameof(DetachBall), _detachDelay);
        }

        private void DetachBall()
        {
            _currentBallSprintJoint.enabled = false;
            _currentBallSprintJoint = null;

            Invoke(nameof(SpawnBall), _respawnDelay);
        }

        #endregion
    }
}