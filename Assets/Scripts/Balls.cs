using System.Collections;
using UnityEngine;

public class Balls : MonoBehaviour
{
    public Color ballColor;
    public float duration = 0.4f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void ColorSetBalls(Color color)
    {
        ballColor = color;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = ballColor;
    }

    public Color GetColor()
    {
        return ballColor;
    }

    // Thực thi khi lấy bóng ra khỏi Tube
    public void BallMoveFromTube(Transform outPos)
    {
        StopAllCoroutines();
        StartCoroutine(MoveBallSmoothly(outPos, false));
    }

    // Thực thi khi chuyển bóng vào Tube
    public void BallMoveToTube(Transform targetPos)
    {
        StopAllCoroutines();
        StartCoroutine(MoveBallSmoothly(targetPos, true));
    }

    private IEnumerator MoveBallSmoothly(Transform targetPos, bool isFinalMove)
    {
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        circleCollider.isTrigger = true;

        Vector3 endPos = targetPos.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(transform.position, targetPos.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; // Đảm bảo bóng đúng vị trí

        if (isFinalMove)
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            circleCollider.isTrigger = false;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
        }
    }

    // Thực thi khi muốn bóng quay về vị trí cũ
    public void ReturnBall()
    {
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        circleCollider.isTrigger = false;
    }
}
