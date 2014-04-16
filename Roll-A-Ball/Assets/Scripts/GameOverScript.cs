using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {
	private float timer = 2f;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (GameOverLoader());
	}
	
	IEnumerator GameOverLoader()
	{
		yield return new WaitForSeconds (timer);
		if (Input.GetKey(KeyCode.Escape)) 
		{
			Application.Quit();
		}
		if (Input.GetKey(KeyCode.Space)) 
		{
			Application.LoadLevel("biek2");
		}
	}
}
