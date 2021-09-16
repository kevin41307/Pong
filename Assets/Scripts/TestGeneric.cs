using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGeneric<T> where T : MonoBehaviour
{

}

public class A : MonoBehaviour
{

}

public class B : A
{

}


public interface IGeneric2<T>  where T : MonoBehaviour, IVolatilize
{
    TestGeneric<T> Start { get; set; }

}

public interface ICombibe<T> where T : MonoBehaviour, IVolatilize
{

}

public class Test : MonoBehaviour, IVolatilize
{
    public float expiredTime => throw new NotImplementedException();

    TestGeneric<Test> Start { get; set; }

    public void GetNew(Action giveBackCB)
    {
        throw new NotImplementedException();
    }

    public void GiveBack()
    {
        throw new NotImplementedException();
    }

    public IEnumerator StartGiveBack(float time)
    {
        throw new NotImplementedException();
    }
}