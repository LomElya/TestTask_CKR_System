using System.Collections;
using UnityEngine;

namespace TestTask_CKR_System.Core.Utils.Coroutin
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        public Coroutine Run(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
    }
}