using System.Collections;
using UnityEngine;

namespace TestTask_CKR_System.Core.Utils.Coroutin
{
    public interface ICoroutineRunner
    {
        Coroutine Run(IEnumerator routine);
        void Stop(Coroutine coroutine);
    }
}