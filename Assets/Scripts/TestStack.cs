using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStack : MonoBehaviour
{
    Stack<int> myStack = new Stack<int> ();

    private void Start()
    {
        myStack.Push(1);
        myStack.Peek();
        myStack.Pop();
        //myStack.Pop();
        
        //myStack.Peek();
    }
}
