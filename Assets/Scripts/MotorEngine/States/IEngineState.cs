using UnityEngine;

public interface IEngineState
{

    const float IdleThreshold = 4f;
    const float FullSpeedThreshold = 20f;
    void Enter(EngineSound engineSound);
    void Exit();
    void UpdateState(EngineStateMachine stateMachine, MotorMovement motor, EngineSound engineSound);
}
