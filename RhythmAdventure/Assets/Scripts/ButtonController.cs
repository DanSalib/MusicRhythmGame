using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {


    public Button gameButton;
    private Stopwatch gameTimer = new Stopwatch();
    private float startTime;
    private float endTime;
    public float score;
    public bool buttonClicked;

    public void NewButton(float start, float end, float x, float y)
    {
        startTime = start;
        endTime = end;
        gameButton.gameObject.transform.localPosition = new Vector3(x, y);
    }

	// Use this for initialization
	void Start () {
        gameButton.gameObject.SetActive(false);
        buttonClicked = false;
        gameTimer.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(gameTimer.ElapsedMilliseconds >= startTime && gameTimer.ElapsedMilliseconds <= endTime)
        {
            if (buttonClicked)
            {
                gameButton.gameObject.SetActive(false);
                gameTimer.Stop();
                gameTimer.Reset();
                OnDestroy();
                return;
            } else
            {
                gameButton.image.color = new Vector4(1 - CalcColor(), CalcColor(), 0, 1);
                gameButton.gameObject.SetActive(true);
            }
            
        }else
        {
            gameButton.gameObject.SetActive(false);
        }
	}

    public void ButtonClicked()
    {
        buttonClicked = true;
        float clickTime = gameTimer.ElapsedMilliseconds - startTime;
        score = CalcScore(clickTime);
        UnityEngine.Debug.Log("score = " + score);

    }

    public float CalcPerfectTime()
    {
        return ((endTime - startTime) / 2f);
    }

    public float CalcScore(float clickTime)
    {
        UnityEngine.Debug.Log("clickTime = " + clickTime);
        UnityEngine.Debug.Log("PerfectTime = " + CalcPerfectTime());
        return 1 - Mathf.Abs(clickTime - CalcPerfectTime()) / CalcPerfectTime();
    }

    public float CalcColor()
    {
        if(((gameTimer.ElapsedMilliseconds - startTime) / CalcPerfectTime()) <= 1f)
        {
            return (gameTimer.ElapsedMilliseconds - startTime) / CalcPerfectTime();
        }
        else if ((endTime - gameTimer.ElapsedMilliseconds) / CalcPerfectTime() <= 1)
        {
            return (endTime - gameTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        return 0;
       
    }
    private void OnDestroy()
    {
        Destroy(gameButton.gameObject);
    }
}
