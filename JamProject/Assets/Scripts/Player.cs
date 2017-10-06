using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    [Header("Balance")]
    public float base_movement_speed = 2.0f;
    public float hit_dmg = 10.0f;
    public float max_mov_increase = 5.0f;
    public float stunned_duration = 2.0f;

    [Header("Debugging")]
    public float death_bar = 0.0f;
    public string bullet_tag = "Bullet";

    public int player_id;

    //Axis names
    private string p1_x_axis = "P1_MOV_HOR";
    private string p2_x_axis = "P2_MOV_HOR";
    private string p1_y_axis = "P1_MOV_VER";
    private string p2_y_axis = "P2_MOV_VER";
    private string p1_parry = "P1_PARRY";
    private string p2_parry = "P2_PARRY";
    private string p1_parry_x_axis = "P1_PARRY_HOR";
    private string p1_parry_y_axis = "P1_PARRY_VER";
    private string p2_parry_x_axis = "P2_PARRY_HOR";
    private string p2_parry_y_axis = "P2_PARRY_VER";

    private bool is_dead = false;
    
    //States (state machine?)
    private bool is_parrying = false;
    private bool stunned = false;

    private float stunned_current_time = 0.0f;

    private List<Bullet> bullets_in_range; //TODO: Change transform for Bullet Class

    //Movement
    private float movement_speed = 0.0f;

    //Hold the ball
    Bullet bullet_holded = null;
    float holding_time = 0.0f;
    public float bullet_offset;
    Vector2 last_direction;

    void Start()
    {
        movement_speed = base_movement_speed;
        is_dead = false;
        is_parrying = false;
        stunned = false;
        stunned_current_time = 0.0f;
        bullets_in_range = new List<Bullet>();
        bullet_holded = null;
        holding_time = 0.0f;
    }

	
	// Update is called once per frame
	void Update () 
    {
        //Don't look down....
        List<int> to_remove = new List<int>();
        for (int i = 0; i < bullets_in_range.Count; ++i)
        {
            if(bullets_in_range[i] == null)
            {
                to_remove.Add(i);
            }
        }

        foreach(int index in to_remove)
        {
            bullets_in_range.RemoveAt(index);
        }

            if (is_dead)
                return;

        if(is_parrying)
        {
            //Save aim direction
            if (player_id == 1)
            {
                last_direction.x = Input.GetAxis(p1_x_axis);
                last_direction.y = Input.GetAxis(p1_y_axis);
            }
            else
            {
                last_direction.x = Input.GetAxis(p2_x_axis);
                last_direction.y = Input.GetAxis(p2_y_axis);
            }

            if ( (Input.GetAxis(p1_parry) > 0.0f && player_id == 1) || (Input.GetAxis(p2_parry) > 0.0f && player_id == 2))
            {
                holding_time += Time.deltaTime;
                return; //Holding the parry
            }
            //Release the parry
            SetBulletNewDirection();
            is_parrying = false;
            return;
        }

        if (stunned)
        {
            Stunned();
            return;
        }

        if (player_id == 1)
        {
            MovementP1();
            ParryP1();
        }
        else
        {
            MovementP2();
            ParryP2();
        }
	}

    void MovementP1()
    {
        float step = movement_speed * Time.deltaTime;
        float dx = Input.GetAxis(p1_x_axis);
        float dy = Input.GetAxis(p1_y_axis);

        transform.position += new Vector3(dx * step, dy * step, 0);

    }

    void MovementP2()
    {
        float step = movement_speed * Time.deltaTime;
        float dx = Input.GetAxis(p2_x_axis);
        float dy = Input.GetAxis(p2_y_axis);

        transform.position += new Vector3(dx * step, dy * step, 0);
    }

    void ParryP1()
    {
        if(Input.GetAxis(p1_parry) > 0.0f)
        {
            ParryAction();
        }
    }

    void ParryP2()
    {
        if (Input.GetAxis(p2_parry) > 0.0f)
        {
            ParryAction();
        }
    }

    void Stunned()
    {
        stunned_current_time += Time.deltaTime;
        if(stunned_current_time >= stunned_duration)
        {
            stunned = false;
            Debug.Log("Player" + player_id + " is no longer stunned");
        }
    }

    void ParryAction()
    {
        if(!is_parrying)
        {
            if(bullets_in_range.Count > 0)
            {
                bool success = false;
                foreach(Bullet b in bullets_in_range)
                {
                    success = b.Hold();
                    if (success)
                    {
                        bullet_holded = b;
                        b.gameObject.SetActive(false);
                        break;
                    }
                }

                if(success)
                {
                    is_parrying = true;
                    holding_time = 0.0f;
                    Debug.Log("Player: " + player_id + "is parrying");
                }
                else
                {
                    ParryFail();
                }
            }
            else
            {
                ParryFail();
            }
        }
    }

    void ParryFail()
    {
        stunned = true;
        stunned_current_time = 0.0f;
        Debug.Log("Player " + player_id + " is stunned");
    }

    void SetBulletNewDirection()
    {
        Vector3 new_pos;
        new_pos.x = gameObject.transform.position.x + (last_direction.normalized.x * bullet_offset);
        new_pos.y = gameObject.transform.position.y + (last_direction.normalized.y * bullet_offset);
        new_pos.z = bullet_holded.transform.position.z;
        bullet_holded.transform.position = new_pos;
        bullet_holded.gameObject.SetActive(true);

        bullet_holded.Release(new Vector3(last_direction.x, last_direction.y, 0), holding_time);
        bullet_holded = null;
    }

    public void Hit(Bullet bullet)
    {
        Debug.Log("Player: " + player_id + " hit");
        death_bar += hit_dmg;

        bool found = bullets_in_range.Contains(bullet);

        if (found)
        {
            bullets_in_range.Remove(bullet);
            //Say bullet to destroy. Mark bullet as death. Remove it after anim.
            bullet.IWantToDie();
        }

   
        if(death_bar >= 100.0f)
        {
            Debug.Log("Player: " + player_id + " is dead");
            death_bar = 100.0f;
            is_dead = true;
            return;
        }

        movement_speed = base_movement_speed + max_mov_increase * (death_bar / 100.0f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.tag == bullet_tag)
        {
            Debug.Log("Bullet enter in player: " + player_id);
            Bullet b = col.GetComponent<Bullet>();
            if(b)
            {
                if(bullets_in_range.Contains(b) == false)
                    bullets_in_range.Add(col.GetComponent<Bullet>());
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.transform.tag == bullet_tag)
        {
            Debug.Log("Bullet exit in player: " + player_id);
            Bullet b = col.GetComponent<Bullet>();
            if(b)
            {
                if (bullets_in_range.Contains(b))
                    bullets_in_range.Remove(b);
            }
        }
    }
}
