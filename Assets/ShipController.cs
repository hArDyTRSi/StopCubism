using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour
{
public int activeTurrets = 1;
public float forwardSpeed;
public float upDownSpeed;
public float rotationAngle;

public Transform[] turretPositions;
public GameObject prefabShot;
public AudioClip shot;
public AudioClip death;

private bool _paused = false;
private bool _dead = false;
private int _score = 0;
private float _screenHeight;
private float _cameraZ;

//HACK: remove me, once todo in Die() is implemented!!!
private float _zoomTimer = 0.0f;


private GameObject _player;
private TextMesh _scoreBoard;
private GameObject _pauseMenu;

//####################################################################################################

void Start()
{
	_screenHeight = Screen.height / 2;
	_cameraZ = -Camera.main.transform.position.z / 2.0f + 0.5f;
	_player = GameObject.FindGameObjectWithTag("Player");
	_scoreBoard = GameObject.FindWithTag("Scoreboard").GetComponent<TextMesh>();
	_pauseMenu = GameObject.FindWithTag("UpgradeMenu");
	_pauseMenu.SetActive(false);
}

void Update()
{ 
	// check for Pause-Change
	CheckPause();

	if(!_dead && !_paused)
	{
		ControlShip();

		Shoot();
	}
}

void FixedUpdate()
{ 
	// move Player forward in Level
	MoveShip();
}

void OnTriggerEnter()
{
	if(!_dead)
	{
		StartCoroutine(Die());
	}
}

//####################################################################################################

void MoveShip()
{
	_player.transform.position += Vector3.right * forwardSpeed * Time.fixedDeltaTime;
}

void ControlShip()
{
	// get Player-Input
	float upDown = Input.mousePosition.y - transform.position.y - _screenHeight;
	upDown -= (transform.position.y * _screenHeight) / _cameraZ;
	upDown /= _screenHeight;
		
	// reposition Player on Y-Axis
	Vector3 newPosition = new Vector3(
			transform.position.x,
			Mathf.Clamp(transform.position.y + upDown * upDownSpeed * Time.deltaTime, -_cameraZ, _cameraZ),
			transform.position.z);
	transform.position = newPosition;
		
	// rotate Player
	transform.rotation = Quaternion.Euler((transform.position.y / _cameraZ) * rotationAngle, 0.0f, 0.0f);

}

void Shoot()
{
	if(Input.GetButtonDown("Fire1"))
	{
		for(int p=0; p<activeTurrets; p++)
		{
			Instantiate(prefabShot, turretPositions[p].position + Vector3.right * 0.5f, Quaternion.identity);
		}
		audio.PlayOneShot(shot);
	}
}

IEnumerator Die()
{
	_dead = true;

	audio.PlayOneShot(death);

	//TODO: let ship burst away, part for part (lotsa cubes!)

	//HACK: remove once implemented upper todo
	while(_zoomTimer < 3.0f)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + _zoomTimer * 0.25f);
		transform.Rotate(_zoomTimer * 5.0f, 0.0f, 0.0f);
		_zoomTimer += Time.deltaTime;
		yield return new WaitForEndOfFrame();
	}

//	yield return new WaitForSeconds(3.0f);
	
	int hiscore = PlayerPrefs.GetInt("HiScore");
	if(_score > hiscore)
	{
		PlayerPrefs.SetInt("HiScore", _score);
		PlayerPrefs.Save();
	}
	Destroy(gameObject);

}

void CheckPause()
{
	if(Input.GetKeyDown(KeyCode.P))
	{
		if(_paused)
		{
			Time.timeScale = 1;
			_player.transform.position = _player.transform.position + Vector3.up * 100.0f;
			_pauseMenu.SetActive(false);
			_paused = false;
		}
		else
		{
			Time.timeScale = 0;
			_player.transform.position = _player.transform.position - Vector3.up * 100.0f;
			_pauseMenu.SetActive(true);
			_paused = true;
		}
	}

}
//####################################################################################################

public void AddScore(int score)
{
	_score += score;
	_scoreBoard.text = "SCORE: " + _score.ToString();
}

}
