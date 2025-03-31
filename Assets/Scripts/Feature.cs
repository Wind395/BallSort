using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Feature : MonoBehaviour
{
    // Lấy danh sách ball đã Save lúc bắt đầu để chơi lại Level
    public void ResetLevel(List<Tube> tubes, GameObject winText, List<List<Color>> initialState, Stack<MoveCommand> moveHistory, GameObject ballPrefab)
    {
        for (int i = 0; i < tubes.Count; i++)
        {
            // Mỗi tube được reset lại theo danh sách màu đã lưu ban đầu.
            tubes[i].SetBalls(initialState[i], ballPrefab);
        }
        moveHistory.Clear();
        winText.gameObject.SetActive(false);
    }
}
