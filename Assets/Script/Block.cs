using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public void SetMaterial(Material color)
    {
        GetComponent<Renderer>().material = color;
    }

    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetGhostColor() 
    {
        var oriColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(oriColor.r, oriColor.g, oriColor.b, 0.3f);
    }

    public void ReturnOriColor()
    {
        var oriColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(oriColor.r, oriColor.g, oriColor.b, 1.0f);
    }
}
