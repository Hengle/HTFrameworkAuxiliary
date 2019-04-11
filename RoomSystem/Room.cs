using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 房间
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class Room : MonoBehaviour
    {
        /// <summary>
        /// 当前Player所在的房间
        /// </summary>
        public static Room CurrentRoom;

        /// <summary>
        /// 房间名
        /// </summary>
        public string Name;
        /// <summary>
        /// 房间里的所有实体
        /// </summary>
        public GameObject[] Entities;
        /// <summary>
        /// 房间的所有门
        /// </summary>
        public Door[] Doors;

        /// <summary>
        /// 主角是否在房间内
        /// </summary>
        private bool _isExistHero = false;
        /// <summary>
        /// 当前是否展示房间里的实体
        /// </summary>
        private bool _isShowEntities = false;

        private void Start()
        {
            for (int i = 0; i < Doors.Length; i++)
            {
                Doors[i].OpenEvent += OnOpenDoor;
                Doors[i].CloseEvent += OnCloseDoor;
            }

            _isShowEntities = false;
            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i].SetActive(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_isExistHero || (_isExistHero && CurrentRoom != this))
            {
                if (other.tag == "Player")
                {
                    _isExistHero = true;
                    CurrentRoom = this;

                    OnOpenDoor();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isExistHero)
            {
                if (other.tag == "Player")
                {
                    _isExistHero = false;
                    if (CurrentRoom == this)
                    {
                        CurrentRoom = null;
                    }

                    OnCloseDoor();
                }
            }
        }

        /// <summary>
        /// 房间的门被打开
        /// </summary>
        private void OnOpenDoor()
        {
            if (!_isShowEntities)
            {
                _isShowEntities = true;
                for (int i = 0; i < Entities.Length; i++)
                {
                    Entities[i].SetActive(true);
                }
            }
        }

        /// <summary>
        /// 房间的门被关闭
        /// </summary>
        private void OnCloseDoor()
        {
            if (_isShowEntities)
            {
                for (int i = 0; i < Doors.Length; i++)
                {
                    if (Doors[i].IsOpened)
                    {
                        return;
                    }
                }

                if (_isExistHero)
                {
                    return;
                }

                _isShowEntities = false;
                for (int i = 0; i < Entities.Length; i++)
                {
                    Entities[i].SetActive(false);
                }
            }
        }
    }
}