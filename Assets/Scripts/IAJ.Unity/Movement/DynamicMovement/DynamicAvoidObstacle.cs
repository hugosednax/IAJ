using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;
namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        private float avoidMargin;
        private float maxLookAhead;
        private GameObject obstacle;

        public DynamicAvoidObstacle(GameObject obstacle)
        {
            this.Target = new KinematicData();
            this.obstacle = obstacle;
        }
        public override string Name
        {
            get { return "AvoidObstacle"; }
        }
        public float AvoidMargin
        {
            set { avoidMargin = value; }
            get { return avoidMargin; }
        }
        public float MaxLookAhead
        {
            set { maxLookAhead = value; }
            get { return maxLookAhead; }
        }

        public override MovementOutput GetMovement()
        {
            Vector3 forward = Character.velocity.normalized;
            RaycastHit hit;
            RaycastHit hitR;
            RaycastHit hitL;
            RaycastHit realHit;

            if (PriorityManager.debugMode)
            {
                Debug.DrawRay(Character.position, forward * MaxLookAhead, Color.red);
                Debug.DrawRay(Character.position, Quaternion.AngleAxis(45, Vector3.up) * forward * MaxLookAhead / 2, Color.green);
                Debug.DrawRay(Character.position, Quaternion.AngleAxis(-45, Vector3.up) * forward * MaxLookAhead / 2, Color.green);
            }

            Ray rwhisker = new Ray(Character.position, (Quaternion.AngleAxis(-45, Vector3.up) * forward).normalized);
            Ray lwhisker = new Ray(Character.position, (Quaternion.AngleAxis(45, Vector3.up) * forward).normalized);
            Ray central = new Ray(Character.position, forward);

            if (!forward.Equals(new Vector3(0,0,0)))
            {
                bool RwhiskerCollided = obstacle.GetComponent<Collider>().Raycast(rwhisker, out hitR, MaxLookAhead / 2);
                bool LwhiskerCollided = obstacle.GetComponent<Collider>().Raycast(lwhisker, out hitL, MaxLookAhead / 2);

                if (obstacle.GetComponent<Collider>().Raycast(central, out hit, MaxLookAhead) || RwhiskerCollided || LwhiskerCollided)
                {
                    if (hit.collider != null)
                        realHit = hit;
                    else if (hitR.collider != null)
                        realHit = hitR;
                    else
                        realHit = hitL;

                    //Debug.Log(realHit.transform.name);
                    Target.position = realHit.point + realHit.normal.normalized * AvoidMargin;
                    if(PriorityManager.debugMode)
                        Debug.DrawRay(realHit.point, Target.position - realHit.point, Color.blue);
                }
                else return new MovementOutput();
            }
            return base.GetMovement();
        }

    }
}
