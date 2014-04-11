using UnityEngine;
using System.Collections;
using SimplexNoise;

public class GenerateLevel : MonoBehaviour
{
public float timeDilation = 0.1f;
public float chanceDilation = 0.01f;

public GameObject[] prefabCuboid;

private float _spawnOffset;
private Transform _cameraPosition;
private int _lastSpawnPosition = -1;
private int _cuboidsPerSpawn;
private int _cuboidsMax;
static Vector2 OFFSET;

//####################################################################################################

void Start()
{
	_cameraPosition = Camera.main.transform;

	//TODO: calculate offset by ScreenSize
	_spawnOffset = 15.0f;

	//TODO: calculate cuboids per spawn by ScreenSize
	_cuboidsPerSpawn = 5;

	//cache count of different Cuboid-Prefabs
	_cuboidsMax = prefabCuboid.Length - 1;

	// cache random offset into noise-field
	Random.seed = Random.Range(0, int.MaxValue - 1);
	OFFSET = new Vector2(Random.value * 100.0f, Random.value * 100.0f);
//	OFFSET = new Vector2(Random.value, Random.value);

}


void Update()
{
	Vector3 camPos = _cameraPosition.position;
	int camX = (int)camPos.x;
	
	if(camX > _lastSpawnPosition)
	{
		_lastSpawnPosition = camX;

		for(int i=-_cuboidsPerSpawn; i<=_cuboidsPerSpawn; i++)
		{
			Vector3 newPos = new Vector3(camX + _spawnOffset, i, 0.0f);
//			float noise = CalcNoiseVal(newPos, offset, 1.0f);
			float noise = CalcNoiseVal(newPos);

			int whichCuboid = Mathf.Clamp((int)(noise * (Time.time * timeDilation)), 0, _cuboidsMax);

			float chance = Random.Range(0.0f, 1.0f);
			if(chance > 0.975f - Time.time * chanceDilation)
			{
				Instantiate(prefabCuboid[(int)whichCuboid], newPos, Quaternion.identity);
			}
//			}
		}
	}
}

//####################################################################################################

//	public static float CalcNoiseVal(Vector3 pos, Vector2 offset, float scale)
public static float CalcNoiseVal(Vector3 pos)
{
//		float noiseX = (pos.x + offset.x) * scale;
//		float noiseY = (pos.y + offset.y) * scale;
	float noiseX = pos.x + OFFSET.x;
	float noiseY = pos.y + OFFSET.y;
	return Mathf.Max(0.0f, Noise.Generate(noiseX, noiseY));
}

}
