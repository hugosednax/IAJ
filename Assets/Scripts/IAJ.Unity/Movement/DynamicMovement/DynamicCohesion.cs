using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicCohesion : DynamicArrive
    {
        List<KinematicData> flock;
        float radius, fanAngle;
        Vector3 click;
        const float CLICK_DISTANCE = 5f;

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

        public Vector3 Click
        {
            get { return click; }
            set { click = value; }
        }

        public override MovementOutput GetMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float angle = Vector3.Angle(ray.direction, Vector3.down) * Mathf.Deg2Rad;
                float distance = Camera.main.transform.position.y / Mathf.Cos(angle);
                Click = ray.origin + (ray.direction * distance);
                click.y = 0;
            }

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
            if (Click.x != -1 && Click.y != -1 && Click.z != -1 && (Character.position - click).magnitude <= CLICK_DISTANCE)
            {
                massCenter += Click;
                massCenter /= (closeBoids + 1);
            }else
                massCenter /= closeBoids;
            Target.position = massCenter;
            return base.GetMovement();
        }

        public override string Name
        {
            get { return "Dynamic Cohesion"; }
        }

    }
}
