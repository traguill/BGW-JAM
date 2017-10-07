using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    
    public float shoot_ratio;
    public GameObject[] bullets;
    public float angle = 0;
    public int max_bullet_power = 5;
    public bool test_mode = false;
    public float offset;
    Vector3 direction = Vector3.up;
    float current_time;
    Animator anim;

    AudioSource audio_src;

    private void Start()
    {
        ChangeAngle(angle);
        anim = GetComponent<Animator>();
        if (anim == null)
            anim = transform.parent.GetComponent<Animator>();

        audio_src = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (test_mode)
            CheckShoot();
    }

    void CheckShoot()
    {
        if (shoot_ratio <= current_time)
        {
            Shoot(bullets[0]);
            current_time = 0f;
        }
        else
            current_time += Time.deltaTime;
    }

    public void Shoot(GameObject go)
    {
        anim.SetTrigger("Shoot");
        ChangeAngle(angle);
        GameObject bullet_go_tmp = Instantiate(go, transform.position + transform.right, Quaternion.identity);
        Bullet bullet_tmp = bullet_go_tmp.GetComponent<Bullet>();
        bullet_tmp.power = max_bullet_power;
        bullet_tmp.SetDirection(direction);
        audio_src.Play();
    }

    void ChangeAngle(float new_angle)
    {
        angle  = new_angle;
        float ang_tmp = Random.Range(-angle,  angle);
        direction = transform.right;
        Vector3 new_dir = direction;
        ang_tmp *= Mathf.Deg2Rad;
        float cos = Mathf.Cos(ang_tmp);
        float sin = Mathf.Sin(ang_tmp);
        new_dir.x = direction.x * cos - direction.y * sin;
        new_dir.y = -direction.x * sin + direction.y * cos;
        direction = new_dir;
    }
}
