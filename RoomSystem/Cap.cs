using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 可打开的盖子
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Cap : MonoBehaviour
    {
        public Vector3 Opened;
        public Vector3 Closed;
        public OpenType TheOpenType = OpenType.Move;

        private bool _isOpen = false;

        /// <summary>
        /// 打开或关闭
        /// </summary>
        public void OpenOrClose()
        {
            if (_isOpen)
            {
                _isOpen = false;
                if (TheOpenType == OpenType.Rotate)
                {
                    transform.DOLocalRotateQuaternion(Quaternion.Euler(Closed), 1);
                }
                else
                {
                    transform.DOLocalMove(Closed, 1);
                }
            }
            else
            {
                _isOpen = true;
                if (TheOpenType == OpenType.Rotate)
                {
                    transform.DOLocalRotateQuaternion(Quaternion.Euler(Opened), 1);
                }
                else
                {
                    transform.DOLocalMove(Opened, 1);
                }
            }
        }

        /// <summary>
        /// 打开盖子
        /// </summary>
        public void Open()
        {
            if (!_isOpen)
            {
                _isOpen = true;
                if (TheOpenType == OpenType.Rotate)
                {
                    transform.DOLocalRotateQuaternion(Quaternion.Euler(Opened), 1);
                }
                else
                {
                    transform.DOLocalMove(Opened, 1);
                }
            }
        }

        /// <summary>
        /// 关闭盖子
        /// </summary>
        public void Close()
        {
            if (_isOpen)
            {
                _isOpen = false;
                if (TheOpenType == OpenType.Rotate)
                {
                    transform.DOLocalRotateQuaternion(Quaternion.Euler(Closed), 1);
                }
                else
                {
                    transform.DOLocalMove(Closed, 1);
                }
            }
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            OpenOrClose();
        }
    }

    public enum OpenType
    {
        Move,
        Rotate
    }
}
