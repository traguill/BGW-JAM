using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    //Public variables
    public float max_velocity;
    public float max_acceleration;
    public float acceleration_step;
    public int max_power = 1;
    public Color current_color = Color.white;
    float time_inside_wall = 0f;
    //Private variables
    Vector3 velocity = Vector3.zero;
    Vector3 direction = Vector3.zero;
    int current_rebounds = 0;
    SpriteRenderer current_sprite;
    private bool holding = false; //One of the players is holding the ball
    private bool dead = false;
    private float acceleration;
    

    private void Start()
    {
        current_sprite = GetComponent<SpriteRenderer>();
        current_color = current_sprite.color;
        holding = false;
        TurretManager.current.AddBullet(this);
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        if(velocity.magnitude < max_velocity && holding == false)
        {
            acceleration += acceleration_step * Time.deltaTime;
            if (acceleration > max_acceleration)
                acceleration = max_acceleration;
            velocity += direction * acceleration * Time.deltaTime;
            if(velocity.magnitude > max_velocity)
            {
                velocity = direction * max_velocity;
            }
        }
        current_sprite.color = current_color;

        if(holding == false)
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

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (time_inside_wall > 0.25f)
                Destroy(gameObject);
            else
                time_inside_wall += Time.deltaTime;
        }
    }


    public void SetDirection(Vector3 new_dir)
    {
        if (new_dir.magnitude == 0)
            direction = -direction;
        else
        {
            direction = new_dir.normalized;
        }

        acceleration = 0.0f;
        velocity = Vector3.zero;
    }

    //Asks the ball to hold. If the ball is already holded by another player this method returns false
    public bool Hold()
    {
        if(!holding)
        {
            holding = true;
            return holding;
        }
        return false;
    }

    public void Release(Vector3 new_direction, float time_holded)
    {
        holding = false;
        SetDirection(new_direction);
        Debug.Log("Ball holded " + time_holded + " sec");
    }

    public void OnDestroy()
    {
        TurretManager.current.RemoveBullet(this);
    }

    public void IWantToDie()
    {
        dead = true;
        Destroy(gameObject, 0.5f);
    }
}
