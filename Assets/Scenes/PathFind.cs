using Assets.Scenes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;

public class PathFind : MonoBehaviour
{
    [DllImport("PathFindCplusplus")]
    private static extern bool InitCPP(int w, int h, byte[] pathdata, int length);
    [DllImport("PathFindCplusplus")]
    private static extern bool ReleaseCPP();

    [DllImport("PathFindCplusplus")]
    private static extern bool FindCPP(Vector3 Start, Vector3 End, Vector2[] outPath, ref int pathCount);
    private bool isInitCPP = false;
    public Texture2D dataTex;
    byte[] data = null;
    List<Vector2> lstpath = new List<Vector2>();
    int data_width = 0;

    List<Cell> openList = new List<Cell>();//开启列表
    List<Cell> closeList = new List<Cell>();//关闭列表

    void LoadData()
    {
        data_width = dataTex.width;
        Color32[] tempdata = dataTex.GetPixels32();
        data = new byte[tempdata.Length];
        for (int i = 0; i < data.Length; i++)
        {
            if (tempdata[i].r == 0)
            {
                int j = 0;

            }
            data[i] = tempdata[i].r;

        }
    }
    public bool isWalkable(Vector2 v)
    {
        int x = (int)v.x;
        int y = (int)v.y;

        int temp = data[y * data_width + x];
        Debug.Log("x:" + x + " " + "y:" + y + " " + "data:" + temp);

        return data[y * data_width + x] != 0;
    }
    public bool isWalkable(int x, int y)
    {

        return data[y * data_width + x] != 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
    }

    private void OnDestroy()
    {
        if (isInitCPP)
        {
            ReleaseCPP();
        }
    }
    //测试函数 仅用于演示
    public bool TestFind(Vector2 begin, Vector2 end, List<Vector2> lstPath)
    {
        lstPath.Clear();

        if (!isWalkable(begin))
        {
            return false;
        }
        if (!isWalkable(end))
        {
            return false;
        }
        lstPath.Add(begin);
        lstPath.Add(new Vector2(890, 143));
        lstPath.Add(new Vector2(868, 901));
        lstPath.Add(new Vector2(858, 901));
        lstPath.Add(new Vector2(661, 358));
        lstPath.Add(end);

        return true;
    }
    //寻路函数入口 
    public bool Find(Vector2 start, Vector2 end, List<Vector2> lstPath)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        lstPath.Clear();


        if (!isWalkable(start))
        {
            return false;
        }
        if (!isWalkable(end))
        {
            return false;
        }

        //补充真正的寻路 如果寻路成功返回true 如果寻路失败返回false
        //和A*算法无关，只是为了显示使用
        int showFindNum = 1;



  


        //起点
        Cell startCell = new Cell((int)start[0], (int)start[1], false);
        //终点
        Cell endCell = new Cell((int)end[0], (int)end[1], false);

        Debug.LogFormat("寻路开始,start({0}),end({1})!", start, end);

        openList.Add(startCell);//将起点作为待处理的点放入开启列表

        //SortedAddToOpenList(startCell);

        //如果开启列表没有待处理点表示寻路失败，此路不通
        while (openList.Count > 0)
        {
            //遍历开启列表，找到消费最小的点作为检查点
            Cell cur = openList[0];
            //for (int i = 0; i < openList.Count; i++)
            //{
            //    if (openList[i].fCost < cur.fCost && openList[i].hCost < cur.hCost)
            //    {
            //        cur = openList[i];
            //    }
            //}
            //Debug.Log("当前检查点：" + cur.ToString() + " 编号：" + showFindNum + "  open列表节点数量：" + openList.Count);
            showFindNum++;

            //从开启列表中删除检查点，把它加入到一个“关闭列表”，列表中保存所有不需要再次检查的方格。
            openList.Remove(cur);
            closeList.Add(cur);

            //检查是否找到终点
            if (cur == endCell)
            {
                Cell cell = cur;
                while (cell != null)
                {
                    lstPath.Add(new Vector2(cell.x, cell.y));
                    cell = cell.parent;
                }

                Debug.Log("寻路结束！");

                //sw.Stop();
                TimeSpan ts2 = sw.Elapsed;
                Debug.Log("寻路结束！总共花费:" + ts2.TotalMilliseconds + "ms");
                return true;
            }

            //根据检查点来找到周围可行走的点
            //1.如果是墙或者在关闭列表中则跳过
            //2.如果点不在开启列表中则添加
            //3.如果点在开启列表中且当前的总花费比之前的总花费小，则更新该点信息
            List<Cell> aroundCells = GetAllAroundCells(cur);
            foreach (var cell in aroundCells)
            {
                if (cell.isWall || closeList.ContainsCell(cell)) continue;
                int cost = cur.gCost + GetDistanceCost(cell, cur);

                if (cost < cell.gCost || !openList.ContainsCell(cell))
                {
                    cell.gCost = cost;
                    cell.hCost = GetDistanceCost(cell, endCell);
                    cell.parent = cur;
                    //Debug.Log("cell:" + cell.ToString() + "  parent:" + cur.ToString());
                    if (!openList.ContainsCell(cell))
                        SortedAddToOpenList(cell);
                }
            }

        }
        Debug.Log("寻路失败!");

