using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T ins;

    public static T Ins 
    {
        get 
        { 
            if (ins == null)
            {
                T[] objs = FindObjectsOfType<T>();
                if (objs.Length > 0)
                {
                    T obj = objs[0];
                    ins = obj;
                }
                else
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    ins = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
            }
            return ins; 
        } 
    }
}
