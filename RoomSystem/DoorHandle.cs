using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 门把手
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DoorHandle : MonoBehaviour
    {
        /// <summary>
        /// 附加到的门
        /// </summary>
        public Door AttachDoor;
        /// <summary>
        /// 是否前开门（否者后开门）
        /// </summary>
        public bool IsFrontOpen = true;

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (AttachDoor.IsOpened)
            {
                AttachDoor.Close();
            }
            else
            {
                if (IsFrontOpen)
                {
                    AttachDoor.FrontOpen();
                }
                else
                {
                    AttachDoor.BackOpen();
                }
            }
        }
    }
}
