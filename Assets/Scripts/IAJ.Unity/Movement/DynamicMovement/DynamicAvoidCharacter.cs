using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement{
    public class DynamicAvoidCharacter : DynamicSeek {
        float collisionRadius;
        float maxTimeLookAhead;
		float avoidMargin;

        public float CollisionRadius{
            set { this.collisionRadius = value; }
            get { return collisionRadius; }
        }

        public float MaxTimeLookAhead{
            set { this.maxTimeLookAhead = value; }
            get { return maxTimeLookAhead; }
        }

		public float AvoidMargin{
			set{ this.avoidMargin = value;}
			get{ return avoidMargin; }
		}

		public override MovementOutput GetMovement()
        {
            /*if (targets == null)
                return new MovementOutput();*/
            float shortestTime = Mathf.Infinity;
            Vector3 deltaPos, deltaVel, closestDeltaPos = new Vector3(), closestDeltaVel = new Vector3();
			float closestDistance = 0f, deltaSpeed = 0f, timeToClosest = 0f, closestMinSeparation = 0f;
            KinematicData closestTarget = new KinematicData();

            deltaPos = Target.position - Character.position;
            deltaVel = Target.velocity - Character.velocity;
            deltaSpeed = deltaVel.magnitude;
            if (deltaSpeed == 0) return new MovementOutput(); ;
            timeToClosest = -(Vector3.Dot(deltaPos,deltaVel))/(deltaSpeed*deltaSpeed);
            if (timeToClosest > MaxTimeLookAhead) return new MovementOutput(); ;
            float distance = deltaPos.magnitude;
            float minSeparation = distance - deltaSpeed*timeToClosest;
            if (minSeparation > 2 * collisionRadius) return new MovementOutput(); ;
            if(timeToClosest > 0 && timeToClosest < shortestTime){
	            shortestTime = timeToClosest;
                closestTarget = Target;
	            closestMinSeparation = minSeparation;
	            closestDistance = distance;
	            closestDeltaPos = deltaPos;
	            closestDeltaVel = deltaVel;
			}

            if(shortestTime == Mathf.Infinity) return new MovementOutput();
            Vector3 avoidanceDirection;
            if(closestMinSeparation <= 0 || closestDistance < 2*collisionRadius)
            avoidanceDirection = Character.position - closestTarget.position;
            else
            avoidanceDirection = (closestDeltaPos + closestDeltaVel * shortestTime)*-1;
            MovementOutput output = new MovementOutput();
            output.linear = avoidanceDirection.normalized*MaxAcceleration;
            Debug.DrawRay(Character.position, avoidanceDirection.normalized * MaxAcceleration, Color.black);
            return output;
        }
    }
}