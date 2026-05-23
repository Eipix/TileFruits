using Levels;
using UnityEngine;

namespace Commons.EventTrigger
{
    [RequireComponent(typeof(SphereCollider))]
    public class SphereColliderEventTrigger : ColliderEventTrigger<SphereCollider>
    {
        public float Radius
        {
            get => Trigger.radius;
            set => Trigger.radius = value;
        }

        public Vector3 Center
        {
            get => Trigger.center;
            set => Trigger.center = value;
        }
    }
}
