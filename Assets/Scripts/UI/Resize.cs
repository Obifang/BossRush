using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resize : MonoBehaviour
{
    public float ResizeDuration = 1.0f;
    public float ResizeWidth = 1.0f;
    public float ResizeHeight = 1.0f;
    public float ResizeSpeed = 1.0f;
    public bool ReverseWhenCompleted = false;

    private Vector3 _originalScale;
    private RectTransform _rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResizeOnClick()
    {
        StartCoroutine(Resizer());
    }

    public IEnumerator Resizer()
    {
        float timer = ResizeDuration;
        Vector3 scale = _originalScale;
        while (timer >  0) {
            timer -= Time.deltaTime;

            scale.x += ResizeWidth * Time.deltaTime * ResizeSpeed;
            scale.y += ResizeHeight * Time.deltaTime * ResizeSpeed;
            _rectTransform.localScale = scale;
            yield return new WaitForEndOfFrame();
        }

        if (!ReverseWhenCompleted) {
            _rectTransform.localScale = _originalScale;
            yield break;
        }
        timer = ResizeDuration;

        while (timer > 0) {
            timer -= Time.deltaTime;

            scale.x -= ResizeWidth * Time.deltaTime * ResizeSpeed;
            scale.y -= ResizeHeight * Time.deltaTime * ResizeSpeed;
            _rectTransform.localScale = scale;
            yield return new WaitForEndOfFrame();
        }

        _rectTransform.localScale = _originalScale;
    }
}
