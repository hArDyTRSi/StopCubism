using UnityEngine;
using System.Collections;

public class TextFlasher : MonoBehaviour
{
private float _timer;
private TextMesh _tm;
//####################################################################################################

void Start()
{
	_tm = GetComponent<TextMesh>();
	_timer = Random.Range(0.0f, 10.0f);
}
	
void Update()
{
	_timer += Time.deltaTime * 10.0f;
	float col = 0.5f + 0.5f * Mathf.Sin(_timer);
	if(transform.position.z > 1.0f)
	{
		col = _tm.color.r * (1.0f - Time.deltaTime * 5.0f);
	}
	_tm.color = new Color(col, col, col);
}

//####################################################################################################
	
}
