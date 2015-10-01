using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicSeparation : DynamicMovement
    {
        List<KinematicData> flock;
        float separationFactor, radius, separationStrength;

        List<KinematicData> Flock
        {
            get { return flock; }
            set { flock = value; }
        }

        float SeparationFactor
        {
            get { return separationFactor; }
            set { separationFactor = value; }
        }

        float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public override MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();
            Vector3 direction = new Vector3();

            foreach (KinematicData boid in flock)
            {
                if(boid != Character)
                    direction = Character.position - boid.position;
                if(direction.magnitude < radius){
                    separationStrength = Mathf.Min(separationFactor / (direction.magnitude * direction.magnitude), MaxAcceleration);
                    direction.Normalize();
                    output.linear += direction * separationStrength;
                }
            }
            if(output.linear.magnitude > MaxAcceleration){
                output.linear.Normalize();
                output.linear*=MaxAcceleration;
            }
            return output;
        }

        public override string Name
        {
            get { return "Dynamic Separation"; }
        }

        public override KinematicData Target
        {
            get
            {
                return Target;
            }
            set
            {
                Target = value;
            }
        }
    }
}
