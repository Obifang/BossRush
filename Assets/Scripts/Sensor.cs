using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public LayerMask IgnoreLayer;
    private int _colCount;

    private void OnEnable()
    {
        _colCount = 0;
    }

    public bool IsColliding()
    {
        return _colCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == IgnoreLayer)
            return;
        _colCount++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == IgnoreLayer)
            return;
        _colCount--;
    }
}
