using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> where T : MonoBehaviour, IPooled<T>
{
    public T[] instances;

    protected Stack<int> m_FreeIdx;

    public void Initialize(int count, T prefab)
    {
        instances = new T[count];
        m_FreeIdx = new Stack<int>(count);

        for (int i = 0; i < count; ++i)
        {
            instances[i] = Object.Instantiate(prefab);
            instances[i].gameObject.SetActive(false);
            instances[i].poolID = i;
            instances[i].pool = this;

            m_FreeIdx.Push(i);
        }
    }

    public T GetNew()
    {
        if(m_FreeIdx.Count <= 0 )
        {
#if UNITY_EDITOR            
            Debug.Log("ObjectPool" + typeof(T).Name + " is Empty.");
#endif
            return null;
        }
        int idx = m_FreeIdx.Pop();
        instances[idx].gameObject.SetActive(true);
        
        return instances[idx];
    }

    public T GetNew(float expiredTime) //TODO: auto extend pool size?
    {
        T instance = GetNew();
        if (instance != null)
            instance.StartCoroutine(StartGiveBack(instance, expiredTime));
        return instance;

    }
    IEnumerator StartGiveBack(T obj, float expiredtime)
    {
        yield return new WaitForSeconds(expiredtime);
        Free(obj);
        
    }

    public void Free(T obj)
    {
        m_FreeIdx.Push(obj.poolID);
        instances[obj.poolID].gameObject.SetActive(false);
#if UNITY_EDITOR
        /*
        string s = default;
        foreach (var item in m_FreeIdx)
        {
            s += (item.ToString() + " ");
        }
        Debug.Log(s);
        */
#endif
    }
}

public interface IPooled<T> where T : MonoBehaviour, IPooled<T>
{
    int poolID { set; get; }
    ObjectPooler<T> pool { set; get; }

}

public interface IVolatilize
{
    public abstract float expiredTime { get; }
    public abstract void GetNew(System.Action giveBackCB);
    public abstract void GiveBack();
    public abstract IEnumerator StartGiveBack(float time);
}