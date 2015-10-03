using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;
using Assets.Scripts.IAJ.Unity.Movement;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicVelocityMatch : DynamicMovement
    {
        public override string Name
        {
            get { return "VelocityMatch"; }
        }

        public override KinematicData Target { get; set; }

        public float TimeToTargetSpeed { get; set; }

        public KinematicData MovingTarget { get; set; }

        public DynamicVelocityMatch()
        {
            this.TimeToTargetSpeed = 0.5f;
        }
        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();
            output.linear = (this.MovingTarget.velocity - this.Character.velocity)/this.TimeToTargetSpeed;

            if (output.linear.sqrMagnitude > this.MaxAcceleration*this.MaxAcceleration)
            {
                output.linear = output.linear.normalized*this.MaxAcceleration;
            }
            output.angular = 0;
            //Debug.DrawRay(Character.position, output.linear, new Color(70 / 255, 15 / 255, 11 / 255));
            return output;
        }
    }
}
