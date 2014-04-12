using UnityEngine;
using System.Collections;

public class TextFlasherStartButton : MonoBehaviour
{
private float _timer;
private TextMesh _tm;
private ShipController _ship;
private GameObject _pauseMenu;
private GameObject _player;

//####################################################################################################
	
void Start()
{
	_player = GameObject.FindGameObjectWithTag("Player");
	_ship = GameObject.FindWithTag("Ship").GetComponent<ShipController>();
	_pauseMenu = GameObject.FindWithTag("UpgradeMenu");
	
	_tm = GetComponentInChildren<TextMesh>();
	_timer = 0.0f;
}
	
void OnMouseEnter()
{
	_timer = 0.0f;
}
void OnMouseOver()
{
	// flash text
	_timer += (1.0f / 60.0f) * 10.0f;
	float col = 0.5f + 0.5f * Mathf.Sin(_timer);
	_tm.color = new Color(col, col, col);

	if(Input.GetMouseButton(0))
	{
		_pauseMenu.SetActive(false);
		if(_tm.text == "START")
		{
			_ship.SetScore(0);
			_tm.text = "RESUME";
		}
		_ship.paused = false;
		_player.transform.position = _player.transform.position + Vector3.up * 100.0f;
		Time.timeScale = 1;
//		_ship.StartCoroutine(MoveOntoScreen());
		_ship.StartCoroutine("MoveOntoScreen");
	}
}
void OnMouseExit()
{
	_tm.color = new Color(0.0f, 0.0f, 0.0f);
}

//####################################################################################################
	
}
