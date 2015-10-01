using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicFlockVelocityMatching : DynamicVelocityMatch
    {
        List<KinematicData> flock;
        float separationFactor, radius, separationStrength, fanAngle;

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

        float FanAngle
        {
            get { return fanAngle; }
            set { fanAngle = value; }
        }

        public override MovementOutput GetMovement()
        {
            Vector3 averageVelocity = new Vector3();
            int closeBoids = 0;
            foreach(KinematicData boid in flock){
                if(Character != boid){
                    Vector3 direction = boid.position - Character.position;
                    if(direction.magnitude <= radius){
                        float angle = MathHelper.ConvertVectorToOrientation(direction);
                        float angleDifference = Mathf.DeltaAngle(Character.orientation,angle);
                        if(Mathf.Abs(angleDifference) <= fanAngle){
                            averageVelocity += boid.velocity;
                            closeBoids ++;
                        }
                    }
                }
            }
            if(closeBoids == 0) return new MovementOutput();
            averageVelocity /= closeBoids;
            Target.velocity = averageVelocity;
            return base.GetMovement();
        }

        public override string Name
        {
            get { return "Dynamic Flock Velocity Matching"; }
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
