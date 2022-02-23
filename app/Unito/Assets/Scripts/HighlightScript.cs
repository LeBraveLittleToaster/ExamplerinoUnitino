using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightScript : MonoBehaviour
{
    public delegate void OnTileClicked(int x, int y);
    public static event OnTileClicked onTileClicked;

    [SerializeField] private Color hightLightColor = Color.red;
    private Color _baseColor;
    private Renderer _renderer;
    private TileScript _tile;

    void Start()
    {
        _tile = GetComponentInParent<TileScript>();
        _renderer = GetComponent<Renderer>();
        _baseColor = _renderer.material.color;
    }
    // The following three methods need an attached collider to work, cause they belong to the physics part of unity
    private void OnMouseUp()
    {
        onTileClicked?.Invoke(_tile.XIndex, _tile.YIndex);
    }

    private void OnMouseEnter()
    {
        _renderer.material.color = hightLightColor;
    }

    private void OnMouseExit()
    {
        _renderer.material.color = _baseColor;
    }
}
