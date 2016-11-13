using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

    public GameObject bulletPrefab;

    public float SECONDS_BETWEEN_SHOTS = 3;
    private float shootTimer;


	// Use this for initialization
	void Start () {
        shootTimer = SECONDS_BETWEEN_SHOTS;
	}
	
	// Update is called once per frame
	void Update () {
        shootTimer -= Time.deltaTime;
        if(shootTimer <= 0)
        {
            shootTimer = SECONDS_BETWEEN_SHOTS;
            ShootBullet();
        }
	}

    void ShootBullet()
    {
        GameObject bullet = (GameObject) Instantiate(bulletPrefab);
        bullet.transform.position = gameObject.transform.position;
    }
}
