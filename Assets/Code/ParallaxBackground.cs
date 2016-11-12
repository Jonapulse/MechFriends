using UnityEngine;
using System.Collections;

public class ParallaxBackground : MonoBehaviour
{
	public float scrollFraction;

	private Vector3 camStartPosition;
	private Vector3 originalPosition;

	void Start ()
	{
		camStartPosition = Camera.main.transform.position;
		originalPosition = transform.position - Camera.main.transform.position;
	}

	void Update ()
	{
		transform.position = (Camera.main.transform.position - camStartPosition) * (1 - scrollFraction) + originalPosition;
	}
}
