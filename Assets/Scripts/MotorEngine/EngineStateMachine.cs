using UnityEngine;

public class EngineStateMachine
{
    private IEngineState _currentState;
    private readonly MotorMovement _motor;

    public EngineStateMachine(MotorMovement motor)
    {
        _motor = motor;
        SetState(new EngineIdleState());
    }

    public void SetState(IEngineState newState)
    {
        Debug.Log($"[EngineStateMachine] Switching from {_currentState?.GetType().Name ?? "None"} to {newState.GetType().Name}");
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void Update()
    {
        _currentState?.UpdateState(this, _motor);
    }
}