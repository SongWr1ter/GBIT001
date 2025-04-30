
using System.Collections.Generic;
using UnityEngine;

public class AStarGridObject
{
    private int x;
    private int y;
    private Grid<AStarGridObject> grid;
    public float fCost => gCost+hCost;
    public float gCost;//从起点到当前节点的实际代价
    public float hCost;//当前节点到目标节点的代价的启发式估计（采用平方根距离
    public AStarGridObject parent;
    public enum Walkable
    {
        walkable,
        obstacle
    }
    private Walkable type;
    //usage:grid[y,x][row,col],数组xy（行列）和坐标xy（横纵）是反着来的，我们按数组的来
    public AStarGridObject(Grid<AStarGridObject> grid, int row, int col)
    {
        this.grid = grid;
        this.x = col;
        this.y = row;
        type = Walkable.walkable;
    }

    public void ChangeValue(Walkable value)
    {
        this.type = value;
        grid.TriggerGridValueChanged(x,y);
    }

    public override string ToString()
    {
        return type.ToString();
    }
    
    public Walkable WalkableType() {return type;}
    public Vector2 GetGridPosition() {return new Vector2(x,y);}
    public int GetGridX() {return x;}
    public int GetGridY() {return y;}
}

public class AStar
{
    private Grid<AStarGridObject> grid;
    [Header("AStar Grid")]
    private Vector2 mapSize;
    private float mapGridSize;
    [Tooltip("改变Grid的大小不会改变地图大小，而是改变Grid的稠密程度")]
    private Transform startObjectTrans;//产生AStar网格游戏对象的坐标
    
    
    private Vector3 startPos;
    private Vector3 leftDown;
    private Vector3 rightUp;
    private int rowCount;
    private int colCount;
    private int nodeCount;
    private int Node_Start_Strategy = 2;
    private Vector3 nodeStartPos;
    public void Init()
    {
        leftDown = startObjectTrans.position + new Vector3(-mapSize.x / 2.0f, -mapSize.y / 2.0f, 0);
        rightUp = startObjectTrans.position + new Vector3(mapSize.x / 2.0f, mapSize.y / 2.0f, 0);
        rowCount = Mathf.RoundToInt(mapSize.y / mapGridSize);
        colCount = Mathf.RoundToInt(mapSize.x / mapGridSize);
        nodeCount = rowCount * colCount;
        
        if (Node_Start_Strategy == 1)
        {
            nodeStartPos = leftDown;
        }
        else
        {
            float actualLength = mapSize.y - rowCount * mapGridSize;
            float actualWidth = mapSize.x - colCount * mapGridSize;
            nodeStartPos = leftDown + new Vector3(actualLength / 2.0f, actualWidth / 2.0f, 0);
        }
        
        grid = new Grid<AStarGridObject>(rowCount,colCount,mapGridSize,startObjectTrans.position,((grid1, row, col) =>
        {
            return new AStarGridObject(grid1,row,col);
        }));
    }
    
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        AStarGridObject startNode = grid.GetGridObjectByWorldPosition(startPos);
        AStarGridObject targetNode = grid.GetGridObjectByWorldPosition(startPos);

        List<AStarGridObject> openSet = new List<AStarGridObject>(); // 开放列表
        HashSet<AStarGridObject> closedSet = new HashSet<AStarGridObject>(); // 关闭列表
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // 找到fCost最小的节点
            AStarGridObject currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            //更改列表
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            //程序出口
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }
            //对c的邻居n做如下操作
            foreach (AStarGridObject neighbour in GetNeighbours(currentNode))
            {
                //如果n已经计算过or是不可行走区域，则跳过
                if (closedSet.Contains(neighbour) || neighbour.WalkableType() != AStarGridObject.Walkable.walkable)
                {
                    continue;
                }
                //计算c与n的邻接距离
                float tentativeGCost = currentNode.gCost + Distance(currentNode, neighbour);
                //如何确保它就是最短路径，如何确保它便利了所有路径
                if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
                else if (tentativeGCost >= neighbour.gCost)//c到n的代价大于开始到n的代价，说明不如原来的路径
                {
                    continue;
                }

                neighbour.parent = currentNode;
                neighbour.gCost = tentativeGCost;
                neighbour.hCost = Distance(neighbour, targetNode);
            }
        }

        return null; // 没有找到路径
    }

    private List<Vector3> RetracePath(AStarGridObject startNode, AStarGridObject targetNode)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerable<AStarGridObject> GetNeighbours(AStarGridObject currentNode)
    {
        int X = currentNode.GetGridX();int Y = currentNode.GetGridY();
        List<AStarGridObject> neighbours = new List<AStarGridObject>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
               var g = grid.GridObjectArray(X+i-1,Y+j-1);
               if(g != null) neighbours.Add(g);
            }
        }
        return neighbours;
    }

    private static float Heuristic(AStarGridObject p1, AStarGridObject p2)
    {
        return Mathf.Abs(p1.GetGridX() - p2.GetGridX()) + Mathf.Abs(p1.GetGridY() - p2.GetGridY());    
    }

    private static float Distance(AStarGridObject p1, AStarGridObject p2)
    {
        return Vector2.Distance(p1.GetGridPosition(), p2.GetGridPosition());
    }
}
