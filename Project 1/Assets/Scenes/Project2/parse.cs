using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
	public TextAsset file;
	public GameObject spawn; 
	public GameObject drone; 

	public List<Vector3> points;
	public int curPoint = 1;
	[SerializeField] LineRenderer line;
	
	void Start()
    {
		spawn.SetActive(false);
		points = ParseFile();
		for (int i = 1; i<points.Count;i++){
			GameObject cp = Instantiate(spawn, points[i], Quaternion.identity);
			cp.GetComponent<checkpoint>().order = i;
			cp.SetActive(true);
		}
		drone.transform.position = points[0];
    }

	void Update(){
		if (curPoint < points.Count){
			line.enabled = true;
			line.SetPosition(0, drone.transform.position);
			line.SetPosition(1, points[curPoint]);
			line.startColor=Color.blue;
            line.endColor=Color.blue;
		} else {
			line.enabled = false;
			drone.GetComponent<DroneController>().gameEnded = true;
		}
	}

	List<Vector3> ParseFile()
	{
		float ScaleFactor = 1.0f / 39.37f;
		List<Vector3> positions = new List<Vector3>();
		string content = file.ToString();
		string[] lines = content.Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			string[] coords = lines[i].Split(' ');
			Vector3 pos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
			positions.Add(pos * ScaleFactor);
		}
		return positions;
	}
}