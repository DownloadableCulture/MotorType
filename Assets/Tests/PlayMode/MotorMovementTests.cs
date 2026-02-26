using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MotorMovementTests
{
    [UnityTest]
    public IEnumerator Rigidbody_HasZeroVelocity_OnStartup()
    {
        var go = new GameObject("Motor");
        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        go.AddComponent<MotorMovement>();

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(Vector3.zero, rb.linearVelocity,
            "Rigidbody should have zero velocity on startup");

        Object.Destroy(go);
    }
}