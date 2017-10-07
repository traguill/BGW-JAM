using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TURRET_ZONE
{
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3
}

public enum START_STATE
{
    READY = 0,
    STEADY = 1,
    GO = 2,
    MATCH = 3
}

public class TurretManager : MonoBehaviour {

    public int ideal_bullet_numbers = 25;
    public float bullet_cooldown = 2f;
    public float dificulty_timer = 120f;
    public int initial_bullets_NS = 4;
    public int initial_bullets_WE = 3;
    public int steady_bullets_NS = 4;
    public int steady_bullets_WE = 3;
    public int go_bullets_NS = 4;
    public int go_bullets_WE = 3;
    public float time_between_init_bullets = 0.25f;
    public float time_to_steady = 1f;
    public float time_to_go = 0.5f;
    public float cooldown_divider = 2f;
    public int ideal_bullet_mult = 2;
    public int max_power = 0;
    public GameObject[] bullets;

    public static TurretManager current;

    List<Turret> north_turrets;
    List<Turret> east_turrets;
    List<Turret> west_turrets;
    List<Turret> south_turrets;

    List<Bullet> active_bullets;
    TURRET_ZONE last_zone = TURRET_ZONE.WEST;
    float current_time = 0f;
    float current_dificulty_timer = 0f;
    bool normal_behaviour = false;
    bool max_dificulty = false;
    START_STATE current_start_state = START_STATE.READY;

    // Use this for initialization
    void Awake ()
    {
        if (current == null)
            current = this;
        else
            Destroy(this);

        AddAllTurretsToList(ref north_turrets, "NorthTurret");
        AddAllTurretsToList(ref east_turrets, "EastTurret");
        AddAllTurretsToList(ref west_turrets, "WestTurret");
        AddAllTurretsToList(ref south_turrets, "SouthTurret");

        active_bullets = new List<Bullet>();
    }

    private void Start()
    {
        current_start_state = START_STATE.MATCH;
        ChangeStartState(START_STATE.READY);
    }
    // Update is called once per frame
    void Update ()
    { 
        if(current_start_state == START_STATE.MATCH)
        {
            NormalBehaviour();
            if(!max_dificulty)
                DificultyBehaviour();
        }
            
    }

    void DificultyBehaviour()
    {
        if (dificulty_timer <= current_dificulty_timer)
        {
            bullet_cooldown /= cooldown_divider;
            ideal_bullet_numbers *= (int)ideal_bullet_mult;
            max_dificulty = true;
        }
        else
            current_dificulty_timer += Time.deltaTime;
    }

    void NormalBehaviour()
    {
        if(ideal_bullet_numbers > active_bullets.Count)
        {
            ChooseShootZone();
        }
        else
        {
            if (bullet_cooldown < current_time)
            {
                ChooseShootZone();
            }
            else
                current_time += Time.deltaTime;
        }
    }

    void InitialBehaviour(int ns,int we)
    {
        StartCoroutine(DelayShoot(north_turrets, ns));
        StartCoroutine(DelayShoot(south_turrets, ns));
        StartCoroutine(DelayShoot(west_turrets, we));
        StartCoroutine(DelayShoot(east_turrets, we));
    }


    public void AddBullet(Bullet b)
    {
        if (!active_bullets.Contains(b))
            active_bullets.Add(b);
    }

    public void RemoveBullet(Bullet b)
    {
        if(active_bullets.Contains(b))
            active_bullets.Remove(b);
    }

    void AddAllTurretsToList(ref List<Turret> turret_list, string tag)
    {
        turret_list = new List<Turret>();
        GameObject[] turrets = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject t in turrets)
        {
            Turret turret = t.GetComponent<Turret>();
            turret_list.Add(turret);
        }
    }

    void ChooseShootZone()
    {
        AddZone();
        switch (last_zone)
        {
            case TURRET_ZONE.NORTH:
                StartCoroutine(DelayShoot(north_turrets, 2));
                break;
            case TURRET_ZONE.EAST:
                StartCoroutine(DelayShoot(east_turrets, 2));
                break;
            case TURRET_ZONE.SOUTH:
                StartCoroutine(DelayShoot(south_turrets, 2));
                break;
            case TURRET_ZONE.WEST:
                StartCoroutine(DelayShoot(west_turrets, 2));
                break;
        }
        current_time = 0;
    }

    void Shoot(List<Turret> turret_list)
    {
        foreach(Turret t in turret_list)
        {
            t.max_bullet_power = max_power;
            t.Shoot(bullets[0]);
        }
    }

    IEnumerator DelayShoot(List<Turret> turret_list,int num)
    {
        START_STATE on_method_start = current_start_state;
        for(int i = 0;i<num;i++)
        {
            int ran = Random.Range(0, turret_list.Count);
            turret_list[ran].max_bullet_power = max_power;
            turret_list[ran].Shoot(bullets[0]);
            float rand = Random.Range(time_between_init_bullets * 0.9f, time_between_init_bullets * 1.1f);
            yield return new WaitForSeconds(rand + rand * i);
        }

        if (current_start_state != START_STATE.MATCH && on_method_start == current_start_state)
        {
            on_method_start++;
            ChangeStartState(on_method_start);
        }
            
    }

    void ChangeStartState(START_STATE new_state)
    {
        if (current_start_state == new_state)
            return;
        current_start_state = new_state;
        switch (new_state)
        {
            case START_STATE.READY:
                InitialBehaviour(initial_bullets_NS, initial_bullets_WE);
                break;
            case START_STATE.STEADY:
                Invoke("Steady", time_to_steady);
                break;
            case START_STATE.GO:
                Invoke("StartGO", time_to_go);
                break;
            case START_STATE.MATCH:
                StartNormalBeh();
                break;
        }
    }

    void Steady()
    {
        InitialBehaviour(steady_bullets_NS, steady_bullets_WE);
    }

    void StartGO()
    {
        InitialBehaviour(go_bullets_NS, go_bullets_WE);
    }

    void StartNormalBeh()
    {
        max_power = 3;
    }

    void AddZone()
    {
        if (last_zone == TURRET_ZONE.WEST)
            last_zone = 0;
        else
            last_zone++;
    }
}
