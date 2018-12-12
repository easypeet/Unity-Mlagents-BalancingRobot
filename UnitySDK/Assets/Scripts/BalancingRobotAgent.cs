using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class BalancingRobotAgent : Agent
{

    public Transform floorTransform;
    public Transform rotObject;
    public Transform Target;

    private Rigidbody rBody;
    private Rigidbody rBodyRotationObject;
    //private Transform initialRobotTransform;
    //private Transform initialRotationObjectTransform;

    private float initialTargetDistance = 80.0f;
    private float maxSpeed = 3.5f;
    private float maxAngle = 90.0f;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        rBodyRotationObject = rotObject.GetComponent<Rigidbody>();
        //initialRobotTransform = this.transform;
        //initialRotationObjectTransform = rotObject.transform;
    }
       

    public override void AgentReset()
    {
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;

        this.transform.localPosition = new Vector3(0, 0.5f, 0); ;
        this.transform.rotation = new Quaternion(0.7f, 0, 0, 0.7f);

        rBodyRotationObject.angularVelocity = Vector3.zero;
        rBodyRotationObject.velocity = Vector3.zero;

        rotObject.localPosition = new Vector3(0, 2.0f, 0);
        rotObject.rotation = new Quaternion(0, 0, 0, 1.0f);
        
    }

    public override void CollectObservations()
    {
        // Calculate relative position
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);

        // Relative position x1
        AddVectorObs(distanceToTarget / initialTargetDistance);
        
 
        // Agent velocity x2
        AddVectorObs(rBody.velocity.x / maxSpeed);

        // Rotation object rotation x3
        if (rotObject.transform.rotation.eulerAngles.z < maxAngle)
        {
            AddVectorObs(rotObject.transform.rotation.eulerAngles.z / maxAngle);
        }
        else
        {
            AddVectorObs((rotObject.transform.rotation.eulerAngles.z - 360.0f) / maxAngle);
        }

    }


    public float speed = 1;
    // private float previousDistance = float.MaxValue;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

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
        if (rBody.velocity.x < 0)
        {
            AddReward(0.05f);
        }

        if(this.transform.position.y < 0)
        {
            Done();
        }
        // Time penalty
        AddReward(-0.045f);

        // Robot tipped over
        if (rotObject.transform.rotation.eulerAngles.z > 90.0f & rotObject.transform.rotation.eulerAngles.z < 100.0f | rotObject.transform.rotation.eulerAngles.z < 270.0f & rotObject.transform.rotation.eulerAngles.z >250.0f)
        {
            AddReward(-1.0f);
            Done();
        }

        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        // controlSignal.z = vectorAction[1];
        // rBody.AddForce(controlSignal * speed);
        rBody.AddTorque(new Vector3(0, 0, 1f)* vectorAction[0] * speed);
    }
}
