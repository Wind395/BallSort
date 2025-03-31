using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    // Danh sách chứa các Ball trong tube.
    public List<Balls> listBalls = new List<Balls>();

    // Tham chiếu đến LevelBuilder (giả sử LevelBuilder có method SelectTube)
    private LevelBuilder levelBuilder;

    // Thiết lập tham chiếu đến LevelBuilder
    public void Setup(LevelBuilder levelBuilder)
    {
        this.levelBuilder = levelBuilder;
    }


    // Thêm ball vào tube, trả về true nếu thêm thành công.
    public bool AddBall(Balls ball)
    {
        if (listBalls.Count < 4)
        {
            Debug.LogWarning("Thêm ball vào tube");
            listBalls.Add(ball);
            // Cập nhật vị trí của ball theo số lượng ball hiện có (bạn có thể tinh chỉnh lại vị trí nếu cần)
            ball.transform.localPosition = new Vector3(0, listBalls.Count * 2.0f, 0);
            return true;
        }
        return false;
    }

    // Loại bỏ ball ở đỉnh tube và trả về ball đó.
    public Balls RemoveBall(Balls ball)
    {
        if (listBalls.Count > 0)
        {
            Debug.LogWarning("Removing");
            // Lấy ball cuối cùng trong danh sách
            ball = listBalls[listBalls.Count - 1];
            listBalls.RemoveAt(listBalls.Count - 1);
            //ball.transform.SetParent(null); // Gỡ khỏi tube trước khi di chuyển
            return ball;
        }
        return null;
    }

    // Phương thức Reset: xóa hết ball hiện có và tạo lại từ danh sách màu được cung cấp.
    public void SetBalls(List<Color> colors, GameObject ballPrefab)
    {
        // Xóa các ball hiện có
        foreach (Balls ball in listBalls)
        {
            Destroy(ball.gameObject);
        }
        listBalls.Clear();

        // Tạo lại các ball theo danh sách màu
        for (int i = 0; i < colors.Count; i++)
        {
            GameObject ballObj = Instantiate(ballPrefab, this.transform);
            Balls ball = ballObj.GetComponent<Balls>();
            ball.ColorSetBalls(colors[i]); // Gán màu cho ball
            AddBall(ball);
        }
    }

    

    // Kiểm tra tube đã được sắp xếp (solved) hay chưa:
    // - Nếu tube trống thì coi là solved.
    // - Nếu tube có đủ 4 ball và tất cả ball cùng màu thì solved.
    public bool IsSorted()
    {
        if (listBalls.Count == 0)
            return true;
        if (listBalls.Count < 4)
            return false;
        Color firstColor = listBalls[0].GetColor();
        foreach (Balls ball in listBalls)
        {
            if (ball.GetColor() != firstColor)
                return false;
        }
        return true;
    }

    // Lấy danh sách các màu của các ball trong tube.
    public List<Color> GetBallColors()
    {
        List<Color> colors = new List<Color>();
        foreach (Balls ball in listBalls)
        {
            colors.Add(ball.GetColor());
        }
        return colors;
    }
}
