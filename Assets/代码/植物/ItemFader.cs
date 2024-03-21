using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂载到作物身上
/// </summary>
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private readonly float fadeInSeconds = 0.25f;
    private readonly float fadeOutSeconds = 0.35f;
    private readonly float targetAlpha = 0.45f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(FadeOutRoutine());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        StartCoroutine(FadeInRoutine());
    }


    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }


    /// <summary>
    /// 将物体的透明度变为1f
    /// </summary>
    private IEnumerator FadeInRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float distance = 1f - currentAlpha;
        
        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha += distance / fadeInSeconds * Time.deltaTime;
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }
        
        _spriteRenderer.color = new Color(1f, 1f, 1f,1f);

    }
    
    /// <summary>
    /// 将物体的透明度变为targetAlpha
    /// </summary>
    private IEnumerator FadeOutRoutine()
    {
        float currentAlpha = _spriteRenderer.color.a;
        float distance = currentAlpha - targetAlpha;

        while (currentAlpha - targetAlpha > 0.01f)
        {
            //计算物体每帧需要减少的透明度，以实现物体逐渐淡出的效果。
            currentAlpha = currentAlpha - distance / fadeOutSeconds * Time.deltaTime;
            
            _spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            
            yield return null;
        }
        
        _spriteRenderer.color = new Color(1f, 1f, 1f, targetAlpha);
    }
}
