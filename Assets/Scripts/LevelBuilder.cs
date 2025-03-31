using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBuilder : MonoBehaviour
{

    public int tubeCount;
    public int ballCount;
    public GameObject tubePrefab;
    public GameObject ballPrefab;
    public Transform[] tubePositions;
    public GameObject winText;
    public Button resetButton;
    public Button undoButton;

    private List<Tube> tubes = new List<Tube>();
    private Tube selectedTube = null;
    private List<List<Color>> initialState = new List<List<Color>>();
    private Stack<MoveCommand> moveHistory = new Stack<MoveCommand>();
    private bool selectedBall = false;
    private Feature feature;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        feature = GetComponent<Feature>();
        InitializeGame();
        resetButton.onClick.AddListener(OnResetButtonPressed);
        undoButton.onClick.AddListener(UndoMove);
    }

    void Update()
    {
        CheckWinCondition();
        MouseClick();
    }

    private void InitializeGame()
    {
        winText.gameObject.SetActive(false);
        CreateTubes();
        DistributeBalls();
        SaveInitialState();
    }

    # region BallControl
    private void MouseClick()
    {
        // Xử lý input chuột (bạn có thể bổ sung cho touch)
        if (Input.GetMouseButtonDown(0)) {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
            if (hit.collider != null && hit.collider.tag == "Tube") 
            {
                Debug.Log(hit.collider.name);
                Tube clickedTube = hit.collider.GetComponent<Tube>();
                if (clickedTube != null) {
                    Debug.Log("Clicked tube: " + clickedTube.name);
                    HandleTubeSelection(clickedTube);
                }
            }
            // Nếu nhấn vào ball, lấy tube chứa ball đó (giả sử ball là con của tube)
            else if (hit.collider.tag == "Ball")
            {
                Debug.Log(hit.collider.name);
                Balls clickedBall = hit.collider.GetComponent<Balls>();
                if (clickedBall != null)
                {
                    Tube parentTube = clickedBall.transform.parent.GetComponent<Tube>();
                    if (parentTube != null)
                    {
                        Debug.Log("Clicked ball's tube: " + parentTube.name);
                        HandleTubeSelection(parentTube);
                    }
                }
            }
        }
    }

    // Xử lý chọn tube và di chuyển ball
    void HandleTubeSelection(Tube clickedTube) 
    {
        if (selectedTube == null && !selectedBall) 
        {
            if (clickedTube.listBalls.Count > 0) 
            {
                Debug.Log("Selected tube: " + clickedTube.name);
                selectedTube = clickedTube;
                selectedBall = true;
                Balls ball = clickedTube.listBalls[clickedTube.listBalls.Count - 1];
                ball.BallMoveFromTube(selectedTube.transform.GetChild(0));
                // Có thể thêm hiệu ứng highlight tube đã chọn
            }
        } 
            else 
            {
                if (selectedTube == clickedTube && selectedBall)
                {
                    Balls balls = clickedTube.listBalls[clickedTube.listBalls.Count - 1];
                    Debug.Log("Selected the same tube");
                    selectedTube = null;
                    selectedBall = false;
                    balls.ReturnBall();
                    return;
                }

                AttemptMove(selectedTube, clickedTube);
                selectedTube = null;
            }
    }

    // Thực hiện di chuyển ball giữa các tube
    void AttemptMove(Tube fromTube, Tube toTube)
    {
        // Kiểm tra tube nguồn có ball không
        if (fromTube.listBalls == null || fromTube.listBalls.Count == 0)
        {
            Debug.Log("Cannot move ball from an empty tube");
            return;
        }

        // Lấy ball ở đỉnh tube (chỉ số cuối cùng: Count - 1)
        Balls ball = fromTube.listBalls[fromTube.listBalls.Count - 1];
        Debug.Log(ball.name);

        // Nếu tube đích đã đầy (>= 4 ball) thì không cho chuyển
        if (toTube.listBalls.Count >= 4)
        {
            Debug.Log("Cannot move ball to a full tube");
            ball.ReturnBall();
            selectedBall = false;
            return;
        }

        // Nếu tube đích không rỗng, kiểm tra màu của ball trên đỉnh tube đích
        if (toTube.listBalls.Count > 0)
        {
            Balls targetBall = toTube.listBalls[toTube.listBalls.Count - 1];
            if (targetBall.GetColor() != ball.GetColor())
            {
                Debug.Log("Cannot move ball to a tube with different color");
                ball.ReturnBall();
                selectedBall = false;
                return;
            }
        }


        // Thực hiện di chuyển ball, lớp MoveCommand để lưu lịch suất di chuyển
        MoveCommand lastMove = new MoveCommand(fromTube, toTube, fromTube.listBalls[fromTube.listBalls.Count - 1]);
        
        // Thêm vào lịch sử di chuyển
        lastMove.Execute();
        moveHistory.Push(lastMove);

        selectedBall = false;
        Debug.Log("Done");
    }
    # endregion

    # region CreateTube and RandomBall
    private void CreateTubes()
    {
        for (int i = 0; i < tubeCount; i++)
        {
            GameObject tubeObject = Instantiate(tubePrefab, tubePositions[i]);
            Tube tube = tubeObject.GetComponent<Tube>();
            tube.Setup(this);
            tubes.Add(tube);
        }
    }

    private void DistributeBalls()
    {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow};
        List<Color> allBalls = new List<Color>();

        // Tạo danh sách bóng với số lượng đều nhau (4 quả mỗi màu)
        foreach (Color color in colors)
        {
            for (int i = 0; i < 4; i++)
            {
                allBalls.Add(color);
            }
        }

        Shuffle(allBalls); // Xáo trộn danh sách bóng
    
        Debug.Log(tubes.Count);
        int ballIndex = 0;

        for (int i = 0; i < tubes.Count - 2; i++) // Chỉ điền bóng vào 4 ống đầu tiên
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject ballObj = Instantiate(ballPrefab, tubes[i].transform);
                Balls ball = ballObj.GetComponent<Balls>();
                ball.ColorSetBalls(allBalls[ballIndex]); // Gán màu sắc cho bóng
                tubes[i].AddBall(ball);
                ballIndex++;
            }
        }
    }
    # endregion

    # region Feature
    public void OnResetButtonPressed()
    {
        feature.ResetLevel(tubes, winText, initialState, moveHistory, ballPrefab);
    }

    public void UndoMove()
    {
        if (moveHistory.Count > 0)
        {
            ICommand lastCommand = moveHistory.Pop();
            lastCommand.Undo();
            selectedBall = false;
            Debug.Log("Undo move");
        }
    }
    # endregion

    # region Shuffle Method
    private void Shuffle(List<Color> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Color temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    # endregion

    # region Save and Load State
    // Lưu danh sách màu bóng ban đầu
    private void SaveInitialState()
    {
        initialState.Clear();
        foreach (Tube tube in tubes)
        {
            initialState.Add(tube.GetBallColors());
        }
    }
    #endregion

    # region WindCodition
    private void CheckWinCondition()
    {
        foreach (Tube tube in tubes)
        {
            if (!tube.IsSorted())
            {
                return;
            }
        }
        winText.gameObject.SetActive(true);
    }
    #endregion
}
