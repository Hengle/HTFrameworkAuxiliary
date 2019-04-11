using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace HT.Framework.Auxiliary
{
    [DefaultExecutionOrder(-10)]
    public class NavMeshSourceTag : MonoBehaviour
    {
        public static List<MeshFilter> Meshes = new List<MeshFilter>();

        private void OnEnable()
        {
            var m = GetComponent<MeshFilter>();
            if (m != null)
            {
                Meshes.Add(m);
            }
        }

        private void OnDisable()
        {
            var m = GetComponent<MeshFilter>();
            if (m != null)
            {
                Meshes.Remove(m);
            }
        }

        public static void Collect(ref List<NavMeshBuildSource> sources)
        {
            sources.Clear();

            for (var i = 0; i < Meshes.Count; ++i)
            {
                var mf = Meshes[i];
                if (mf == null) continue;

                var m = mf.sharedMesh;
                if (m == null) continue;

                var s = new NavMeshBuildSource();
                s.shape = NavMeshBuildSourceShape.Mesh;
                s.sourceObject = m;
                s.transform = mf.transform.localToWorldMatrix;
                s.area = 0;
                sources.Add(s);
            }
        }
    }
}
