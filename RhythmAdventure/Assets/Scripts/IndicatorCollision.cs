using UnityEngine;
using System.Collections;

public class IndicatorCollision : MonoBehaviour
{
    public bool isHit = false;

    public void OnPointerEnter()
    {
        UnityEngine.Debug.Log("hit!");
        this.isHit = true;
    }  

    public void OnPointerExit()
    {
        UnityEngine.Debug.Log("exit!");
        this.isHit = false;
    }
}