using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStruct : MonoBehaviour
{
    public class B
    {
        public int w;
        public int z;
    }


    public struct A
    {
        public int x;
        public int y;
    }
    A a = new A { x = 1, y = 2 };
    B b = new B { w = -1, z = -2 };
    B[] bArray = new B[10];
    A[] aArray = new A[2];
    A[] aArray2 = new A[2];
    A aCopy;

    private void Start()
    {
        //Debug.Log(a.x + " / " + a.y);
        //Debug.Log(b.w + " " + b.z);
        //ChangeV(ref a);
        ChangeB(b);
        //Debug.Log(b.w + " " + b.z);
        bArray[0] = new B();
        //Debug.Log(bArray[0].w + " " + bArray[0].z);
        //Debug.Log(a.x + " / " + a.y);

        aArray[0].x = -1;
        aArray[0].y = -1;

        aArray2 = aArray; // copy address
        aArray2[0] = aArray[0]; // copy data

        foreach (var item in aArray2)
        {
            Debug.Log(item.x);
            Debug.Log(item.y);
        }
        aArray2[0].x = -99;
        foreach (var item in aArray)
        {
            Debug.Log(item.x);
            Debug.Log(item.y);
        }
        aCopy = a; // copy data
        aCopy.x = -999;
        Debug.Log(a.x);
        Debug.Log(a.y);

    }
    void ChangeB( B _b)
    {
        _b.w = 7;
        _b.z = 8;
        //Debug.Log(b.w + " " + b.z);
    }
    void ChangeV(ref A _a)
    {
        _a.x = 4;
        _a.y = 5;
        //Debug.Log(_a.x + " / " + _a.y);
    }

}
