using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 门把手
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class DoorHandle : RoomBehaviour
    {
        /// <summary>
        /// 附加到的门
        /// </summary>
        public Door AttachDoor;
        /// <summary>
        /// 开门方向
        /// </summary>
        public DoorOpenDirection OpenDirection;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnMouseDown()
        {
            if (GlobalTools.IsPointerOverUGUI())
            {
                return;
            }

            if (AttachDoor.IsOpened)
            {
                AttachDoor.Close();
            }
            else
            {
                if (OpenDirection == DoorOpenDirection.Front)
                {
                    AttachDoor.FrontOpen();
                }
                else
                {
                    AttachDoor.BackOpen();
                }
            }
        }

        /// <summary>
        /// 开门的方向
        /// </summary>
        public enum DoorOpenDirection
        {
            /// <summary>
            /// 前开门
            /// </summary>
            Front,
            /// <summary>
            /// 后开门
            /// </summary>
            Back
        }
    }
}
