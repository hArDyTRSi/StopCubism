using UnityEngine;
using System.Collections;

public class CuboidController : MonoBehaviour
{
public int hardness = 1;

public AudioClip death;

private bool _dead = false;
private int _scoreSupply = 0;

private Transform _camPos;
private ShipController _player;
private TextMesh _hardnessIndicator;

//####################################################################################################

void Start()
{
	_player = GameObject.FindWithTag("Ship").GetComponent<ShipController>();
	_camPos = Camera.main.transform;
	_hardnessIndicator = GetComponentInChildren<TextMesh>();
	_scoreSupply = hardness;
	_hardnessIndicator.text = hardness.ToString();
}


void Update()
{
	if(transform.position.x < _camPos.position.x - 15.0f)
	{
		Destroy(gameObject);
	}
}


void OnTriggerEnter()
{
	if(_dead)
	{
		return;
	}

	// reduce hardness
	hardness--;

	_hardnessIndicator.text = hardness.ToString();

	// if hardness reached 0, remove cuboid
	if(hardness < 1)
	{
		// let Cuboid die...
		StartCoroutine(Die());
	}
}

//####################################################################################################

IEnumerator Die()
{
	// supply score to Scoreboard
	_player.AddScore(_scoreSupply);


	_dead = true;

	// turn off Collider, so shots can go through this cuboid from now on
	gameObject.GetComponent<BoxCollider>().enabled = false;

	// play death-sound
	audio.PlayOneShot(death, 1.0f - Mathf.Clamp(Vector3.Distance(transform.position, _player.transform.position) / 20.0f, 0.0f, 1.0f));
	
	float randomDir = Random.Range(-0.2f, 0.2f);
	float deathTimer = 0.0f;
	while(deathTimer < 5.0f)
	{
		deathTimer += Time.deltaTime;

		// zoom Cuboid away
		transform.position = new Vector3(
				transform.position.x + deathTimer * 0.3f,
				transform.position.y + deathTimer * randomDir,
				transform.position.z + deathTimer * 0.2f);

		// rotate Cuboid
		transform.Rotate(deathTimer * 1.1f, deathTimer * 0.7f, deathTimer * 0.9f);

		yield return new WaitForEndOfFrame();
	}

	// eventually Die!
	Destroy(gameObject);
//	yield return null;
}

}
