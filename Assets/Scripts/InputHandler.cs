using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameInputActions;

public class InputHandler : MonoBehaviour, IGameplayActions
{
    GameInputActions gameInputActions;
    public event Action onFire;
    public event Action<bool> onWandTrackingStatusChanged;
    public bool IsWandTracked {get; private set; }
    void Awake()
    {
        gameInputActions = new GameInputActions();
        gameInputActions.Gameplay.Fire.performed += OnFire;
        // gameInputActions.Gameplay.Fire.canceled += OnFire;
        gameInputActions.Gameplay.WandTracked.performed += OnWandTracked;
        gameInputActions.Gameplay.WandTracked.canceled += OnWandTracked;
    }
    // Start is called before the first frame update
    void Start()
    {
        gameInputActions.Enable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        onFire?.Invoke();
    }

    public void OnWandTracked(InputAction.CallbackContext context)
    {
        IsWandTracked = context.ReadValue<int>() == 2 ? true : false;
        onWandTrackingStatusChanged?.Invoke(IsWandTracked);

        if (!IsWandTracked)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }
    }
}
