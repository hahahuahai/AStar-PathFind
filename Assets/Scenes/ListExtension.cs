using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scenes
{
    public static class ListExtension
    {
        public static bool ContainsCell(this List<Cell> list, Cell cell)
        {
            return list.Count((c) => { return c.x == cell.x && c.y == cell.y; }) > 0;
        }
    }
}
