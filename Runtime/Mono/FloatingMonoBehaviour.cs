using System.Collections;
using UnityEngine;

public class FloatingMonoBehaviour
{
    class Holder : MonoBehaviour { }
    static Holder _coroutine;
    static Holder Coroutine
    {
        get
        {
            if (_coroutine == null)
                return _coroutine = new GameObject("FloatingMonoBehaviour").AddComponent<Holder>();

            return _coroutine;
        }
    }

    public static void StartCoroutine(IEnumerator coroutine)
    {
        Coroutine.StartCoroutine(coroutine);
    }
}
