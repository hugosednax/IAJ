using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;
using Assets.Scripts.IAJ.Unity.Movement;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicCohesion : DynamicArrive
    {
        List<KinematicData> flock;
        float radius, fanAngle;
        //const float CLICK_DISTANCE = 5f;

        public List<KinematicData> Flock
        {
            get { return flock; }
            set { flock = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float FanAngle
        {
            get { return fanAngle; }
            set { fanAngle = value; }
        }

        public override MovementOutput GetMovement()
        {

            Vector3 massCenter = new Vector3();
            int closeBoids = 0;
            foreach(KinematicData boid in flock){
                if(Character != boid){
                    Vector3 direction = boid.position - Character.position;
                    if(direction.magnitude <= radius){
                        float angle = Util.MathHelper.ConvertVectorToOrientation(direction);
                        float angleDifference = Mathf.Deg2Rad*Mathf.DeltaAngle(Mathf.Rad2Deg*Character.orientation, Mathf.Rad2Deg*angle);
                        if(Mathf.Abs(angleDifference) <= fanAngle){
                            massCenter += boid.position;
                            closeBoids ++;
                        }
                    }
                }
            }
            if(closeBoids == 0) return new MovementOutput();
            /*
            if (PriorityManager.click.x != -1 && (Character.position - PriorityManager.click).magnitude >= CLICK_DISTANCE)
            {
                //Debug.Log("Has clicked");
                massCenter += PriorityManager.click * 10;
                massCenter /= (closeBoids);
            }else*/
            massCenter /= closeBoids;
            Debug.DrawRay(Character.position, massCenter - Character.position, Color.cyan);
            Target.position = massCenter;
            return base.GetMovement();
        }

        public override string Name
        {
            get { return "Dynamic Cohesion"; }
        }

    }
}
