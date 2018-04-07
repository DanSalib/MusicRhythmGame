using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject buttonPrefab;
    ButtonController buttonController;
    ButtonController[] buttonMap;
    delegate void ButtonMapMethod();
    public float gameScore;


	// Use this for initialization
	void Start () {
        buttonMap = new ButtonController[] { ButtonEvent(1000, 4000, 0, 0), ButtonEvent(2000, 4500, 50, -50), ButtonEvent(2500, 6000, -100, 100) };
    }
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i< buttonMap.Length; i++)
        {
            if (buttonMap[i] != null && buttonMap[i].buttonClicked)
            {
                gameScore += buttonMap[i].score;
                buttonMap[i] = null;
            }  
        }
        UnityEngine.Debug.Log("total score = " + gameScore);
	}

    ButtonController ButtonEvent(float startTime, float endTime, float x, float y)
    {
        GameObject button = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        button.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        buttonController = button.GetComponent<ButtonController>();
        buttonController.NewButton(startTime, endTime, x, y);
        return buttonController;
    }


}
