using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicSeek : DynamicMovement
    {
        public override string Name
        {
            get { return "Seek"; }
        }

        public override KinematicData Target { get; set; }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();
			/*if (this.Target == null)
				Debug.Log ("target null");
			if (this.Character == null)
				Debug.Log ("char null");*/
            output.linear = this.Target.position - this.Character.position;

            if (output.linear.sqrMagnitude > 0)
            {
                output.linear.Normalize();
                output.linear *= this.MaxAcceleration;
            }

            return output;
        }
    }
}
