using UnityEngine;
using System.Collections;

public class LifeCode : MonoBehaviour {

    public GameObject destroyMeOnDeath;
    public GameObject lifeBarFill;
    public float life = 100;

    private float maxLife;

    void Start()
    {
        maxLife = life;
        
    }

	public void Damage(float dmg)
    {
        life -= dmg;
        lifeBarFill.transform.localScale = new Vector3(life / maxLife, 1, 1);
        if(life <= 0)
        {
            Destroy(destroyMeOnDeath);
        }
    }
}
