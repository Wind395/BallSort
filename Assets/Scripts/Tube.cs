using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tube : MonoBehaviour
{
    public List<Balls> balls = new List<Balls>();
    public RectTransform topPosition;
    public RectTransform bottomPosition;
    public float ballSpace = 1; // Khoảng cách giữa các bóng

    

    void Start()
    {
        UpdateBallPositions();
    }


    // Add ball to the tube
    public void AddBall(Balls ball)
    {
        balls.Add(ball);
    }

    // Remove ball from the tube
    public void RemoveBall(Balls ball)
    {
        balls.Remove(ball);
    }

    // Get the number of balls in the tube
    public Vector3 GetBallPosition(int index)
    {
        if (balls.Count == 0) return bottomPosition.position;
        // Lấy chiều cao thực tế của bóng sau khi co giãn
        float ballHeight = balls[0].rectTransform.rect.height * balls[0].rectTransform.lossyScale.y;
        float spacing = ballHeight * ballSpace; // Khoảng cách điều chỉnh theo tỷ lệ co giãn
        return bottomPosition.position + new Vector3(0, index * spacing, 0);
    }

    // Get sprite of the balls in the tube
    public List<Sprite> GetBallSprites()
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach (var ball in balls)
        {
            sprites.Add(ball.thisSprite);
        }
        return sprites;
    }

    // Update the position of all balls in the tube
    public void UpdateBallPositions()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            Vector3 pos = GetBallPosition(i);
            balls[i].rectTransform.position = pos; // Đặt trực tiếp hoặc dùng SetPosition
        }
    }

    // Remove all balls from the tube
    public void ClearBalls()
    {
        foreach (var ball in balls)
        {
            Destroy(ball.gameObject);
        }
        balls.Clear();
    }

    // Check condition to win the game
    public bool IsSorted()
    {
        if (balls.Count == 0) return true;
        if (balls.Count < 4) return false;

        //Sprite firstSprite = balls[0].thisSprite;
        for (int i = 1; i < balls.Count; i++)
        {
            if (balls[0].thisSprite != balls[i].thisSprite)
            {
                Debug.Log("Tube is not sorted!");
                return false;
            }
        }
        return true;
    }
}
