using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damageQuantity = 10;
    public float speed; //Velocidad de la bala
    public float activeTime = 3;
    public float shootTime;
    // Start is called before the first frame update
    void Start()
    {
        shootTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
        
    }

    void FixedUpdate()
    {
        if (Time.time - shootTime >= activeTime)
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().DamagePlayer(damageQuantity);
            Destroy(gameObject);
        }else Destroy(gameObject);
    }
}
