using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 开/关门触发器
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DoorTrigger : RoomBehaviour
    {
        /// <summary>
        /// 是否触发
        /// </summary>
        public bool IsTrigger { get; private set; } = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IsTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IsTrigger = false;
            }
        }
    }
}
