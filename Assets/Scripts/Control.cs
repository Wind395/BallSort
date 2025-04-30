using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Control : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Button undoButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI fpsShowText;
    [SerializeField] private GameObject winText; // Tham chiếu đến Text Win

    [SerializeField] private LevelBuilder levelBuilder; // Tham chiếu đến LevelBuilder để truy cập các ống và bóng
    private Tube selectedTube = null;
    private Stack<MoveCommand> moveCommands = new Stack<MoveCommand>();
    private bool isProcessingMove = false; // Biến mới thay thế isMoving

    void Start()
    {
        undoButton.onClick.AddListener(UndoLastMove);
        resetButton.onClick.AddListener(() => ResetLevel(winText, levelBuilder.initialState, moveCommands, levelBuilder.ballPrefab));
    }

    void Update()
    {
        fpsShowText.text = "FPS: " + (1.0f / Time.deltaTime).ToString("F2");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Nếu đang xử lý di chuyển, bỏ qua mọi nhấp chuột
        if (isProcessingMove) return;

        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            Debug.Log("No raycast found");
            return;
        }

        Debug.Log("Click on: " + eventData.pointerCurrentRaycast.gameObject.name);
        Tube clickedTube = null;

        if (eventData.pointerCurrentRaycast.gameObject.tag == "Tube")
        {
            clickedTube = eventData.pointerCurrentRaycast.gameObject.GetComponent<Tube>();
            Debug.Log("Clicked on tube: " + clickedTube.name + " with balls: " + clickedTube.balls.Count);
        }
        else if (eventData.pointerCurrentRaycast.gameObject.tag == "Ball")
        {
            clickedTube = eventData.pointerCurrentRaycast.gameObject.transform.parent.GetComponent<Tube>();
            Debug.Log("Clicked on tube: " + clickedTube.name + " with balls: " + clickedTube.balls.Count);
        }

        if (clickedTube != null)
        {
            HandleTubeSelection(clickedTube);
        }
    }

    public void HandleTubeSelection(Tube clickedTube)
    {
        if (selectedTube == null)
        {
            if (clickedTube.balls.Count > 0)
            {
                Debug.Log("Selected Tube: " + clickedTube.name);
                selectedTube = clickedTube;
                StartCoroutine(SelectTubeCoroutine(clickedTube));
            }
        }
        else
        {
            if (selectedTube == clickedTube)
            {
                Debug.Log("Putting ball back in same tube");
                StartCoroutine(MoveBallBackCoroutine(selectedTube));
            }
            else
            {
                StartCoroutine(MoveBallCoroutine(selectedTube, clickedTube));
            }
            selectedTube = null;
        }
    }

    private IEnumerator SelectTubeCoroutine(Tube tube)
    {
        isProcessingMove = true;
        Balls ball = tube.balls[tube.balls.Count - 1];
        isProcessingMove = false;
        yield return ball.StartCoroutine(ball.SetPosition(tube.topPosition.position));
    }

    private IEnumerator MoveBallBackCoroutine(Tube tube)
    {
        isProcessingMove = true;
        Balls ball = tube.balls[tube.balls.Count - 1];
        int originalIndex = tube.balls.IndexOf(ball);
        isProcessingMove = false;
        yield return ball.StartCoroutine(ball.SetPosition(tube.GetBallPosition(originalIndex)));
    }

    private IEnumerator MoveBallCoroutine(Tube fromTube, Tube toTube)
    {
        isProcessingMove = true;
        Balls ball = fromTube.balls[fromTube.balls.Count - 1];

        // Kiểm tra điều kiện di chuyển
        if (toTube.balls.Count >= 4)
        {
            Debug.LogWarning("Tube is full, cannot move ball!");
            isProcessingMove = false;
            yield return ball.StartCoroutine(ball.SetPosition(fromTube.GetBallPosition(fromTube.balls.Count - 1)));
        }
        else if (toTube.balls.Count > 0 && toTube.balls[toTube.balls.Count - 1].thisSprite != ball.thisSprite)
        {
            Debug.LogWarning("Wrong color, cannot move ball!");
            isProcessingMove = false;
            yield return ball.StartCoroutine(ball.SetPosition(fromTube.GetBallPosition(fromTube.balls.Count - 1)));
        }
        else
        {
            // Di chuyển hợp lệ
            MoveCommand moveCommand = new MoveCommand(fromTube, toTube, ball);
            isProcessingMove = false;
            yield return StartCoroutine(moveCommand.Execute(toTube, ball));
            moveCommands.Push(moveCommand);
            WinCondition();
            Debug.Log("Move successful");
        }
        
    }

    // Hàm này sẽ được gọi khi nhấn nút Undo
    public void UndoLastMove()
    {
        if (moveCommands.Count > 0 && !isProcessingMove)
        {
            MoveCommand lastMove = moveCommands.Pop();
            StartCoroutine(UndoCoroutine(lastMove));
        }
        else
        {
            Debug.LogWarning("No moves to undo or move in progress!");
        }
    }

    private IEnumerator UndoCoroutine(MoveCommand moveCommand)
    {
        isProcessingMove = true;
        yield return StartCoroutine(moveCommand.Undo(moveCommand.fromTube, moveCommand.toTube, moveCommand.ballMoved));
        isProcessingMove = false;
    }

    public void ResetLevel(GameObject winText, List<List<Sprite>> initialState, Stack<MoveCommand> moveHistory, GameObject ballPrefab)
    {
        levelBuilder.RestoreInitialState(initialState, ballPrefab); // Khôi phục trạng thái ban đầu
        moveHistory.Clear();
        winText.gameObject.SetActive(false);
    }
    

    public void WinCondition()
    {
        foreach (Tube tube in levelBuilder.tubes)
        {
            if (!tube.IsSorted())
            {
                //winText.gameObject.SetActive(true); // Hiện thông báo thắng
                //Debug.Log("You win!");
                return;
            }
        }
        winText.gameObject.SetActive(true); // Hiện thông báo thắng
        Debug.Log("You win!");
    }

    
}