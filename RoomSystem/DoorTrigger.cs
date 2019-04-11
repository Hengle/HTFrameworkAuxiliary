using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 开/关门触发器
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DoorTrigger : MonoBehaviour
    {
        /// <summary>
        /// 是否触发
        /// </summary>
        public bool IsTrigger { get; set; } = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                IsTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                IsTrigger = false;
            }
        }
    }
}
