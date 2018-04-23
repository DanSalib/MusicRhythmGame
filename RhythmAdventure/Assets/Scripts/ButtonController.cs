using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button startButton;
    public Button endButton;
    public Image dragRegion;
    private Stopwatch buttonTimer;
    public Text startButtonText;
    public Text endButtonText;
    public Image indicator;
    public IndicatorCollision indicatorCollision;

    const float ScaleSensitivity = 0.62f;
    private float startTime;
    public float duration;
    public float buttonScore = 0;

    private bool isDrag = false;
    private bool beginDragEvent = false;

    public delegate void ButtonClick(ButtonController button);
    public static event ButtonClick OnClicked;


    public void InitializeButton(float start, float startX, float startY, bool isDrag, float endX, float endY)
    {
        this.transform.SetAsFirstSibling();
        this.gameObject.transform.position = new Vector3(startX, startY);

        this.startButton.transform.SetParent(this.gameObject.transform, false);

        this.isDrag = isDrag;

        if(this.isDrag)
        {
            SetupDragRegion(startX, endX, startY, endY);
        }

        this.startButton.gameObject.SetActive(true);

        this.startTime = start;
        this.buttonTimer = new Stopwatch();
        this.buttonTimer.Start();

        StartCoroutine(this.ScaleIndicator());
    }

    public void SetupDragRegion(float x1, float x2, float y1, float y2)
    {
        Vector3 centerPos = new Vector3(x1 + x2, y1 + y2) / 2f;
        float scaleX = Mathf.Abs(x2 - x1);
        float scaleY = Mathf.Abs(y2 - y1);

        this.dragRegion.gameObject.transform.localScale = new Vector3((scaleX + scaleY)/100f, 1f);
        this.dragRegion.gameObject.transform.position = centerPos;

        float angle = Mathf.Atan2(y2 - y1, x2 - x1);
        this.dragRegion.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);

        this.endButton.transform.SetParent(this.gameObject.transform, false);
        this.endButton.transform.position = new Vector3(x2, y2);

        this.dragRegion.gameObject.SetActive(true);
        this.endButton.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(this.startButton != null && this.startButton.gameObject.activeSelf && this.buttonTimer.ElapsedMilliseconds > this.duration)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        else if (Input.GetMouseButton(0) && this.beginDragEvent && this.indicatorCollision.isHit)
        {
            StartCoroutine(this.MoveIndicator());

            this.buttonScore += 0.02f;
        }
        else if(Input.GetMouseButtonUp(0) && this.beginDragEvent)
        {
            this.buttonTimer.Stop();
            OnClicked(this);

            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
        //else if (this.gameButton != null && this.gameButton.gameObject.activeSelf)
        //{
        //    this.gameButton.image.color = new Vector4(1 - CalcColor(), CalcColor(), 0, 1);
        //}
	}

    public void ButtonClicked()
    {
        if(this.isDrag)
        {
            this.beginDragEvent = true;
        } else
        {
            float clickTime = this.buttonTimer.ElapsedMilliseconds;
            this.buttonTimer.Stop();
            this.buttonScore = CalcScore(clickTime);
            OnClicked(this);

            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }
    }

    public float CalcPerfectTime()
    {
        return ((this.duration) / 2f);
    }

    public float CalcScore(float clickTime)
    {
        return 1 - Mathf.Abs(clickTime - CalcPerfectTime()) / CalcPerfectTime();
    }

    public float CalcColor()
    {
        if(((this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime()) <= 1f)
        {
            return (this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        else if ((duration - this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime() <= 1)
        {
            return (duration - this.buttonTimer.ElapsedMilliseconds) / CalcPerfectTime();
        }
        return 0;
       
    }

    private IEnumerator ScaleIndicator()
    {
        Vector3 originalScale = this.indicator.transform.localScale;
        Vector3 destinationScale = new Vector3(0.6f, 0.6f, 0.6f);

        if (this.buttonTimer.IsRunning)
        {
            while(this.buttonTimer.ElapsedMilliseconds < (this.duration/2f))
            {
                this.indicator.transform.localScale = Vector3.Lerp(originalScale, destinationScale, this.buttonTimer.ElapsedMilliseconds / (this.duration / 2f));
                yield return null;
            }
        }
    }

    private IEnumerator MoveIndicator()
    {
        Vector3 originalLocation = this.indicator.transform.position;
        Vector3 destination = this.endButton.transform.position;

        if (this.buttonTimer.IsRunning)
        {
            while (this.buttonTimer.ElapsedMilliseconds < (this.duration))
            {
                this.indicator.transform.position = Vector3.Lerp(originalLocation, destination, this.buttonTimer.ElapsedMilliseconds / (this.duration / 2f));
                yield return null;
            }
        }
    }

}
