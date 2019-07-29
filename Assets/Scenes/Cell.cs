using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scenes
{
    public class Cell
    {
        public int x;
        public int y;
        public bool isWall;

        // 与起点的长度
        public int gCost;
        // 与目标点的长度
        public int hCost;
        // 总的路径长度
        public int fCost
        {
            get { return gCost + hCost; }
        }

        // 父节点
        public Cell parent = null;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Cell(int x, int y, bool isWall)
        {
            this.x = x;
            this.y = y;
            this.isWall = isWall;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        public static bool operator ==(Cell left, Cell right)
        {
            if ((left as object) == null)
            {
                return (right as object) == null;
            }
            if ((right as object) == null)
                return false;
            if (left.x == right.x && left.y == right.y)
                return true;
            return false;
        }
        public static bool operator !=(Cell left, Cell right)
        {
            if((left as object) == null)
            {
                return !((right as object) == null);
            }
            if ((right as object) == null)
                return true;
            if (left.x != right.x || left.y != right.y)
                return true;
            return false;
        }
    }
}
