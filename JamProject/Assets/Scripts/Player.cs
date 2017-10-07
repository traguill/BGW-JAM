using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{
    [Header("Balance")]
    public float base_movement_speed = 2.0f;
    public float hit_dmg = 10.0f;
    public float max_mov_increase = 5.0f;
    public float stunned_duration;
    public float recoil_duration;
    public float super_extasi_pc = 90.0f;
    public float max_holding_time = 2f;
    
    [Header("Debugging")]
    public float death_bar = 0.0f;
    public string bullet_tag = "Bullet";
    public float time_scale = 1;
    public int player_id;


    [Header("Particles")]
    public GameObject die_part1;
    public GameObject die_part2;


    [Header("Links")]
    public GameModule game_manager;
    

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
    private bool is_recoiling = false;
    private bool game_started = false;

    private float stunned_current_time = 0.0f;

    private List<Bullet> bullets_in_range; //TODO: Change transform for Bullet Class

    float last_velocity = 0f;

    //Movement
    private float movement_speed = 0.0f;

    //Hold the ball
    Bullet bullet_holded = null;
    float holding_time = 0.0f;
    public float bullet_offset;
    public float recoil_offset;
    float recoil_time;
    float recoil_speed = 30.0f;
    Vector3 start_recoil;
    Vector3 end_recoil;
    Vector2 last_direction;
    int hold_level = 0;

    //super cheto
    bool smiling_at_max = false;
    public int smile_level = 0;

    Animator anim;
    Animator smile_anim;
    SpriteRenderer s_ren;
    SpriteMask s_mask;
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
        smiling_at_max = false;
        anim = GetComponent<Animator>();
        s_ren = GetComponent<SpriteRenderer>();
        s_mask = GetComponentInChildren<SpriteMask>();
        foreach(Transform t in transform)
        {
            if (t.CompareTag("Smile"))
                smile_anim = t.GetComponent<Animator>();
        }
    }

	
	// Update is called once per frame
	void Update () 
    {
        if (game_started == false)
            return;

        s_mask.sprite = s_ren.sprite;
        s_ren.sortingOrder = (int)-transform.position.y;
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
                ParryStay();
                return; //Holding the parry
            }
            
            int angle = (int)(Mathf.Atan2(last_direction.x, last_direction.y)*Mathf.Rad2Deg);
            angle = Mathf.Abs(angle);
            Debug.Log("ANGLE:" + angle);
            if(anim != null)
            {
                if (angle < 30)
                    anim.SetTrigger("ShootUp");
                else if (angle < 110)
                    anim.SetTrigger("ShootRight");
                else if (angle < 135)
                    anim.SetTrigger("ShootAlmostDown");
                else
                    anim.SetTrigger("ShootDown");
            }

            //Release the parry
            SetBulletNewDirection();
            Recoil();
            is_parrying = false;
            return;
        }

        if(is_recoiling)
        {
            float dist_covered = (Time.time - recoil_time) * recoil_speed;
            float frac_journey = dist_covered / recoil_offset;
            transform.position = Vector3.Lerp(start_recoil, end_recoil, frac_journey);
            Stunned(recoil_duration);
            return;
        }

        if (stunned)
        {
            Stunned(stunned_duration);
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

        Vector3 velocity = new Vector3((dx * step), dy * step, 0);

        transform.position += velocity;
        if ((last_velocity == 0 && velocity.magnitude > 0) || ((velocity.magnitude == 0) &&  last_velocity > 0))
        {
            ResetAnimation();
        }
        if (anim == null)
            return;
        anim.SetFloat("velocity", velocity.magnitude);
        smile_anim.SetFloat("velocity", velocity.magnitude);

        if (Mathf.Abs(dx * step) > 0)
        {
            bool rotate = (dx * step) > 0 ? false : true;

            if (rotate)
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            else
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }

        
        last_velocity = velocity.magnitude;
    }

    void MovementP2()
    {
        float step = movement_speed * Time.deltaTime;
        float dx = Input.GetAxis(p2_x_axis);
        float dy = Input.GetAxis(p2_y_axis);

        Vector3 velocity = new Vector3((dx * step), dy * step, 0);

        transform.position += velocity;

        if (anim == null)
            return;

        anim.SetFloat("velocity", velocity.magnitude);
        smile_anim.SetFloat("velocity", velocity.magnitude);

        if (Mathf.Abs(dx * step) > 0)
        {
            bool rotate = (dx * step) > 0 ? false : true;

            if (rotate)
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            else
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
           
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

    void Stunned(float stun_time)
    {
        stunned_current_time += Time.deltaTime;
        if(stunned_current_time >= stun_time)
        {
            stunned = false;
            is_recoiling = false;
            stunned_current_time = 0.0f;
            Debug.Log("Player" + player_id + " is no longer stunned");
        }
    }

    void Recoil()
    {
        is_recoiling = true;

        Vector3 new_pos;
        new_pos.x = gameObject.transform.position.x - (last_direction.normalized.x * recoil_offset);
        new_pos.y = gameObject.transform.position.y - (last_direction.normalized.y * recoil_offset);
        new_pos.z = gameObject.transform.position.z;

        recoil_time = Time.time;
        start_recoil = gameObject.transform.position;
        end_recoil = new_pos;        
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
                    hold_level = 0;
                    Debug.Log("Player: " + player_id + "is parrying");
                    if(anim != null)
                        anim.SetTrigger("parry");
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
        Debug.Log("Player " + player_id + " is stunned");
    }

    void ParryStay()
    {
        float current_per = holding_time / max_holding_time;
        smile_anim.gameObject.SetActive(false);
        if (current_per < 1f/3f )
        {
            hold_level = 0;
        }
        else if (current_per < 2f / 3f)
        {
            if (hold_level == 1)
                return;
            if(anim != null)
            {
                anim.SetTrigger("parry");
                hold_level = 1;
            }
        }
        else
        {
            if (hold_level == 2)
                return;

            if (anim != null)
            {
                anim.SetTrigger("parry");
                
                hold_level = 2;
            }
        }
    }
    void SetBulletNewDirection()
    {
        smile_anim.gameObject.SetActive(true);
        if (Mathf.Abs(last_direction.x) > 0)
        {
            bool rotate = (last_direction.x) > 0 ? false : true;

            if (rotate)
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
            else
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }

        Vector3 new_pos;
        new_pos.x = gameObject.transform.position.x + gameObject.GetComponent<CircleCollider2D>().offset.x + (last_direction.normalized.x * bullet_offset);
        new_pos.y = gameObject.transform.position.y + gameObject.GetComponent<CircleCollider2D>().offset.y + (last_direction.normalized.y * bullet_offset);
        new_pos.z = bullet_holded.transform.position.z;
        bullet_holded.transform.position = new_pos;
        bullet_holded.gameObject.SetActive(true);

        if (anim != null)
            anim.SetBool("parry_end",true);

        int boost = smiling_at_max ? 1 : 0;
        bullet_holded.max_velocity *= (hold_level + 1 + boost) - (hold_level) * 0.5f;
        bullet_holded.acceleration_step *= (hold_level + 1 + boost) - (hold_level) * 0.5f;
        bullet_holded.max_power += hold_level + 1 + boost;
        bullet_holded.Release(new Vector3(last_direction.x, last_direction.y, 0), holding_time,smiling_at_max);
        bullet_holded = null;
    }

    public void Hit(Bullet bullet)
    {
        Debug.Log("Player: " + player_id + " hit");
        death_bar += hit_dmg * (bullet.max_power +1);
        int last_level = smile_level;
        bool found = bullets_in_range.Contains(bullet);

        if (found)
        {
            bullets_in_range.Remove(bullet);
            //Say bullet to destroy. Mark bullet as death. Remove it after anim.
            bullet.IWantToDie(transform.position);
        }

        if(death_bar >= 100.0f)
        {
            Debug.Log("Player: " + player_id + " is dead");
            death_bar = 100.0f;
            if(!is_dead)
            {
                ResetAnimation();
                anim.SetTrigger("Die");
                Invoke("CreateDieParticles", 2f);
                
            }
            
            smile_anim.gameObject.SetActive(false);
            is_dead = true;

            game_manager.EndGame(player_id);

            return;
        }

        if(death_bar >= super_extasi_pc)
        {
            smiling_at_max = true;
            smile_level = 3;

        }
        else if(death_bar >= 60)
        {
            smile_level = 2;

        }    
        else if (death_bar >= 30)
        {
            smile_level = 1;
            
        }
            
        if(last_level != smile_level)
        {
            AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(currentBaseState.fullPathHash, -1, 0f);
            smile_anim.SetInteger("SmileLevel", smile_level);
        }
        
        StartCoroutine(BlinkOnHit());
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

    IEnumerator BlinkOnHit()
    {
        float blinks = 0;
        while( blinks<5)
        { 
            s_mask.enabled = !s_mask.enabled;
            blinks++;
            yield return new WaitForSeconds(0.1f);
        }

        s_mask.enabled = false;
    }

    void ResetAnimation()
    {
        AnimatorStateInfo currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        anim.Play(currentBaseState.fullPathHash, -1, 0f);

        AnimatorStateInfo currentBaseState2 = smile_anim.GetCurrentAnimatorStateInfo(0);
        smile_anim.Play(currentBaseState2.fullPathHash, -1, 0f);
    }


    void CreateDieParticles()
    {
        Instantiate(die_part1, transform);
        Invoke("CreateDieParticles2", 0.5f);
    }

    void CreateDieParticles2()
    {
        Instantiate(die_part2, transform.position + Vector3.up * 62.2f, Quaternion.identity);
    }
    public void GameStars()
    {
        game_started = true;

    }
}
