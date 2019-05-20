using UnityEngine;

namespace HT.Framework.Auxiliary
{
    /// <summary>
    /// 房间行为对象
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class RoomBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Collider[] colliders = GetComponents<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = true;
            }
        }
    }
}
