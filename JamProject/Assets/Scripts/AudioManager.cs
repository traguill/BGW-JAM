using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
    [HideInInspector]
    public static AudioManager amg;

    public float laugh_hit_delay = 0.5f;
    public AudioSource absorb1;
    public AudioSource absorb2;

    public AudioSource basic_shot1;
    public AudioSource basic_shot2;

    public AudioSource player_hit1;
    public AudioSource player_hit2;
    public AudioSource hit_laugh1;
    public AudioSource hit_laugh2;

    public AudioSource player_die1;
    public AudioSource player_die2;
    public AudioSource die_laugh1;
    public AudioSource die_laugh2;

    public AudioClip[] laugh;

    void Awake()
    {
        amg = this;
    }


	public void PlayAbsorb(int id, Vector3 position)
    {
        if(id == 1)
        {
            absorb1.transform.position = position;
            absorb1.Play();
        }
        else
        {
            absorb2.transform.position = position;
            absorb2.Play();
        }
    }

    public void PlayShot(int id, Vector3 position)
    {
        if (id == 1)
        {
            basic_shot1.transform.position = position;
            basic_shot1.Play();
        }
        else
        {
            basic_shot2.transform.position = position;
            basic_shot2.Play();
        }
    }

    public void PlayPlayerHit(int id, Vector3 position)
    {
        if (id == 1)
        {
            player_hit1.transform.position = position;
            hit_laugh1.transform.position = position;
            SetRandomLaugh(hit_laugh1);
            player_hit1.Play();
            hit_laugh1.PlayDelayed(laugh_hit_delay);
        }
        else
        {
            player_hit2.transform.position = position;
            hit_laugh2.transform.position = position;
            SetRandomLaugh(hit_laugh2);
            player_hit2.Play();
            hit_laugh2.PlayDelayed(laugh_hit_delay);
        }
    }

    void SetRandomLaugh(AudioSource src)
    {
        int id = Random.Range(0, 3);
        src.clip = laugh[id];
    }

    public void PlayPlayerDie(int id, Vector3 position)
    {
        if (id == 1)
        {
            player_die1.transform.position = position;
            die_laugh1.transform.position = position;
            player_die1.Play();
            die_laugh1.PlayDelayed(laugh_hit_delay);
        }
        else
        {
            player_die2.transform.position = position;
            die_laugh2.transform.position = position;
            player_die2.Play();
            die_laugh2.PlayDelayed(laugh_hit_delay);
        }
    }

}
