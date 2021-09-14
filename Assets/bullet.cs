using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    GunScript gun; 
    // Start is called before the first frame update
    void Start()
    {
        gun = FindObjectOfType<GunScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            gun.TargetHit(collision.gameObject.transform);
            Destroy(this.gameObject);
        }
    }
}