        return false;
    }

    void SortedAddToOpenList(Cell cell)
    {
        bool inserted = false;

        for (int i = 0; i < openList.Count; ++i)
        {
            if (cell.fCost < openList[i].fCost)
            {
                openList.Insert(i, cell);
                break;
            }
        }

        if (!inserted)
        {
            openList.Add(cell);
        }
    }

    /// <summary>
    /// 估价函数，启发函数
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    int GetDistanceCost(Cell a, Cell b)
    {
        int cntX = Mathf.Abs(a.x - b.x);
        int cntY = Mathf.Abs(a.y - b.y);
        // 判断到底是那个轴相差的距离更远
        if (cntX > cntY)
        {
            return 14 * cntY + 10 * (cntX - cntY);
        }
        else
        {
            return 14 * cntX + 10 * (cntY - cntX);
        }
    }

    // 取得周围的节点
    public List<Cell> GetAllAroundCells(Cell cell)
    {
        List<Cell> list = new List<Cell>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // 如果是自己，则跳过
                if (i == 0 && j == 0)
                    continue;
                int x = cell.x + i;
                int y = cell.y + j;
                // 判断是否越界且不是墙，如果没有，加到列表中
                if (x < dataTex.width && x >= 0 && y < dataTex.height && y >= 0 && isWalkable(x, y))
                {
                    list.Add(new Cell(x, y, false));
                }
            }
        }
        return list;
    }
    private void OnGUI()
    {
        DrawDebug();
        DrawButton();
    }

    //显示按钮
    void DrawButton()
    {

        if (GUI.Button(new Rect(0, 0, 100, 100), "Find Example"))
        {
            if (TestFind(new Vector2(50, 50), new Vector2(512, 512), lstpath))
            {
                Debug.Log("find success");
            }
            else
            {
                Debug.Log("find failed");
            }
        }

        
        if (GUI.Button(new Rect(0, 100, 100, 100), "Find"))
        {
            if (Find(new Vector2(840, 300), new Vector2(877, 144), lstpath))    //(877,880)    if (Find(new Vector2(100, 65), new Vector2(114, 65), lstpath))                    
                // new Vector2(840, 300), new Vector2(877, 144)
            {
                Debug.Log("find success");
            }
            else
            {
                Debug.Log("find failed");
            }
        }

        if (GUI.Button(new Rect(0, 200, 100, 100), "FindCPP"))
        {
            if (!isInitCPP)
            {
                InitCPP(data_width, data_width, data, data.Length);
                isInitCPP = true;
            }
            Vector2[] allPath = new Vector2[1024];
            int path_len = allPath.Length;
            if (FindCPP(new Vector2(50, 50), new Vector2(322, 536), allPath, ref path_len))
            {
                lstpath.Clear();
                for (int i = 0; i < path_len; i++)
                {
                    lstpath.Add(allPath[i]);
                }
                Debug.Log("find success");
            }
            else
            {
                lstpath.Clear();
                Debug.Log("find failed");
            }
        }
    }
    //显示地图和路径
    void DrawDebug(float scale = 0.5f)
    {
        GUI.DrawTexture(new Rect(0, 0, dataTex.width * scale, dataTex.height * scale), dataTex);
        for (int i = 1; i < lstpath.Count; i++)
        {
            Vector2 last = lstpath[i - 1];
            Vector2 next = lstpath[i];
            float length = (next - last).magnitude;
            if (length > 4.0f)
            {
                int count = (int)(length * 0.25f);
                Vector2 step = (next - last) / count;
                for (int j = 0; j < count; j++)
                {
                    Vector2 current = last + step * j;
                    GUI.DrawTexture(new Rect(current.x * scale, (1024 - current.y) * scale, 1, 1), Texture2D.whiteTexture);
                }
            }
            else
            {
                GUI.DrawTexture(new Rect(next.x * scale, (1024 - next.y) * scale, 1, 1), Texture2D.whiteTexture);
            }
        }
    }



}
