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

    const float DragScoreModifier = 0.05f;

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
            StartCoroutine(this.FadeAway());
        }
        else if (Input.GetMouseButton(0) && this.beginDragEvent && this.indicatorCollision.isHit)
        {
            StartCoroutine(this.MoveIndicator());

            this.buttonScore += ButtonController.DragScoreModifier;
        }
        else if(Input.GetMouseButtonUp(0) && this.beginDragEvent)
        {
            this.buttonTimer.Stop();
            OnClicked(this);

            StartCoroutine(this.FadeAway());
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
            //this.buttonTimer.Stop();
            this.buttonScore = CalcScore(clickTime);
            OnClicked(this);


            StartCoroutine(this.FadeAway());

          //  this.gameObject.SetActive(false);
            //Destroy(this.gameObject);
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
                this.indicator.transform.position = Vector3.Lerp(originalLocation, destination, this.buttonTimer.ElapsedMilliseconds / (this.duration));
                yield return null;
            }
        }
    }

    public IEnumerator FadeAway()
    {
        Color originalColor = this.startButton.image.color;
        Color finalColor = new Color(this.startButton.image.color.r, this.startButton.image.color.g, this.startButton.image.color.b, 0);
        Color finalTextColor = new Color(this.startButton.image.color.r, this.startButton.image.color.g, this.startButton.image.color.b, 0.25f);

        Vector3 originalPosition = new Vector3();
       if(this.isDrag)
        {
            originalPosition = this.endButtonText.transform.position;
        } else
        {
            originalPosition = this.startButtonText.transform.position;

        }
        Vector3 destination = new Vector3(originalPosition.x, originalPosition.y + 50);

        float ElapsedTime = 0.0f;
        float TotalTime = 0.6f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            this.startButton.image.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.endButton.image.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.dragRegion.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.indicator.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));

            if (this.isDrag)
            {
                this.endButtonText.text = (Mathf.RoundToInt((this.buttonScore * 1000) / 100) * 100).ToString();
                this.endButtonText.gameObject.transform.position = Vector3.Lerp(originalPosition, destination, (ElapsedTime / TotalTime));
            }
            else
            {
                this.startButtonText.text = (Mathf.RoundToInt((this.buttonScore*1000)/100)*100).ToString();
                this.startButtonText.gameObject.transform.position = Vector3.Lerp(originalPosition, destination, (ElapsedTime / TotalTime));
            }

            this.startButtonText.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));
            this.endButtonText.color = Color.Lerp(originalColor, finalColor, (ElapsedTime / TotalTime));

            yield return null;
        }

        Destroy(this.gameObject);
    }

}
