using NaughtyAttributes;
using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private InputAction _jump;
    private bool isControllable;

    private PlayerMovement _movement;
    private PlayerStatus _status;

    private readonly List<RaycastResult> _raycastResults = new();

    public Rigidbody2D Rb{ get; private set; }

    public Vector2 Velocity { 
        get => Rb.linearVelocity;
        set 
        {  
            Rb.linearVelocity = value; 
        }
    }

    private void Awake()
    {
        //playerinputの有効化
        var input = GetComponent<PlayerInput>();
        input.currentActionMap.Enable();

        _jump = input.currentActionMap.FindAction("Jump");
        isControllable = true;

        Rb = GetComponent<Rigidbody2D>();

        _movement = GetComponent<PlayerMovement>();
        if (_movement == null)
        {
            Debug.LogError("PlayerMovementがアタッチされていません。");
        }
        _status = GetComponent<PlayerStatus>();
        _status.IsDead
            .Pairwise()
            .Subscribe(pair =>
            {
                bool prev = pair.Previous;
                bool cur = pair.Current;

                //死亡した場合、操作不可能にする
                if (!prev && cur)
                {
                    isControllable = false;
                }
            });
        
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerUp -= OnFingerUp;

        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (!isControllable)
            return;

        HandleNonTouchInput();
    }

    private void OnFingerDown(Finger finger)
    {
        if (!isControllable)
            return;

        //スクリーンが別のUI上の時は入力無視
        Vector2 screenPos = finger.screenPosition;
        if (IsPointerOverUI(screenPos))
        {
            return;
        }

        _movement.StartJumpRequest();
    }
    private void OnFingerUp(Finger finger)
    {
        if (!isControllable)
            return;

        //スクリーンが別のUI上の時は入力無視
        Vector2 screenPos = finger.screenPosition;
        if (IsPointerOverUI(screenPos))
        {
            return;
        }

        _movement.JumpReleaseRequest();
    }

    private void HandleNonTouchInput()
    {
        //タッチスクリーンがある場合は別で処理する
        if (Touchscreen.current != null)
        {
            return;
        }

        if (_jump.WasPressedThisFrame())
        {
            //if (ShouldIgnoreJumpInput())
            //{
            //    return;
            //}

            _movement.StartJumpRequest();
        }
        else if (_jump.WasReleasedThisFrame())
        {
            _movement.JumpReleaseRequest();
        }
    }

    private bool ShouldIgnoreJumpInput()
    {
        //タッチスクリーンじゃない時は終了
        if(Touchscreen.current == null)
        {
            return false;
        }

        if (!Touchscreen.current.press.isPressed)
        {
            return false;
        }

        Vector2 screenPos = Touchscreen.current.position.ReadValue();
        return IsPointerOverUI(screenPos);
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
        {
            Debug.Log("EventSystemがありません。");
            return false;
        }

        var pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        _raycastResults.Clear();

        EventSystem.current.RaycastAll(
            pointerEventData,
            _raycastResults
        );

        return _raycastResults.Count > 0;
    }
}
