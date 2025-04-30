using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Balls : MonoBehaviour
{
    public Sprite thisSprite;
    public RectTransform rectTransform;
    public float duration;

    public void SetSprite(Sprite sprite)
    {
        gameObject.GetComponent<Image>().sprite = sprite;
        thisSprite = sprite;
    }

    public IEnumerator SetPosition(Vector3 targetPos)
    {

        // Sử dụng DOTween để di chuyển đối tượng đến vị trí mới
        Tween t = rectTransform.DOMove(targetPos, duration).SetEase(Ease.Linear);
        yield return t.WaitForCompletion(); // Chờ cho đến khi tween hoàn thành

        // float elapsedTime = 0f;
        // Vector3 startPos = rectTransform.position;
        // while (elapsedTime < duration)
        // {
        //     rectTransform.position = Vector3.Slerp(startPos, targetPos, elapsedTime / duration);
        //     elapsedTime += Time.deltaTime;
        //     yield return null;
        // }
        // rectTransform.position = targetPos;
        //yield return new WaitForSeconds(time); // Giữ lại delay nếu cần
}
}
