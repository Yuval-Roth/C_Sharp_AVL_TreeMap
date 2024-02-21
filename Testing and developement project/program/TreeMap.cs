using System.Collections;


    //===========================================================================
    //                                TreeMap
    //===========================================================================



    /// <summary>
    /// ======================================================<br/>
    /// This class implements a TreeMap<K,V> that that is ordered with a <typeparamref name="K"/> that is <see cref="IComparable"/><br/>
    /// This implementation is <see cref="IEnumerable{T}"/> and supports in-order enumeration over the tree.<br/><br/>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Yuval Roth</c>
    /// <br/>
    /// ===================
    /// </summary>
    public sealed class TreeMap<K, V> : IEnumerable<V> where K : IComparable
    {
        private TreeMapNode? root;

        /// <summary>
        /// Creates an empty <c>TreeMap</c>
        /// </summary>
        public TreeMap()
        {
            root = null;
        }

        ///<summary>
        /// Adds an element into the <c>TreeMap</c>.<br/><br/>
        ///</summary>
        ///<returns>A pointer to the inserted Value</returns>
        public V? Put(K key, V value)
        {
            V? output = default;

            // if tree is empty, add to the root
            if (root == null)
            {
                root = new TreeMapNode(key, value, this);
                output = root.Value;
            }
            //otherwise pass it down
            else
            {
                TreeMapNode current = root;
                while (current != null)
                { 
                    // overwrite existing data if key exists
                    if (current.Key.CompareTo(key) == 0)
                    {
                        current.Value = value;
                        break;
                    }

                    //find a place to add it
                    else if (current.Key.CompareTo(key) > 0)
                    {
                        //empty spot
                        if (current.Left == null)
                        {
                            current.Left = new TreeMapNode(key, value, this);
                            current.Left.Parent = current;
                            output = current.Left.Value;
                            current.FixHeights();
                            if (current.Parent != null) current.Parent.Balance(true);
                            break;
                        }

                        //pass it down
                        else current = current.Left;
                    }
                    else
                    {
                        //empty spot
                        if (current.Right == null)
                        {
                            current.Right = new TreeMapNode(key, value, this);
                            current.Right.Parent = current;
                            output = current.Right.Value;
                            current.FixHeights();
                            if (current.Parent != null) current.Parent.Balance(true);
                            break;
                        }

                        //pass it down
                        else current = current.Right;
                    }
                }
            }
            return output;
        }

        ///<summary>
        ///Removes the element with this key from the <c>TreeMap</c><br/><br/>
        ///<b>Throws</b> <c>KeyNotFoundException</c> if the element is not in the <c>TreeMap</c>
        ///</summary>
        ///<returns>The removed node's element data</returns>
        ///<exception cref="KeyNotFoundException"></exception>
        public V Remove(K key)
        {
            try
            {
                return Search(key).Remove().Value;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
        }

        ///<summary>Check if the <c>TreeMap</c> contains an element with this key<br/><br/>
        /// </summary>
        ///<returns><c>true</c> if an element with this key exists in the tree and <c>false</c> otherwise</returns>
        public bool Contains(K key)
        {
            try
            {
                return Search(key) != null;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        ///<summary>Check if the <c>TreeMap</c> is empty</summary>
        ///<returns><c>true</c> if the tree is empty and <c>false</c> otherwise</returns>
        public bool IsEmpty()
        {
            return root == null;
        }

        /// <summary>
        /// search for an element with the specified key and get its <c>Data</c><br/><br/>
        /// <br/><br/>
        /// <b>Throws</b> <c>KeyNotFoundException</c> if there is no element<br/>
        /// with this key in the <c>TreeMap</c>
        /// </summary>
        /// <returns><c>The element's <c>Data</c></c></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public V Get(K key)
        {
            try
            {
                return Search(key).Value;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
        }

        private TreeMapNode Search(K key)
        {
            if (root == null) throw new KeyNotFoundException("Key not found");

            TreeMapNode current = root;

            while (current != null)
            {
                //check if the current node is the target
                if (current.Key.CompareTo(key) == 0)
                {
                    return current;
                }
                //binary search for it
                else if (current.Left != null && current.Key.CompareTo(key) > 0)
                {
                    current = current.Left;
                }
                else if (current.Right != null && current.Key.CompareTo(key) < 0)
                {
                    current = current.Right;
                }
            }

            //can't find it
            throw new KeyNotFoundException("Key not found");
        }

        public override string ToString()
        {
            if (root != null) return root.ToString();
            else return "EmptyTree";
        }
        public void PrintTree()
        {
            if (root != null) root.PrintTree();
            else Console.WriteLine("EmptyTree");
        }

        public IEnumerator GetEnumerator()
        {
            return new TreeMap_InOrder_Data_Enumerator(this);
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return new TreeMap_InOrder_Data_Enumerator(this);
        }


        //===========================================================================
        //                                TreeMapNode
        //===========================================================================

        private sealed class TreeMapNode
        {
            private readonly TreeMap<K, V> tree;
            private readonly K key;
            private V value;
            private TreeMapNode? left;
            private TreeMapNode? right;
            private TreeMapNode? parent;
            private int height;

            public TreeMapNode(K key, V value, TreeMap<K, V> tree)
            {
                this.tree = tree;
                left = null;
                right = null;
                parent = null;
                this.key = key;
                this.value = value;
                height = 0;
            }
            //======================================
            //            Getters / Setters
            //======================================


            public K Key => key;
            public V Value
            {
                get { return value; }
                set { this.value = value; }
            }

            public TreeMapNode? Left
            {
                get { return left; }
                set { left = value; }
            }
            public TreeMapNode? Right
            {
                get { return right; }
                set { right = value; }
            }
            public TreeMapNode? Parent
            {
                get { return parent; }
                set { parent = value; }
            }
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            //======================================
            //            Functionality
            //======================================

            ///<summary>
            ///Removes a node from the <c>TreeMap</c><br/><br/>
            ///
            ///<b>Warning:</b> Only works on a node that is in a tree.<br/>
            ///can throw unexpected exceptions if misused
            ///</summary>
            ///<returns>The removed TreeMapNode</returns>
            public TreeMapNode Remove()
            {
                TreeMapNode successor = null;
                // case 1: node has no children
                if (left == null & right == null)
                {
                    if (ThisNodeIsALeftSon())
                    {
                        parent.left = null;
                    }
                    else if (ThisNodeIsARightSon())
                    {
                        parent.right = null;
                    }
                    else tree.root = null;
                    if (parent != null) parent.FixHeights();
                }

                // case 2: node only has a right child
                else if (left == null)
                {
                    if (ThisNodeIsALeftSon())
                    {
                        parent.left = right;
                    }
                    else if (ThisNodeIsARightSon())
                    {
                        parent.right = right;
                    }
                    else tree.root = right;
                    right.parent = parent;
                    right.FixHeights();
                }

                // case 3: node only has a left child
                else if (right == null)
                {
                    if (ThisNodeIsALeftSon())
                    {
                        parent.left = left;
                    }
                    else if (ThisNodeIsARightSon())
                    {
                        parent.right = left;
                    }
                    else tree.root = left;
                    left.parent = parent;
                    left.FixHeights();
                }

                // case 4: node has 2 children
                else
                {

                    successor = Successor().Remove();

                    //parent
                    successor.parent = parent;
                    if (ThisNodeIsALeftSon()) parent.left = successor;
                    else if (ThisNodeIsARightSon()) parent.right = successor;

                    //left child
                    successor.left = left;
                    if (left != null) left.parent = successor;

                    //right child
                    successor.right = right;
                    if (right != null) right.parent = successor;

                    if (this == tree.root) tree.root = successor;
                    successor.FixHeights();

                }
                if (tree.root != null)
                {
                    TreeMapNode current = this;
                    if (successor != null)
                        current = successor;
                    else current = current.parent;
                    while (current != null)
                    {
                        current.Balance(false);
                        current = current.parent;
                    }
                }
                return this;
            }

            public void FixHeights()
            {
                TreeMapNode current = this;
                while (current != null)
                {
                    if (current.left == null & current.right == null) current.height = 0;
                    else
                    {
                        int leftHeight = -1;
                        int rightHeight = -1;
                        if (current.left != null) leftHeight = current.left.Height;
                        if (current.right != null) rightHeight = current.right.Height;
                        if (leftHeight >= rightHeight) current.height = leftHeight + 1;
                        else current.height = rightHeight + 1;
                    }
                    current = current.parent;
                }
            }
            /// <summary>
            /// Find the successor of a node <br/><br/>
            /// </summary>
            /// <returns>TreeMapNode if there is a successor or <b>null</b> otherwise</returns>
            public TreeMapNode Successor()
            {
                // if there is a right child
                // the minimum of the right subtree is the successor
                if (right != null)
                {
                    return right.Minimum();
                }

                // if the node is a left son the parent is the successor
                else if (ThisNodeIsALeftSon())
                {
                    return parent;
                }

                // the first bigger ancestor is the successor
                // if there is no bigger ancestor, return null
                else
                {
                    TreeMapNode current = parent;
                    while (current != null && key.CompareTo(current.key) > 0)
                    {
                        current = current.parent;
                    }
                    return current;
                }
            }

            /// <summary>
            /// Find the minimum in the <c>TreeMap</c>
            /// </summary>
            /// <returns>TreeMapNode</returns>
            public TreeMapNode Minimum()
            {

                // go left until there is more left to go
                TreeMapNode current = this;
                while (current.left != null)
                {
                    current = current.left;
                }
                return current;
            }
            private bool ThisNodeIsARightSon()
            {
                if (parent != null) return parent.right == this;

                return false;
            }
            private bool ThisNodeIsALeftSon()
            {
                if (parent != null) return parent.left == this;

                return false;
            }

            /// <summary>
            /// Balances the node.<br/><br/>
            /// <paramref name="Find_First_Unbalanced_Node"/>:<br/>
            /// <b>true:</b> Balances the current node or the first unbalanced ancestor it finds.<br/>
            /// <b>false:</b> if the current node doesn't need balancing,<br/> it won't search for the first unbalanced ancestor.<br/>
            /// </summary>
            /// <returns>true if any balancing was done, false otherwise</returns>
            public bool Balance(bool Find_First_Unbalanced_Node)
            {
                int leftHeight = -1;
                int rightHeight = -1;
                if (left != null) leftHeight = left.Height;
                if (right != null) rightHeight = right.Height;

                if (Math.Abs(leftHeight - rightHeight) > 1 & height != 1)
                {
                    if (leftHeight > rightHeight)
                    {
                        int leftLeftHeight = -1;
                        int leftRightHeight = -1;
                        if (left.left != null) leftLeftHeight = left.left.Height;
                        if (left.right != null) leftRightHeight = left.right.Height;
                        if (leftLeftHeight > leftRightHeight) LeftLeftRotation();
                        else LeftRightRotation();
                    }
                    else
                    {
                        int rightLeftHeight = -1;
                        int rightRightHeight = -1;
                        if (right.left != null) rightLeftHeight = right.left.Height;
                        if (right.right != null) rightRightHeight = right.right.Height;
                        if (rightRightHeight > rightLeftHeight) RightRightRotation();
                        else RightLeftRotation();
                    }
                    FixHeights();
                    return true;
                }
                else
                {
                    if (parent != null & Find_First_Unbalanced_Node) return parent.Balance(true);
                    else return false;
                }
            }
            private void LeftLeftRotation()
            {
                RightRotate();
            }
            private void LeftRightRotation()
            {
                left.LeftRotate();
                RightRotate();
            }
            private void RightRightRotation()
            {
                LeftRotate();
            }
            private void RightLeftRotation()
            {
                right.RightRotate();
                LeftRotate();
            }
            private void RightRotate()
            {
                TreeMapNode leftRightChild = left.right;
                left.right = this;
                left.parent = parent;
                if (ThisNodeIsALeftSon()) parent.left = left;
                else if (ThisNodeIsARightSon()) parent.right = left;
                else tree.root = left;
                parent = left;
                left = leftRightChild;
                if (leftRightChild != null) leftRightChild.parent = this;
                if (parent.left != null) parent.left.FixHeights();
            }
            private void LeftRotate()
            {
                TreeMapNode rightLeftChild = right.left;
                right.left = this;
                right.parent = parent;
                if (ThisNodeIsALeftSon()) parent.left = right;
                else if (ThisNodeIsARightSon()) parent.right = right;
                else tree.root = right;
                parent = right;
                right = rightLeftChild;
                if (rightLeftChild != null) rightLeftChild.parent = this;
                if (parent.right != null) parent.right.FixHeights();
            }
            public override string ToString()
            {
                return ToString("  ", "");
            }
            private string ToString(string spaces, string output)
            {
                if (right != null) output = right.ToString(spaces + "        ", output);

                if (parent != null) output += spaces + Key.ToString() + "(" + parent.key.ToString() + ")" + "\n";
                else output += spaces + Key.ToString() + "(root)" + "\n";

                if (left != null) output = left.ToString(spaces + "        ", output);
                return output;
            }
            public void PrintTree()
            {
                PrintTree("  ");
            }
            private void PrintTree(string spaces)
            {
                if (right != null) right.PrintTree(spaces + "        ");

                if (parent != null) Console.WriteLine(spaces + Key.ToString() + "(" + parent.key.ToString() + ")");
                else Console.WriteLine(spaces + Key.ToString() + "(root)");

                if (left != null) left.PrintTree(spaces + "        ");
            }
        }
        public class TreeMap_InOrder_Data_Enumerator : IEnumerator<V>
        {
            TreeMapNode initialPosition;
            TreeMapNode? current;
            TreeMapNode? next;
            public TreeMap_InOrder_Data_Enumerator(TreeMap<K, V> tree)
            {
                if (tree.IsEmpty() == false)
                {
                    initialPosition = tree.root.Minimum();
                    PrepareNext();
                }
            }

            V IEnumerator<V>.Current => current.Value;

            public object Current => current.Value;

            public bool MoveNext()
            {
                if (next == null) return false;

                current = next;
                PrepareNext();
                return true;
            }
            private void PrepareNext()
            {
                if (current == null) next = initialPosition;
                else next = current.Successor();
            }
            public void Reset()
            {
                current = null;
            }

            public void Dispose() { }
        }

    }
