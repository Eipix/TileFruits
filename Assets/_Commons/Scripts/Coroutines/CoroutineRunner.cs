using UnityEngine;

namespace Commons.Coroutines
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        public void StopCoroutineIfNotNull(Coroutine routine)
        {
            if(routine != null)
                StopCoroutine(routine);
        }
    }
}
