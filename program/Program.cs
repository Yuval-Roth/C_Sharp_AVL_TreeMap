using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVLTree
{
    internal class Program
    {
        public static void Main()
        {
            AVLTree<int, int> tree = new();
            for (int i = 0; i < 10000; i++)
            {
                tree.Add(i, i);
            }
            Console.WriteLine(tree);

        }
    }
}
