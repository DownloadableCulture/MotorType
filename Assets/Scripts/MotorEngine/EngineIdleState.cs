using UnityEngine;

public class EngineIdleState : IEngineState
{
    public void Enter() { }
    public void Exit() {  }
    public void UpdateState(EngineStateMachine stateMachine, MotorMovement motor)
    {
        float speed = motor.CurrentSpeed;
        if (speed > IEngineState.IdleThreshold)
        {
            stateMachine.SetState(new EngineAccelerationState());
        }
    }
}
