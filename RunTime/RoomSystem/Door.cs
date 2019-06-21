using DG.Tweening;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 门
    /// </summary>
    public sealed class Door : RoomBehaviour
    {
        /// <summary>
        /// 是否自动开/关门（当Player触碰到门触发器时）
        /// </summary>
        public bool AutoOpen = true;
        /// <summary>
        /// 门的实体
        /// </summary>
        public Transform DoorEntity;
        /// <summary>
        /// 前触发器
        /// </summary>
        public DoorTrigger FrontTriggerEntity;
        /// <summary>
        /// 后触发器
        /// </summary>
        public DoorTrigger BackTriggerEntity;
        /// <summary>
        /// 开门的方式
        /// </summary>
        public DoorOpenType OpenType;
        /// <summary>
        /// 前开门的结束值
        /// </summary>
        public Vector3 FrontOpenValue;
        /// <summary>
        /// 后开门的结束值
        /// </summary>
        public Vector3 BackOpenValue;
        /// <summary>
        /// 关门的值
        /// </summary>
        public Vector3 CloseValue;

        /// <summary>
        /// 打开门事件
        /// </summary>
        public event HTFAction OpenEvent;
        /// <summary>
        /// 关闭门事件
        /// </summary>
        public event HTFAction CloseEvent;
        /// <summary>
        /// 门是否可以打开
        /// </summary>
        public bool IsCanOpen { get; set; } = true;
        /// <summary>
        /// 门是否可以关闭
        /// </summary>
        public bool IsCanClose { get; set; } = true;
        /// <summary>
        /// 是否是打开的
        /// </summary>
        public bool IsOpened { get; private set; } = false;

        private float _speed = 0.5f;

        private void Update()
        {
            if (AutoOpen)
            {
                if (IsOpened)
                {
                    if (!FrontTriggerEntity.IsTrigger && !BackTriggerEntity.IsTrigger)
                    {
                        Close();
                    }
                }
                else
                {
                    if (FrontTriggerEntity.IsTrigger)
                    {
                        FrontOpen();
                    }
                    else if (BackTriggerEntity.IsTrigger)
                    {
                        BackOpen();
                    }
                }
            }
        }

        /// <summary>
        /// 前开门
        /// </summary>
        public void FrontOpen()
        {
            if (IsCanOpen)
            {
                if (!IsOpened)
                {
                    IsOpened = true;
                    if (OpenEvent != null)
                    {
                        OpenEvent();
                    }
                    switch (OpenType)
                    {
                        case DoorOpenType.Rotate:
                            DoorEntity.DOLocalRotate(FrontOpenValue, _speed);
                            break;
                        case DoorOpenType.Stretch:
                            DoorEntity.DOLocalMove(FrontOpenValue, _speed);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 后开门
        /// </summary>
        public void BackOpen()
        {
            if (IsCanOpen)
            {
                if (!IsOpened)
                {
                    IsOpened = true;
                    if (OpenEvent != null)
                    {
                        OpenEvent();
                    }
                    switch (OpenType)
                    {
                        case DoorOpenType.Rotate:
                            DoorEntity.DOLocalRotate(BackOpenValue, _speed);
                            break;
                        case DoorOpenType.Stretch:
                            DoorEntity.DOLocalMove(BackOpenValue, _speed);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 关门
        /// </summary>
        public void Close()
        {
            if (IsCanClose)
            {
                if (IsOpened)
                {
                    IsOpened = false;
                    if (CloseEvent != null)
                    {
                        CloseEvent();
                    }
                    switch (OpenType)
                    {
                        case DoorOpenType.Rotate:
                            DoorEntity.DOLocalRotate(CloseValue, _speed);
                            break;
                        case DoorOpenType.Stretch:
                            DoorEntity.DOLocalMove(CloseValue, _speed);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 开门的方式
        /// </summary>
        public enum DoorOpenType
        {
            /// <summary>
            /// 旋转式门
            /// </summary>
            Rotate,
            /// <summary>
            /// 伸展式门
            /// </summary>
            Stretch
        }
    }
}
