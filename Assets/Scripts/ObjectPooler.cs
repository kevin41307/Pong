using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> where T : MonoBehaviour, IPooled<T>
{
    public T[] instances;

    protected Stack<int> m_FreeIdx;
    public List<int> notFreeIdx { protected set; get; }
    public Stack<int> FreeIdx { get => m_FreeIdx; } 
    private int lastInitIndex = 0;
    string prefab_name = default;

    public void Initialize(int count, T prefab) // 
    {
        instances = new T[count];
        prefab_name = prefab.name;
        m_FreeIdx = new Stack<int>(count);
        
        for (int i = lastInitIndex; i < lastInitIndex + count; ++i)
        {
            instances[i] = Object.Instantiate(prefab);
            instances[i].gameObject.SetActive(false);
            instances[i].name += " " + i; // invert id sequence +(lastInitIndex + count - 1 - i)
            instances[i].poolID = i; //invert id sequence lastInitIndex + count - 1 - i
            instances[i].pool = this;

            m_FreeIdx.Push(i);
        }
        notFreeIdx = new List<int>();
        lastInitIndex += count;
    }

    public T GetNew()
    {
        if(m_FreeIdx.Count <= 0 )
        {
#if UNITY_EDITOR            
            Debug.Log("ObjectPool: " + prefab_name + " is Empty.");
#endif
            return null;
        }
        int idx = m_FreeIdx.Pop();
        notFreeIdx.Add(idx);
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
    public T[] GetNews(bool active = true)
    {
        if (m_FreeIdx.Count <= 0)
        {
#if UNITY_EDITOR            
            Debug.Log("ObjectPool: " + prefab_name + " is Empty.");
#endif
            return null;
        }
        if (active)
        {
            notFreeIdx.AddRange(m_FreeIdx);
            m_FreeIdx.Clear();
        }
        for (int i = 0; i < instances.Length; i++)
        {
            if (active == false) break;
            instances[i].gameObject.SetActive(active);
        }
        return instances;
    }

    IEnumerator StartGiveBack(T obj, float expiredtime)
    {
        yield return new WaitForSeconds(expiredtime);
        Free(obj);
        
    }

    public void Free(T obj)
    {
        if (m_FreeIdx.Contains(obj.poolID)) return; // prevent index collision
        m_FreeIdx.Push(obj.poolID);
        notFreeIdx.Remove(obj.poolID);
        instances[obj.poolID].gameObject.SetActive(false);
#if UNITY_EDITOR
        /*
        string s = "ObjectPool" + typeof(T).Name;
        foreach (var item in m_FreeIdx)
        {
            s += (item.ToString() + " ");
        }
        Debug.Log(s);
        */
#endif
    }
    public void Frees(T[] objs)
    {
        for (int i = 0; i < objs.Length; i++)
        {
            m_FreeIdx.Push(objs[i].poolID);
            notFreeIdx.Remove(objs[i].poolID);
            instances[objs[i].poolID].gameObject.SetActive(false);
        }
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

public class pool2<T> : ObjectPooler<T>  where T : MonoBehaviour, IPooled<T>
{

}