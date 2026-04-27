using UnityEngine;

public class EngineIdleState : IEngineState
{
    public void Enter(EngineSound engineSound) 
    {
        Debug.Log("[EngineIdleState] Entering Idle State");
        engineSound.PlayIdle();
    }
    public void Exit() {  }
    public void UpdateState(EngineStateMachine stateMachine, MotorMovement motor, EngineSound engineSound)
    {
        float speed = motor.CurrentSpeed;
        if (speed > IEngineState.IdleThreshold)
        {
            stateMachine.SetState(new EngineAccelerationState());
        }
    }
}
