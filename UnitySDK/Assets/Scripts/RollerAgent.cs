﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;


public class RollerAgent : Agent
{

    public Transform floorTransform;

    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void AgentReset()
    {
        if (this.transform.position.y < -1.0)
        {
            // The Agent fell
            this.transform.position = floorTransform.position;
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
        }
        else
        {
            // Move the target to a new spot
            Target.position = new Vector3(floorTransform.position.x + Random.value * 8 - 4,
                                          0.5f,
                                          floorTransform.position.z + Random.value * 8 - 4);
        }
    }

    public override void CollectObservations()
    {
        // Calculate relative position
        Vector3 relativePosition = Target.position - this.transform.position;

        // Relative position
        AddVectorObs(relativePosition.x / 5);
        AddVectorObs(relativePosition.z / 5);

        // Distance to edges of platform
        AddVectorObs((this.transform.localPosition.x + 5) / 5);
        AddVectorObs((this.transform.localPosition.x - 5) / 5);
        AddVectorObs((this.transform.localPosition.z + 5) / 5);
        AddVectorObs((this.transform.localPosition.z - 5) / 5);

        // Agent velocity
        AddVectorObs(rBody.velocity.x / 5);
        AddVectorObs(rBody.velocity.z / 5);

    }


    public float speed = 1;
    //private float previousDistance = float.MaxValue;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        // Time penalty
        AddReward(-0.05f);

        // Fell off platform
        if (this.transform.position.y < -1.0)
        {
            AddReward(-1.0f);
            Done();
        }

        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);
    }
}
