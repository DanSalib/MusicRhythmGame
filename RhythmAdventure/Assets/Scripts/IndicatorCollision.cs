using UnityEngine;
using System.Collections;

public class IndicatorCollision : MonoBehaviour
{
    public bool isHit = false;

    public void OnPointerEnter()
    {
        this.isHit = true;
    }  

    public void OnPointerExit()
    {
        this.isHit = false;
    }
}