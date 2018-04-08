using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class GameController : MonoBehaviour {

    public GameObject buttonPrefab;
    public Text scoreLabel;
    public List<float> buttonStartTimes;
    public List<float> buttonXPositions;
    public List<float> buttonYPositions;
    public float gameSpeed;
    public Stopwatch gameTimer = new Stopwatch();
    ButtonController buttonController;
    ButtonController[] buttonMap;
    public int gameScore = 0;
    public float frameDelay = 20;

	// Use this for initialization
	void Start () {
        gameTimer.Start();
        ButtonController.OnClicked += OnGameButtonClick;
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        float currentGameTime = gameTimer.ElapsedMilliseconds;
        if (buttonStartTimes.Count > 0 && currentGameTime > buttonStartTimes[0])
        {
            CreateButton(currentGameTime, buttonXPositions[0], buttonYPositions[0]);

            buttonStartTimes.RemoveAt(0);
            buttonXPositions.RemoveAt(0);
            buttonYPositions.RemoveAt(0);
        }
	}

    public void CreateButton(float startTime, float x, float y)
    {
        GameObject button = Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        button.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        buttonController = button.GetComponent<ButtonController>();
        buttonController.duration = gameSpeed;
        buttonController.InitializeButton(startTime, x, y);
    }

    public void OnGameButtonClick(ButtonController button)
    {
        this.gameScore += Mathf.RoundToInt(button.buttonScore * 1000);
        UpdateScoreLabel(gameScore);
    }

    private void UpdateScoreLabel(int scoreValue)
    {
        this.scoreLabel.text = "score: " + scoreValue;
    }

    private void OnDestroy()
    {
        ButtonController.OnClicked -= OnGameButtonClick;
    }

    /* OUTDATED
    private int isTimeContainedInRange(float value)
    {
        for (int i = 0; i < frameDelay; ++i)
        {
            if (this.buttonStartTimes.Contains(value + i))
            {
                return buttonStartTimes.IndexOf(value + i);
            }
        }
        return -1;
    }
    */

}
