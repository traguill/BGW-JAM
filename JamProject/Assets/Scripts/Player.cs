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
        if (is_dead)
            return;

        if(is_parrying)
        {
            if( (Input.GetAxis(p1_parry) > 0.0f && player_id == 1) || (Input.GetAxis(p2_parry) > 0.0f && player_id == 2))
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
        float dx, dy;
        if(player_id == 1)
        {
            dx = Input.GetAxis(p1_x_axis);
            dy = Input.GetAxis(p1_y_axis);
        }
        else
        {
            dx = Input.GetAxis(p2_x_axis);
            dy = Input.GetAxis(p2_y_axis);
        }

        bullet_holded.Release(new Vector3(dx, dy, 0), holding_time);
    }

    public void Hit()
    {
        Debug.Log("Player: " + player_id + " hit");
        death_bar += hit_dmg;

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
            bullets_in_range.Add(col.GetComponent<Bullet>());
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.transform.tag == bullet_tag)
        {
            Debug.Log("Bullet exit in player: " + player_id);
            bullets_in_range.Remove(col.GetComponent<Bullet>());
        }
    }
}
