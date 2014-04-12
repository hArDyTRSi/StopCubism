﻿using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour
{
public static float timer = 0.0f;

public bool paused = true;
public int activeTurrets = 1;
public float forwardSpeed;
public float upDownSpeed;
public float rotationAngle;
public float shootSpeed = 0.5f;

public Transform[] turretPositions;
public GameObject prefabShot;
public AudioClip shot;
public AudioClip death;

private	bool _positioned = false;
private bool _dead = false;
private int _score = 0;
private float _screenHeight;
private float _cameraZ;
private float _shootTimer = 0.5f;
private Vector3 _startPosition;
private Vector3 _playerStartPosition;

//HACK: remove me, once todo in Die() is implemented!!!
private float _zoomTimer = 0.0f;

private GameObject _player;
private GameObject _pauseMenu;
private TextMesh _scoreBoard;
private TextMesh _startResume;
private GenerateLevel _genLevel;
//####################################################################################################

void Start()
{
	_screenHeight = Screen.height / 2;
	_cameraZ = -Camera.main.transform.position.z / 2.0f + 0.5f;
	_player = GameObject.FindGameObjectWithTag("Player");
	_scoreBoard = GameObject.FindWithTag("Scoreboard").GetComponent<TextMesh>();
	_pauseMenu = GameObject.FindWithTag("UpgradeMenu");
	_pauseMenu.SetActive(true);
	_startResume = _pauseMenu.GetComponentInChildren<TextMesh>();
	_genLevel = GameObject.FindWithTag("LevelGenerator").GetComponent<GenerateLevel>();
	//start in Menu, game paused
	Time.timeScale = 0;

	_playerStartPosition = _player.transform.position;
	_startPosition = transform.position;
	_player.transform.position = _player.transform.position - Vector3.up * 100.0f;
}

void Update()
{ 
	// add frametime to timer
	if(!paused)
	{
		timer += Time.deltaTime;
	}

	if(!_dead)
	{
		// check for Pause-Change
		CheckPause();

		if(!paused)
		{
			ControlShip();
			Shoot();
		}
	}
}

void FixedUpdate()
{ 
	// move Player forward in Level
	if(!_dead && !paused)
	{
		MoveShip();
	}
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
	if(_positioned)
	{
		_player.transform.position += Vector3.right * forwardSpeed * Time.fixedDeltaTime;
	}
}

void ControlShip()
{
//	if(Input.touchCount == 1)	// should work with GetMouseButton, need to test!
	if(Input.GetMouseButton(0))
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
}

void Shoot()
{
	if(!_positioned)
	{
		return;
	}

	_shootTimer -= Time.deltaTime;
//	if(Input.GetButtonDown("Fire1"))
	//TODO: Put Firebutton bottomRightCorner for Tablets, use key (Right mousebutton) for windows/web!
	if(_shootTimer < 0.0f)
	{
		_shootTimer = shootSpeed;

		for(int p=0; p<activeTurrets; p++)
		{
			Instantiate(prefabShot, turretPositions[p].position + Vector3.right * 0.5f, Quaternion.identity);
		}
		audio.PlayOneShot(shot);
	}
}

void CheckPause()
{
	if(Input.GetKeyDown(KeyCode.Space))
	{
		if(timer < 0.1f)
		{
			return;
		}
		if(paused)
		{
			Time.timeScale = 1;
			_player.transform.position = _player.transform.position + Vector3.up * 100.0f;
			_startResume.color = Color.black;
			_pauseMenu.SetActive(false);
			paused = false;
		}
		else
		{
			Time.timeScale = 0;
			_player.transform.position = _player.transform.position - Vector3.up * 100.0f;
			_startResume.color = Color.black;
			_pauseMenu.SetActive(true);
			paused = true;
		}
	}
		
}

// ############ COROUTINES ############

IEnumerator MoveOntoScreen()
{
//		transform.position -= Vector3.right * 5.0f;
	transform.position = new Vector3(transform.position.x - 5.0f, transform.position.y, transform.position.z);
	Debug.Log("start: " + _startPosition.x);

	while(transform.position.x < _startPosition.x)
	{
		Debug.Log("actual x: " + transform.position.x);
		Debug.Log("while: " + _startPosition.x);
		transform.position += Vector3.right * Time.deltaTime * 3.0f;
	
		yield return new WaitForEndOfFrame();
	}

	transform.position = new Vector3(_startPosition.x, transform.position.y, transform.position.z);
	Debug.Log("end" + _startPosition.x);

	_positioned = true;
}

public IEnumerator Die()
{
	_dead = true;

	audio.PlayOneShot(death);

	// Check if new Highscore!
	CheckHighScore();

	//TODO: let ship burst away, part for part (lotsa cubes!)
	//HACK: remove once implemented upper todo
	while(_zoomTimer < 3.0f)
	{
		transform.position = new Vector3(transform.position.x + _zoomTimer * 0.1f, transform.position.y, transform.position.z + _zoomTimer * 0.1f);
		transform.Rotate(_zoomTimer * 5.0f, 0.0f, 0.0f);
		_zoomTimer += Time.deltaTime;
		yield return new WaitForEndOfFrame();
	}

	// Reset, turn on Menu, etc.
	_zoomTimer = 0.0f;
	_startResume.text = "START";
	timer = 0.0f;
	
	_dead = false;
	_pauseMenu.SetActive(true);
	paused = true;
	_positioned = false;

	// reset Player - Position
	_player.transform.position = _playerStartPosition;
	_player.transform.position = _player.transform.position - Vector3.up * 100.0f;
	transform.position = _startPosition;
	transform.rotation = Quaternion.identity;

	// remove all Cuboids from Level
	GameObject[] cuboids = GameObject.FindGameObjectsWithTag("Cuboid");
	for(int i=0; i<cuboids.Length; i++)
	{
		Destroy(cuboids[i].gameObject);
	}
	_genLevel.lastSpawnPosition = -1;
	_genLevel.SetRandomOffset();

	// remove all Shots from Level
	GameObject[] shots = GameObject.FindGameObjectsWithTag("Shot");
	for(int i=0; i<shots.Length; i++)
	{
		Destroy(shots[i].gameObject);
	}

//	Destroy(gameObject);
}

void CheckHighScore()
{
	// Compare if new HighScore, if, SAVE!
	int hiscore = PlayerPrefs.GetInt("HiScore");
	if(_score > hiscore)
	{
		PlayerPrefs.SetInt("HiScore", _score);
		PlayerPrefs.Save();
	}
}

//####################################################################################################

public void AddScore(int score)
{
	if(!_dead)
	{
		_score += score;
		_scoreBoard.text = "SCORE: " + _score.ToString();
	}
}
public void SetScore(int score)
{
	_score = score;
	_scoreBoard.text = "SCORE: " + _score.ToString();
}

}
