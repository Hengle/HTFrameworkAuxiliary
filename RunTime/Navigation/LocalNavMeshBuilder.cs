using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace HT.Framework.Auxiliary
{
    [DefaultExecutionOrder(-9)]
    [DisallowMultipleComponent]
    public sealed class LocalNavMeshBuilder : MonoBehaviour
    {
        /// <summary>
        /// 导航网格烘培目标
        /// </summary>
        public Transform Tracked;
        /// <summary>
        /// 导航外围区域
        /// </summary>
        public Vector3 Size = new Vector3(80.0f, 20.0f, 80.0f);
        /// <summary>
        /// 持续刷新导航网格
        /// </summary>
        public bool IsUpdataNavMeshData = false;

        private NavMeshData _navMesh;
        private AsyncOperation _operation;
        private NavMeshDataInstance _instance;
        private List<NavMeshBuildSource> _sources = new List<NavMeshBuildSource>();

        IEnumerator Start()
        {
            if (IsUpdataNavMeshData)
            {
                while (true)
                {
                    UpdateNavMesh(true);
                    yield return _operation;
                }
            }
            else
            {
                UpdateNavMesh(true);
                yield return _operation;
            }
        }

        private void OnEnable()
        {
            _navMesh = new NavMeshData();
            _instance = NavMesh.AddNavMeshData(_navMesh);
            if (Tracked == null)
                Tracked = transform;
            UpdateNavMesh(false);
        }

        private void OnDisable()
        {
            _instance.Remove();
        }

        private void UpdateNavMesh(bool asyncUpdate = false)
        {
            NavMeshSourceTag.Collect(ref _sources);
            NavMeshBuildSettings defaultBuildSettings = NavMesh.GetSettingsByID(0);
            Bounds bounds = QuantizedBounds();

            if (asyncUpdate)
                _operation = NavMeshBuilder.UpdateNavMeshDataAsync(_navMesh, defaultBuildSettings, _sources, bounds);
            else
                NavMeshBuilder.UpdateNavMeshData(_navMesh, defaultBuildSettings, _sources, bounds);
        }

        private static Vector3 Quantize(Vector3 v, Vector3 quant)
        {
            float x = quant.x * Mathf.Floor(v.x / quant.x);
            float y = quant.y * Mathf.Floor(v.y / quant.y);
            float z = quant.z * Mathf.Floor(v.z / quant.z);
            return new Vector3(x, y, z);
        }

        private Bounds QuantizedBounds()
        {
            Vector3 center = Tracked ? Tracked.position : transform.position;
            return new Bounds(Quantize(center, 0.1f * Size), Size);
        }

        private void OnDrawGizmosSelected()
        {
            if (_navMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_navMesh.sourceBounds.center, _navMesh.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            Bounds bounds = QuantizedBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            Vector3 center = Tracked ? Tracked.position : transform.position;
            Gizmos.DrawWireCube(center, Size);
        }
    }
}
