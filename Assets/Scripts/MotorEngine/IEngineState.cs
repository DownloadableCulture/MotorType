using UnityEngine;

public interface IEngineState
{

    const float IdleThreshold = 0.1f;
    const float FullSpeedThreshold = 20f;
    void Enter();
    void Exit();
    void UpdateState(EngineStateMachine stateMachine, MotorMovement motor);
}
