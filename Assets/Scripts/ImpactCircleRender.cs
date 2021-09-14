using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImpactCircleRender : MonoBehaviour
{
    Material material;
    Animator animator;
    readonly int m_ImpactCircle = Animator.StringToHash("ImpactCircle");
    private void Start()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
        material = mesh.material;
        Wall.onBallCollisionEnter.AddListener(PlayIt);
    }

    public void PlayIt(Vector3 pos)
    {
        material.SetVector("_CenterPosition", pos);
        animator.Play(m_ImpactCircle);
    }

    private void OnDisable()
    {
        Wall.onBallCollisionEnter.RemoveListener(PlayIt);
    }

}
