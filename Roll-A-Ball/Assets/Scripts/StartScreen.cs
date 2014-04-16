using UnityEngine;
using System.Collections;

public class StartScreen : MonoBehaviour {
	public string levelToLoad = "biek2";
	private float timer = 3f;


	// Use this for initialization
	void Start () {
		StartCoroutine (StartLoader());
	}

	IEnumerator StartLoader()
	{
		yield return new WaitForSeconds (timer);
		Application.LoadLevel (levelToLoad );
	}
}
