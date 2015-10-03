using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;
using Assets.Scripts.IAJ.Unity.Movement;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicClickArrive : DynamicArrive
    {
        List<KinematicData> flock;
        float radius, fanAngle;
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


        public override MovementOutput GetMovement()
        {
            if (PriorityManager.click.x != -1 && (Character.position - PriorityManager.click).magnitude >= CLICK_DISTANCE)
            {
                //Debug.Log("Has clicked");
                Target.position = PriorityManager.click;
                Debug.DrawRay(Character.position, Target.position - Character.position, Color.magenta);
            }
            return base.GetMovement();
        }
    }
}