using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    float vertical;
    float moveSpeed = 8f;
    Rigidbody2D rb;
    Vector2 startPos;
    Vector2 nextPosition;
    bool chargeBtn;
    float chargePower;
    bool chargeFull;
    float chargePosX ;
    float nextX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameManager.onResetGame.AddListener(Reset);
        startPos = transform.position;
        chargePosX = startPos.x - 1f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(chargePower);
        vertical = Input.GetAxisRaw("Vertical");
        chargeBtn = Input.GetButton("Jump");
        if (chargeBtn)
        {
            chargePower = Mathf.Clamp(chargePower + 8 * Time.deltaTime, 0, 1.1f);
        }
        else
        {
            if(chargePower > 1f)
            {
                chargeFull = true;
                StartCoroutine(StartChargeFull());
            }
            chargePower = 0;
        }
    }

    private void FixedUpdate()
    {
        if(chargeBtn)
        {
            nextX = Mathf.Lerp(rb.position.x, chargePosX, 12 * Time.fixedDeltaTime);
        }
        else
        {
            nextX = Mathf.Lerp(rb.position.x, startPos.x, 12 * Time.fixedDeltaTime);
        }
        nextPosition = rb.position + Vector2.up * vertical * moveSpeed * Time.fixedDeltaTime;
        nextPosition.x = nextX;
        rb.MovePosition(nextPosition);


    }

    private void Reset()
    {
        transform.rotation = Quaternion.identity;
        transform.position = startPos;
        rb.velocity = Vector3.zero;
    }

    IEnumerator StartChargeFull()
    {
        yield return new WaitForSeconds(.5f);
        chargeFull = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float chargeMultiplier = (chargeFull) ? 5f : 1f;


        if (collision.collider.CompareTag("ball"))
        {
            if (vertical != 0)
            {
                collision.collider.attachedRigidbody.AddTorque(-Mathf.Sign(vertical) * 40f);
            }
            else
            {
                collision.collider.attachedRigidbody.AddForce(new Vector2(vertical, .7f * vertical) * 20f);
            }
            if( chargeFull )
                collision.collider.attachedRigidbody.AddForce(new Vector2(vertical, .7f * vertical) * 40f * chargeMultiplier);
                
        }
    }
}
