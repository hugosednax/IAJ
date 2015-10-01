using UnityEngine;
using System.Collections;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
	public class DynamicArrive : DynamicVelocityMatch {
		public override string Name
		{
			get { return "Arrive"; }
		}

		public DynamicArrive(){
			this.MovingTarget = new KinematicData ();
		}

	
		private float _slowRadius = 10f;
		
		public float SlowRadius{
			get { return _slowRadius; }
			set { _slowRadius = value; }
		}

		private float _stopRadius = 1f;
		
		public float StopRadius{
			get { return _stopRadius; }
			set { _stopRadius = value; }
		}
		
		public override KinematicData Target { get; set; }
		
		public override MovementOutput GetMovement()
		{
			Vector3 direction = Target.position - Character.position;
			float distance = direction.magnitude;

			if (distance < StopRadius)
				return new MovementOutput();

			float targetSpeed;
			if (distance > SlowRadius)
				targetSpeed = MaxAcceleration;
			else 
				targetSpeed = MaxAcceleration * (distance / SlowRadius);

			this.MovingTarget.velocity = direction.normalized * targetSpeed;

			return base.GetMovement();
		}
	}
}