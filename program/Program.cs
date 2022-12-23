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

            int count = 1000;


            //recursive(count);

            iterative(count);
        }

        private static void iterative(int count)
        {
            Random rand = new();
            DateTime start = DateTime.Now;
            AVLTree_Iterative<int, int> tree2 = new();
            for (int i = 0; i < count; i++)
            {
                tree2.Add(i, rand.Next(-count, count));
            }
            for (int i = 0; i < count; i++)
            {
                tree2.Remove(i);
            }
            Console.WriteLine("iterative finished after " + (DateTime.Now - start).TotalMilliseconds + " ms");
        }

        static void recursive(int count)
        {
            Random rand = new();
            DateTime start = DateTime.Now;
            AVLTree<int, int> tree = new();
            for (int i = 0; i < count; i++)
            {
                tree.Add(i, rand.Next(-count, count));
            }
            for (int i = 0; i < count; i++)
            {
                tree.Remove(i);
            }
            Console.WriteLine("recursive finished after " + (DateTime.Now - start).TotalMilliseconds + " ms");
        }
    }
}
