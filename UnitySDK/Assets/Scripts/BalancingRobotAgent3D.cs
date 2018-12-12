using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class BalancingRobotAgent3D : Agent
{
    
    public Transform target;
    public WheelCollider rightWheel;
    public WheelCollider leftWheel;

    //private Rigidbody rightWheelRigidBody;
    // private Rigidbody leftWheelRigidBody;

    // private Rigidbody rotationObjectRigitBody;

    private Rigidbody rigidbody;
    private SimpleCarController carcontroller;
    private float initialTargetDistance = 150.0f;
    private float maxSpeed = 1200.0f;
    private float maxAngle = 40.0f;
    private bool respawn = true; 

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        carcontroller = GetComponent<SimpleCarController>();
    }
       

    public override void AgentReset()
    {
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.15f, 0);
        this.transform.rotation = new Quaternion(0, 0.7f, 0, 0.7f);

        leftWheel.brakeTorque = Mathf.Infinity;
        rightWheel.brakeTorque = Mathf.Infinity;

        initialTargetDistance = Random.Range(80, 180);
        target.transform.localPosition = new Vector3(-initialTargetDistance, -0.22f, 0);

        // TODO reset wheel torques


        /*
        rotationObjectRigitBody.angularVelocity = Vector3.zero;
        rotationObjectRigitBody.velocity = Vector3.zero;

        rotObject.localPosition = new Vector3(0, 2.0f, 0);
        rotObject.rotation = new Quaternion(0, 0, 0, 1.0f);
        */
    }

    public override void CollectObservations()
    {
        // Calculate relative position
        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);

        // Relative position x1
        AddVectorObs(distanceToTarget / initialTargetDistance);

        // Agent velocity x2
        AddVectorObs(leftWheel.rpm / maxSpeed);
        // MonoBehaviour.print(leftWheel.rpm / maxSpeed);
        //AddVectorObs(rigidbody.velocity.z / maxSpeed);

        // Rotation object rotation x3
        if (this.transform.rotation.eulerAngles.x < 180.0f)
        {
            // MonoBehaviour.print(this.transform.rotation.eulerAngles.x);
            AddVectorObs(this.transform.rotation.eulerAngles.x / maxAngle);
        }
        else
        {
            // MonoBehaviour.print(this.transform.rotation.eulerAngles.x - 360.0f);
            AddVectorObs((this.transform.rotation.eulerAngles.x - 360.0f) / maxAngle);
        }

    }


    public float speed = 1;
    // private float previousDistance = float.MaxValue;

    public override void AgentAction(float[] vectorAction, string textAction)
    {


        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  target.position);

        // Reached target
        if (this.transform.position.x < -initialTargetDistance)
        {
            AddReward(1.0f);
            Done();
        }

        // Reward for getting closer
        if (distanceToTarget < initialTargetDistance)
        {
            // AddReward((initialTargetDistance - distanceToTarget) / 100.0f);
        }
        if (rigidbody.velocity.x < 0)
        {
            AddReward(0.05f);
        }

        
        if(this.transform.position.y < 0)
        {
            AddReward(-1.0f);
            Done();
        }

        // Time penalty
        AddReward(-0.045f);

        // Robot tipped over
        if (this.transform.rotation.eulerAngles.x > maxAngle & this.transform.rotation.eulerAngles.x < 120.0f | this.transform.rotation.eulerAngles.x < (360.0f - maxAngle) & this.transform.rotation.eulerAngles.x > 250.0f)
        {
            AddReward(-1.0f);
            Done();
        }


        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];

        carcontroller.ApplyTorque(vectorAction[0]);
        // controlSignal.z = vectorAction[1];
        // rBody.AddForce(controlSignal * speed);
       // rBody.AddTorque(new Vector3(0, 0, 1f)* vectorAction[0] * speed);
    }
}
