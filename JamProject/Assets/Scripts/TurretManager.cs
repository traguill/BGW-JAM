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

public class TurretManager : MonoBehaviour {

    public int ideal_bullet_numbers = 25;
    public float bullet_cooldown = 2f;
    public float dificulty_timer = 120f;
    public int initial_bullets_NS = 4;
    public int initial_bullets_WE = 3;
    public float dificulty_multi = 2;
    public float time_between_init_bullets = 0.25f;
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
    //Characters

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
        InitialBehaviour();
        Invoke("StartNormalBeh", 5f);
    }
    // Update is called once per frame
    void Update ()
    { 
        if(normal_behaviour)
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
            bullet_cooldown /= dificulty_multi;
            ideal_bullet_numbers *= (int)dificulty_multi;
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

    void InitialBehaviour()
    {
        StartCoroutine(DelayShoot(north_turrets, initial_bullets_NS));
        StartCoroutine(DelayShoot(south_turrets, initial_bullets_NS));
        StartCoroutine(DelayShoot(west_turrets, initial_bullets_WE));
        StartCoroutine(DelayShoot(east_turrets, initial_bullets_WE));
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
        for(int i = 0;i<num;i++)
        {
            int ran = Random.Range(0, turret_list.Count);
            turret_list[ran].max_bullet_power = max_power;
            turret_list[ran].Shoot(bullets[0]);
            yield return new WaitForSeconds(time_between_init_bullets);
        }
    }

    void StartNormalBeh()
    {
        normal_behaviour = true;
        max_power = 1;
    }

    void AddZone()
    {
        if (last_zone == TURRET_ZONE.WEST)
            last_zone = 0;
        else
            last_zone++;
    }
}
