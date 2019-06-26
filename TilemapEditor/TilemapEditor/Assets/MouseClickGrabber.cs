using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseClickGrabber : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tile;
    public TextMeshProUGUI tileCount;
    float tileWidthInPixels = 0.0f;
    float tileHeightInPixels = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        tileHeightInPixels = Screen.height / 10.0f;
        tileWidthInPixels = Screen.width / 18.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            // Vector3 mPos = Input.mousePosition;
            Vector3 mPos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            if (mPos.y < Screen.height - tileHeightInPixels)
            {
                PlaceTile(ResolvePosition(mPos));
            }
        }
    }

    private void PlaceTile(Vector3Int pos)
    {
        //Debug.Log($"Placing on {pos}");
        tilemap.SetTile(pos, tile);
        tilemap.RefreshTile(pos);
        tileCount.text = tilemap.GetUsedTilesCount().ToString();
    }

    private Vector3Int ResolvePosition(Vector3 mPos)
    {
        mPos.z = 0.0f;
        return new Vector3Int(
            (int)(mPos.x),
            (int)(mPos.y),
            0
        );
        /*return new Vector3Int(
             (int)(mPos.x / tileWidthInPixels + this.transform.position.x - 9.0f),
             (int)(mPos.y / tileHeightInPixels + this.transform.position.y - 5.0f),
             0
         );*/
    }
}
