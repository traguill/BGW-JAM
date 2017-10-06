using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    //Public variables
    public int max_velocity;
    public int max_power = 1;
    public Color current_color = Color.white;

    //Private variables
    Vector3 velocity = Vector3.up;
    int current_rebounds = 0;
    SpriteRenderer current_sprite;

    private void Start()
    {
        current_sprite = GetComponent<SpriteRenderer>();
        current_color = current_sprite.color;
        TurretManager.current.AddBullet(this);
    }

    // Update is called once per frame
    void Update()
    {

        if (velocity.magnitude != max_velocity)
        {
            velocity.Normalize();
            velocity *= max_velocity;
        }
        current_sprite.color = current_color;
        transform.position += velocity * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (max_power <= current_rebounds)
            {
                //Kill Bullet
                Destroy(gameObject);
            }
            else
            {
                ContactPoint2D contact = collision.contacts[0];
                velocity = Vector3.Reflect(velocity, contact.normal);
                current_rebounds++;
            }
        }
    }


    public void SetDirection(Vector3 new_vel)
    {
        if (new_vel.magnitude == 0)
            velocity = -velocity;
        else
        {
            velocity = new_vel;
        }

    }

    public void OnDestroy()
    {
        TurretManager.current.RemoveBullet(this);
    }
}
