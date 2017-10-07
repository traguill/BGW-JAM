using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour 
{
    public string bullet_tag;
    public Player player;

    float x=-175;
    float y= 96;
    float w=179;
    float h=-112;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == bullet_tag)
        {
            Debug.Log("Player: "+player.player_id+" has been hitted");
            player.Hit(col.gameObject.GetComponent<Bullet>());
        }
    }

    private void Update()
    {
        if (transform.parent.position.x < x)
            transform.parent.position = new Vector3(x, transform.parent.position.y);
        if (transform.parent.position.y > y)
            transform.parent.position = new Vector3(transform.parent.position.x, y);
        if (transform.parent.position.x > w)
            transform.parent.position = new Vector3(w, transform.parent.position.y);
        if (transform.parent.position.y < h)
            transform.parent.position = new Vector3(transform.parent.position.x, h);
    }
}
