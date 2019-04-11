using System;
using System.Collections;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    [Serializable]
    public sealed class LerpControlledBob
    {
        public float BobDuration;
        public float BobAmount;
        public float Offset { get; private set; }
        
        public IEnumerator DoBobCycle()
        {
            float t = 0f;
            while (t < BobDuration)
            {
                Offset = Mathf.Lerp(0f, BobAmount, t/BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            t = 0f;
            while (t < BobDuration)
            {
                Offset = Mathf.Lerp(BobAmount, 0f, t/BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            Offset = 0f;
        }
    }
}
