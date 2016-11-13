using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    public LayerMask[] hitTheseLayers;

    public Vector3 velocity;

    public int damageAmount = 10;

    void OnCollisionEnter2D(Collision2D coll)
    {
        for(int i = 0; i < hitTheseLayers.Length; i++)
        {
            //Bitmask magic that check to see if you've hit a layer in 'hitTheseLayers'
            if (coll.collider && ((1 << coll.collider.gameObject.layer) & hitTheseLayers[i]) != 0)
            {
                LifeCode hitLife = coll.collider.gameObject.GetComponent<LifeCode>();
                if (hitLife != null) hitLife.Damage(damageAmount);
                Destroy(gameObject);
                
                break;
            }
        }      
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }
}
