using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImpactCircleRender : MonoBehaviour
{
    Material material;
    Animator animator;
    readonly int m_ImpactCircle = Animator.StringToHash("ImpactCircle");
    private void Awake()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
        material = mesh.material;
        Wall.OnBallHitted.AddListener(PlayIt);
    }

    public void PlayIt(Vector3 pos)
    {
        material.SetVector("_CenterPosition", pos);
        animator.Play(m_ImpactCircle);
    }

    private void OnDisable()
    {
        Wall.OnBallHitted.RemoveListener(PlayIt);
    }

}
