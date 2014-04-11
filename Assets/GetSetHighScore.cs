using UnityEngine;
using System.Collections;

public class GetSetHighScore : MonoBehaviour
{

//####################################################################################################

void Start()
{
	if(PlayerPrefs.HasKey("HiScore"))
	{
		GetComponent<TextMesh>().text = "HI: " + PlayerPrefs.GetInt("HiScore").ToString();
	}
}
/*	
	void Update ()
	{
	
	}
*/
//####################################################################################################
	
}
