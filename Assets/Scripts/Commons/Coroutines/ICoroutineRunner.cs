using System.Collections;
using UnityEngine;

namespace Commons.Coroutines
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator routine);

        void StopCoroutineIfNotNull(Coroutine routine);

        void StopCoroutine(Coroutine routine);

        void StopCoroutine(IEnumerator routine);
    }
}
