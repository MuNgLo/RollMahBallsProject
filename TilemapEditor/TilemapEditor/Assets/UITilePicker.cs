using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class UITilePicker : MonoBehaviour
{
    public MouseClickGrabber mouseClickGrabber;
    public TileBase tile;
    public void OnClick()
    {
        mouseClickGrabber.tile = tile;
    }
}
