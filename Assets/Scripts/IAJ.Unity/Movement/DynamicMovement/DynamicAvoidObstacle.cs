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
            

            if (PriorityManager.debugMode)
            {
                Debug.DrawRay(Character.position, forward * MaxLookAhead, Color.red);
                Debug.DrawRay(Character.position, Quaternion.AngleAxis(45, Vector3.up) * forward * MaxLookAhead / 2, Color.green);
                Debug.DrawRay(Character.position, Quaternion.AngleAxis(-45, Vector3.up) * forward * MaxLookAhead / 2, Color.green);
            }

            if (!forward.Equals(new Vector3(0, 0, 0)))
            {
                /*bool RwhiskerCollided = ;
                bool LwhiskerCollided =;*/
                RaycastHit hit;
                Ray ray = new Ray(Character.position, forward);
                float whiskersRange = MaxLookAhead / 2;
                Collider coll = obstacle.GetComponent<Collider>();
                if (coll.Raycast(new Ray(Character.position, forward), out hit, MaxLookAhead))
                {

                }
                else{
                    ray.direction = (Quaternion.AngleAxis(-45, Vector3.up) * forward).normalized;
                    if (coll.Raycast(ray, out hit, whiskersRange))
                    {
                    }else{
                        ray.direction = (Quaternion.AngleAxis(45, Vector3.up) * forward).normalized;
                        if (coll.Raycast(ray, out hit, whiskersRange))
                        {
                        }else return new MovementOutput();
                    }
                }
                Target.position = hit.point + hit.normal.normalized * AvoidMargin;
                if (PriorityManager.debugMode)
                    Debug.DrawRay(hit.point, Target.position - hit.point, Color.blue);
            }
            return base.GetMovement();
        }
    }
}
