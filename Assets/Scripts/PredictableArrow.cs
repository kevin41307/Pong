using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictableArrow : MonoBehaviour, IPooled<PredictableArrow>, ITransformFollower
{
    public bool isPlayer1;
    [HideInInspector]
    public float startAngle = 30f;
    [HideInInspector]
    public float arcAngle = 120f;
    [HideInInspector]
    public bool pauseRotate = false;
    
    float sign = 1f;
    float angle;

    public int poolID { get; set; }
    public ObjectPooler<PredictableArrow> pool { get; set; }
    public Transform target { get; set; }
    [SerializeField]
    private Vector3 m_followOffset;
    public Vector3 followOffset { get => m_followOffset; set => m_followOffset = value; }
    public bool pauseFollow { get; set; }
    public Vector3 testAngle;

    float rotateSpeed = 150f;

    private void OnEnable()
    {
        if (target != null)
        {
            AvatarCard card = AvatarCard.FindSpecifiedCard(target.gameObject);
            if (card.avatarID == 1)
            {
                m_followOffset.Set(0.4f, 0, 0);
                isPlayer1 = true;
            }
            else if (card.avatarID == 2)
            {
                m_followOffset.Set(-0.4f, 0, 0);
                isPlayer1 = false;
            }
        }
    }

    private void OnDisable()
    {
        target = null;
    }

    public void Follow()
    {
        if (pauseFollow == true) return;
            transform.position = target.position + m_followOffset;
    }

    public void StopFollow(GameObject applicant)
    {
        if (applicant != target.gameObject) return;
        pauseRotate = true;
        pauseFollow = true;
    }

    public void StartFollow(Transform _target, float _startAngle)
    {
        target = _target;
        startAngle = _startAngle;
        pauseRotate = false;
        pauseFollow = false;
    }

    public void UpdateArrowOrientation() //world-x-axis is arrow axis, counter-clockwise is positive direction;
    {
        if (pauseRotate) return;
        sign = (isPlayer1) ? -60 : 120;
        angle = sign + startAngle + Mathf.PingPong(Time.time * rotateSpeed , arcAngle);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);
    }
}
