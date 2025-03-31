using UnityEngine;

// Dữ liệu lưu một bước di chuyển để hỗ trợ Undo
public class MoveCommand : MonoBehaviour, ICommand 
{
    public Tube fromTube;
    public Tube toTube;
    public Balls ballMoved;

    // Constructor for creating a new move command
    public MoveCommand(Tube from, Tube to, Balls ball) {
        fromTube = from;
        toTube = to;
        ballMoved = ball;
    }

    // Command Pattern for moving ball
    public void Execute() 
    {
        fromTube.listBalls[fromTube.listBalls.Count - 1].BallMoveFromTube(toTube.transform.GetChild(0));
        fromTube.RemoveBall(ballMoved);
        toTube.AddBall(ballMoved);
        toTube.listBalls[toTube.listBalls.Count - 1].BallMoveToTube(toTube.transform.GetChild(0));
    }

    // Command Pattern for undoing the move
    public void Undo() 
    {
        toTube.listBalls[toTube.listBalls.Count - 1].BallMoveFromTube(toTube.transform.GetChild(0));
        toTube.RemoveBall(ballMoved);
        fromTube.AddBall(ballMoved);
        fromTube.listBalls[fromTube.listBalls.Count - 1].BallMoveToTube(fromTube.transform.GetChild(0));
    }
}