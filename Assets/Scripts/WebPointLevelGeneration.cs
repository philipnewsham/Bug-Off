using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WebPointLevelGeneration : MonoBehaviour
{
    public static event Action<WebPointLevelGeneration> OnWebPointClicked;
    private Color _baseColour = Color.white;

    private void OnMouseUpAsButton()
    {
        OnWebPointClicked?.Invoke(this);
    }

    public void Highlight(bool isHighlighted)
    {
        SetColour(isHighlighted ? Color.cyan : _baseColour);
    }

    public void SetFly(bool isFly)
    {
        _baseColour = isFly ? Color.green : Color.white;
        SetColour(_baseColour);
    }

    public void SetSpider(bool isSpider)
    {
        _baseColour = isSpider ? Color.red : Color.white;
        SetColour(_baseColour);
    }

    public void SetColour(Color colour)
    {
        GetComponent<SpriteRenderer>().color = colour;
    }
}
