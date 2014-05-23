using UnityEngine;
using System.Collections;

public class TextFlasherStartButton : MonoBehaviour
{
private float _flashTimer;
private Vector3 _startPosition;
private TextMesh _startResumeText;
private ShipController _ship;
private GameObject _pauseMenu;
private GameObject _player;

//####################################################################################################

void Start()
{
	_player = GameObject.FindGameObjectWithTag("Player");
	_ship = GameObject.FindWithTag("Ship").GetComponent<ShipController>();
	_pauseMenu = GameObject.FindWithTag("UpgradeMenu");

	_startResumeText = GetComponentInChildren<TextMesh>();
	_flashTimer = 0.0f;

	_startPosition = transform.position;
	

	//TODO: make each char a single cube, (START=5 , RESUME=6, find a workaround!)
	/* GameObject[] cubes = GetChildren
	 * in mouseOver:
	 * for(i)
	 * cubes[i].position.z = sin(time + i)	// sine-jumping-cubes
	*/
}
	
void OnMouseEnter()
{
	_flashTimer = 0.0f;
}
void OnMouseOver()
{
	// flash text
//	_timer += (1.0f / 60.0f) * 10.0f;
	_flashTimer += Time.fixedDeltaTime;
	float col = 0.5f + 0.5f * Mathf.Cos((_flashTimer + Mathf.PI) * 5.0f);
	_startResumeText.color = new Color(col, col, col);

	// jump on background
	transform.position = new Vector3(
//			_startPosition.x,
			transform.position.x,
			_startPosition.y,
			_startPosition.z - Mathf.Abs(0.5f * Mathf.Cos((_flashTimer + 0.5f * Mathf.PI) * 5.0f))
	);

	// START / RESUME
	if(Input.GetMouseButton(0))
	{
		_pauseMenu.SetActive(false);
		if(_startResumeText.text == "START")
		{
			_ship.SetScore(0);
			_startResumeText.text = "RESUME";
		}
		_ship.paused = false;
		_player.transform.position = _player.transform.position + Vector3.up * 100.0f;
		Time.timeScale = 1;

		_ship.StartCoroutine("MoveOntoScreen");

		_startResumeText.color = Color.black;
//		transform.position = _startPosition;
	}
}
void OnMouseExit()
{
//	transform.position = _startPosition;

	_startResumeText.color = Color.black;
}

//####################################################################################################
	
}
