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
