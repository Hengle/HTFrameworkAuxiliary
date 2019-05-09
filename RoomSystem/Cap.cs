using DG.Tweening;
using System;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 盖子
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Cap : RoomBehaviour
    {
        /// <summary>
        /// 打开盖子的方式
        /// </summary>
        public CapOpenType OpenType = CapOpenType.Move;
        /// <summary>
        /// 开门的值
        /// </summary>
        public Vector3 OpenValue;
        /// <summary>
        /// 关门的值
        /// </summary>
        public Vector3 CloseValue;

        /// <summary>
        /// 打开盖子事件
        /// </summary>
        public event Action OpenEvent;
        /// <summary>
        /// 关闭盖子事件
        /// </summary>
        public event Action CloseEvent;
        /// <summary>
        /// 盖子是否可以打开
        /// </summary>
        public bool IsCanOpen { get; set; } = true;
        /// <summary>
        /// 盖子是否可以关闭
        /// </summary>
        public bool IsCanClose { get; set; } = true;
        /// <summary>
        /// 是否是打开的
        /// </summary>
        public bool IsOpened { get; private set; } = false;

        private float _speed = 0.5f;

        /// <summary>
        /// 打开或关闭
        /// </summary>
        public void OpenOrClose()
        {
            if (IsOpened)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
        /// <summary>
        /// 打开盖子
        /// </summary>
        public void Open()
        {
            if (!IsOpened)
            {
                IsOpened = true;
                if (OpenEvent != null)
                {
                    OpenEvent();
                }
                if (OpenType == CapOpenType.Rotate)
                {
                    transform.DOLocalRotate(OpenValue, _speed);
                }
                else
                {
                    transform.DOLocalMove(OpenValue, _speed);
                }
            }
        }
        /// <summary>
        /// 关闭盖子
        /// </summary>
        public void Close()
        {
            if (IsOpened)
            {
                IsOpened = false;
                if (CloseEvent != null)
                {
                    CloseEvent();
                }
                if (OpenType == CapOpenType.Rotate)
                {
                    transform.DOLocalRotate(CloseValue, _speed);
                }
                else
                {
                    transform.DOLocalMove(CloseValue, _speed);
                }
            }
        }

        private void OnMouseDown()
        {
            if (GlobalTools.IsPointerOverUGUI())
            {
                return;
            }

            OpenOrClose();
        }

        /// <summary>
        /// 打开盖子的方式
        /// </summary>
        public enum CapOpenType
        {
            Move,
            Rotate
        }
    }
}
