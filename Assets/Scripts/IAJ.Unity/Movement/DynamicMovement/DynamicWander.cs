using UnityEngine; 
using Assets.Scripts.IAJ.Unity.Util;
namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander : DynamicSeek
    {
		public float wanderRate = 0.7f;
        public DynamicWander()
        {
            this.TurnAngle = 0.5f;
            Target = new KinematicData();
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; private set; }

        public float WanderOffset { get; private set; }
        public float WanderRadius { get; private set; }

        protected float WanderOrientation { get; set; }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();
			WanderOrientation += (Random.value - Random.value) * wanderRate;
			WanderOffset = 10f;
			WanderRadius = 8f;
            float targetOrientation = WanderOrientation + Character.orientation;
			Vector3 circleCenter = Character.position + WanderOffset * MathHelper.ConvertOrientationToVector(Character.orientation);
            Vector3 offsetCenter = circleCenter + WanderRadius * MathHelper.ConvertOrientationToVector(targetOrientation);
            //this.Target = new KinematicData(offsetCenter, Target.velocity, targetOrientation, TurnAngle);
            output.linear = (offsetCenter - Character.position).normalized * MaxAcceleration;
            return output;
        }
    }
}
