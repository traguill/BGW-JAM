using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TURRET_ZONE
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

public class TurretManager : MonoBehaviour {

    public int ideal_bullet_numbers = 25;
    public float bullet_cooldown = 2f;
    public float dificulty_timer = 120f;

    public GameObject[] bullets;

    public static TurretManager current;

    List<Turret> north_turrets;
    List<Turret> east_turrets;
    List<Turret> west_turrets;
    List<Turret> south_turrets;

    List<Bullet> active_bullets;
    TURRET_ZONE last_zone = TURRET_ZONE.WEST;
    float current_time = 0f;
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
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void NormalBehaviour()
    {
        if(ideal_bullet_numbers < active_bullets.Count)
        {

        }
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
}
