using Levels;
using UnityEngine;

namespace Commons
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderEventTrigger : ColliderEventTrigger<BoxCollider>
    {
        public Vector3 Size
        {
            get => Trigger.size;
            set => Trigger.size = value;
        }

        public Vector3 Center
        {
            get => Trigger.center;
            set => Trigger.center = value;
        }
    }
}
