using UnityEngine;
using System.Collections;

public class PlayerFollowScript : MonoBehaviour {

    public GameObject player;

    float minX = -1.47f;

	void Update ()
    {
        float playerX = player.transform.position.x;
        if (playerX < minX) playerX = minX;
        transform.position = new Vector3(playerX, transform.position.y, transform.position.z);
	}
}
