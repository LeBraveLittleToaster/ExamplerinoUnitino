using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockOnClickedEventManager : MonoBehaviour
{
    void Start()
    {
        HighlightScript.onTileClicked += OnTileClickedListener;
    }

    private void OnTileClickedListener(int x, int y)
    {
        Debug.Log(x + " | " + y);
    }
}
