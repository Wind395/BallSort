using System.Collections;
using UnityEngine;

// Dữ liệu lưu một bước di chuyển để hỗ trợ Undo
//[System.Serializable]
public class MoveCommand : MonoBehaviour
{
    public Tube fromTube;
    public Tube toTube;
    public Balls ballMoved;

    // Constructor for creating a new move command
    public MoveCommand(Tube from, Tube to, Balls ball) 
    {
        fromTube = from;
        toTube = to;
        ballMoved = ball;
    }

    // Command Pattern for moving ball
    public IEnumerator Execute(Tube to, Balls ball) 
    {
        // Di chuyển bóng từ tube1 sang tube2
        toTube.balls.Add(ball);
        fromTube.balls.Remove(ball);
        yield return ball.StartCoroutine(ball.SetPosition(to.topPosition.position));
        ball.transform.SetParent(to.transform);

        int index = to.balls.IndexOf(ball);
        Vector3 targetPos = to.GetBallPosition(index);
        yield return ball.StartCoroutine(ball.SetPosition(targetPos));
    }

    // Command Pattern for undoing the move
    public IEnumerator Undo(Tube from, Tube to, Balls ball) 
    {
        // Trả bóng về tube1
        yield return ball.StartCoroutine(ball.SetPosition(to.topPosition.position));
        fromTube.balls.Add(ball);
        toTube.balls.Remove(ball);
        yield return ball.StartCoroutine(ball.SetPosition(from.topPosition.position));
        ball.transform.SetParent(from.transform);
        yield return ball.StartCoroutine(ball.SetPosition(from.GetBallPosition(from.balls.Count - 1)));
    }
}