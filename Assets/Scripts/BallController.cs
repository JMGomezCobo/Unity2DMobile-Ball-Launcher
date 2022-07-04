using UnityEngine;
using UnityEngine.InputSystem;

public class BallController : MonoBehaviour
{
    [Header("Component Bindings")] 
    [SerializeField] private Rigidbody2D _ballRigidbody2D;
    
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            _ballRigidbody2D.isKinematic = false;
            return;
        }
        
        _ballRigidbody2D.isKinematic = true;
        
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(touchPosition);

        _ballRigidbody2D.position = worldPosition;
    }
}