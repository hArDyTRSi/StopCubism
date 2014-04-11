using UnityEngine;
using System.Collections;

public class ShotController : MonoBehaviour
{
public float shotSpeed;
public float selfDestroyRange = 10.0f;
private GameObject _player;

//####################################################################################################


void Start()
{
	_player = GameObject.FindWithTag("Player");
}

void FixedUpdate()
{
	//TODO: Test vs. AddForce();
	transform.position += Vector3.right * Time.fixedDeltaTime * shotSpeed;

	if(transform.position.x - _player.transform.position.x > selfDestroyRange)
	{
		Destroy(gameObject);
	}
}

void OnTriggerEnter()
{
	// remove shot
	Destroy(gameObject);
//	Debug.Log("HIT!");
}

//####################################################################################################

}
