using System;

namespace Util
{
    public static class Enumerator
    {
        public static void InvokeFor<T>(T[] arr, Action<T> action)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                action(arr[i]);
            }
        }
    }

}
