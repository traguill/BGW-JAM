using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour 
{
    public string bullet_tag;
    public Player player;
	
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == bullet_tag)
        {
            Debug.Log("Player: "+player.player_id+" has been hitted");
            player.Hit(col.gameObject.GetComponent<Bullet>());
        }
    }
}
