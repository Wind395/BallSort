using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LevelBuilder : MonoBehaviour
{

    public GameObject tubePrefab;
    public GameObject ballPrefab;
    public List<Sprite> listSprite;
    public int numTubes;
    public int numBalls;
    [SerializeField] private Button randomLevelButton;
    [SerializeField] private GameObject winText;
    public RectTransform[] tubePosition;

    public List<Tube> tubes = new List<Tube>();
    public List<List<Sprite>> initialState = new List<List<Sprite>>();

    void Start()
    {
        Application.targetFrameRate = 60; // Đặt FPS tối đa
        QualitySettings.vSyncCount = 0; // Tắt VSync
        QualitySettings.antiAliasing = 4; // Tăng độ nét

        CreateTubes(numTubes);
        GenerateBalls(numBalls); // Tạo bóng cho các ống
        SaveInitialState(); // Lưu trạng thái ban đầu của các ống

        randomLevelButton.onClick.AddListener(RandomLevel); // Thêm sự kiện cho nút ngẫu nhiên
    }


    public void CreateTubes(int numTubes)
    {
        for (int i = 0; i < numTubes; i++)
        {
            GameObject newTube = Instantiate(tubePrefab, tubePosition[i]);
            tubes.Add(newTube.GetComponent<Tube>());
        }
    }
    #region Generate Balls
    private void GenerateBalls(int numBalls)
    {
       Debug.Log("Add Ball");
        List<Sprite> allBalls = new List<Sprite>();

        // Giới hạn số màu được sử dụng
        numBalls = Mathf.Min(numBalls, listSprite.Count); // Đảm bảo không vượt quá số sprite có sẵn
        for (int i = 0; i < numBalls; i++)
        {
            Sprite sprite = listSprite[i];
            for (int j = 0; j < 4; j++) // Mỗi màu có numBalls bóng
            {
                Debug.Log($"Add Sprite: {sprite.name}");
                allBalls.Add(sprite);
            }
        }
    
        Debug.Log("Shuffle");
        Shuffle(allBalls);
    
        SetBalls(allBalls, ballPrefab); // Tạo bóng cho các ống
    }

    public void SetBalls(List<Sprite> sprites, GameObject ballPrefab)
    {
        // Xóa các ball hiện có
        foreach (Tube tube in tubes)
        {
            tube.ClearBalls(); // Xóa tất cả bóng trong ống
        }

        int index = 0;

        Debug.Log("Generate Balls");
        for (int i = 0; i < tubes.Count - 2; i++)
        {
            Debug.Log("Add Ball");
            for (int j = 0; j < 4; j++)
            {
                Debug.Log("Added Ball");
                GameObject newBall = Instantiate(ballPrefab, tubes[i].bottomPosition.position, Quaternion.identity, tubes[i].transform);
                Balls ballComponent = newBall.GetComponent<Balls>();
                ballComponent.SetSprite(sprites[index]);
                tubes[i].AddBall(ballComponent);
                index++;
            }
        }
    }

    public void RestoreInitialState(List<List<Sprite>> initialState, GameObject ballPrefab)
    {
        // Xóa tất cả bóng hiện có
        foreach (Tube tube in tubes)
        {
            tube.ClearBalls();
        }

        // Khôi phục trạng thái ban đầu
        for (int i = 0; i < tubes.Count && i < initialState.Count; i++)
        {
            foreach (Sprite sprite in initialState[i])
            {
                GameObject newBall = Instantiate(ballPrefab, tubes[i].bottomPosition.position, Quaternion.identity, tubes[i].transform);
                Balls ballComponent = newBall.GetComponent<Balls>();
                ballComponent.SetSprite(sprite);
                tubes[i].AddBall(ballComponent);
            }
        }

        // Cập nhật vị trí bóng
        foreach (Tube tube in tubes)
        {
            tube.UpdateBallPositions();
        }
    }

    public void Shuffle(List<Sprite> listSprite)
    {
        for (int i = listSprite.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = listSprite[i];
            listSprite[i] = listSprite[j];
            listSprite[j] = temp;
        }
    }
    #endregion

    public void SaveInitialState()
    {
        initialState.Clear();
        foreach (Tube tube in tubes)
        {
            initialState.Add(new List<Sprite>(tube.GetBallSprites())); // Sao chép danh sách sprite
            Debug.Log("Save Initial State: " + initialState[initialState.Count - 1].Count + " balls in tube " + (initialState.Count - 1));
        }
    }


    public void RandomLevel()
    {
        // Xóa các bóng hiện có
        foreach (Tube tube in tubes)
        {
            tube.ClearBalls(); // Xóa tất cả bóng trong ống
        }

        // Tạo lại các bóng ngẫu nhiên
        GenerateBalls(numBalls); // Tạo bóng cho các ống
        foreach (Tube tube in tubes)
        {
            tube.UpdateBallPositions(); // Cập nhật vị trí bóng
        }
        
        winText.SetActive(false); // Ẩn thông báo thắng
        SaveInitialState(); // Lưu trạng thái ban đầu của các ống
    }

}
