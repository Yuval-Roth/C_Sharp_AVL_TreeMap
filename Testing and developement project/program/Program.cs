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

            TreeMap<int, int> tree = new();
            tree.Put(1, 1);
            tree.Put(2, 2);
            tree.Put(3, 3);
            tree.Put(4, 4);
            tree.Put(5, 5);
            tree.Put(6, 6);
            tree.Put(2, 20);
            tree.Put(3, 30);
            tree.Put(5, 50);


            tree.PrintTree();

            foreach (int curr in tree)
            {
                Console.WriteLine(curr);
            }
            for (int i = 1; i <= 6; i++)
            {
                tree.Remove(i);
                tree.PrintTree();
                Console.WriteLine("==================");
            }
        }
        
    }
}
