using System;
using System.Collections;
using System.Collections.Generic;


namespace AVLTree.Recursive
{

    //===========================================================================
    //                                AVLTree
    //===========================================================================



    /// <summary>
    /// ======================================================<br/>
    /// This class implements a generic AVL Tree that that is ordered with a <typeparamref name="Key"/> that is <see cref="IComparable"/><br/>
    /// This implementation is <see cref="IEnumerable{T}"/> and supports in-order enumeration over the tree.<br/><br/>
    /// <b>This implementation does not support duplicate keys</b>
    /// <code>Supported operations:</code>
    /// <list type="bullet">Add()</list>
    /// <list type="bullet">Remove()</list>
    /// <list type="bullet">Contains()</list>
    /// <list type="bullet">GetData()</list>
    /// <list type="bullet">IsEmpty()</list>
    /// <br/><br/>
    /// ===================
    /// <br/>
    /// <c>Ⓒ Yuval Roth</c>
    /// <br/>
    /// ===================
    /// </summary>
    public sealed class AVLTree<Key, Data> : IEnumerable<Data> where Key : IComparable
    {
        private AVLTreeNode root;

        /// <summary>
        /// Creates an empty <c>AVLTree</c>
        /// </summary>
        public AVLTree()
        {
            root = null;
        }

        ///<summary>
        /// Adds an element into the <c>AVLTree</c>.<br/><br/>
        ///<b>throws</b> <c>DuplicateKeysNotSupported</c> if an element with the same key already exists in the tree
        ///</summary>
        ///<exception cref="DuplicateKeysNotSupported"></exception>
        ///<returns>A pointer to the inserted Data</returns>
        public Data Add(Key key, Data data)
        {
            // if tree is empty, add to the root
            if (root == null)
            {
                root = new AVLTreeNode(key, data, this);
                return root.Data;
            }
            //otherwise pass it down
            else
            {
                try
                {
                    return root.Add(key, data, this).Data;
                }
                catch (DuplicateKeysNotSupported)
                {
                    throw;
                }
            }

        }

        ///<summary>
        ///Removes the element with this key from the <c>AVLTree</c><br/><br/>
        ///<b>Throws</b> <c>KeyNotFoundException</c> if the element is not in the <c>AVLTree</c>
        ///</summary>
        ///<returns>The removed node's element data</returns>
        ///<exception cref="KeyNotFoundException"></exception>
        public Data Remove(Key key)
        {
            if (root == null) throw new KeyNotFoundException("Key not found in the tree");
            try
            {
                return root.Search(key).Remove().Data;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
        }

        ///<summary>Check if the <c>AVLTree</c> contains an element with this key<br/><br/>
        /// </summary>
        ///<returns><c>true</c> if an element with this key exists in the tree and <c>false</c> otherwise</returns>
        public bool Contains(Key key)
        {
            if (root != null) return root.Contains(key);
            else return false;
        }

        ///<summary>Check if the <c>AVLTree</c> is empty</summary>
        ///<returns><c>true</c> if the tree is empty and <c>false</c> otherwise</returns>
        public bool IsEmpty()
        {
            return root == null;
        }

        /// <summary>
        /// search for an element with the specified key and get its <c>Data</c><br/><br/>
        /// <br/><br/>
        /// <b>Throws</b> <c>KeyNotFoundException</c> if there is no element<br/>
        /// with this key in the <c>AVLTree</c>
        /// </summary>
        /// <returns><c>The element's <c>Data</c></c></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Data GetData(Key key)
        {
            try
            {
                if (root != null) return root.Search(key).Data;
                else throw new KeyNotFoundException("Key not found in the tree");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
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
            return new AVLTree_InOrder_Data_Enumerator(this);
        }

        IEnumerator<Data> IEnumerable<Data>.GetEnumerator()
        {
            return new AVLTree_InOrder_Data_Enumerator(this);
        }


        //===========================================================================
        //                                AVLTreeNode
        //===========================================================================

        private sealed class AVLTreeNode
        {
            private readonly AVLTree<Key, Data> tree;
            private readonly Key key;
            private readonly Data data;
            private AVLTreeNode left;
            private AVLTreeNode right;
            private AVLTreeNode parent;
            private int height;

            public AVLTreeNode(Key key, Data data, AVLTree<Key, Data> tree)
            {
                this.tree = tree;
                left = null;
                right = null;
                parent = null;
                this.key = key;
                this.data = data;
                height = 0;
            }
            //======================================
            //            Getters / Setters
            //======================================


            public Key Key => key;
            public Data Data => data;

            public AVLTreeNode Left
            {
                get { return left; }
                set { left = value; }
            }
            public AVLTreeNode Right
            {
                get { return right; }
                set { right = value; }
            }
            public AVLTreeNode Parent
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
            /// Adds an element into the <c>AVLTree</c>.<br/><br/>
            ///<b>throws</b> <c>DuplicateKeysNotSupported</c> if an element with the same key already exists in the tree
            ///</summary>
            ///<exception cref="DuplicateKeysNotSupported"></exception>
            public AVLTreeNode Add(Key key, Data data, AVLTree<Key, Data> tree)
            {
                //check if element already exists in the tree
                if (this.key.CompareTo(key) == 0) throw new DuplicateKeysNotSupported("Element already exists in the tree");

                //find a place to add it
                if (this.key.CompareTo(key) > 0)
                {
                    //empty spot
                    if (left == null)
                    {
                        left = new AVLTreeNode(key, data, tree)
                        {
                            Parent = this,
                        };
                        AVLTreeNode output = left;
                        FixHeights();
                        if (parent != null) parent.Balance(true);
                        return output;
                    }

                    //pass it down
                    else return left.Add(key, data, tree);
                }
                else
                {
                    //empty spot
                    if (right == null)
                    {
                        right = new AVLTreeNode(key, data, tree)
                        {
                            parent = this
                        };
                        AVLTreeNode output = right;
                        FixHeights();
                        if (parent != null) parent.Balance(true);
                        return output;
                    }

                    //pass it down
                    else return right.Add(key, data, tree);
                }
            }

            ///<summary>Check if the <c>AVLTree</c> contains a node with this key</summary>
            ///<returns><c>true</c> if the node with this key exists in the tree and <c>false</c> otherwise</returns>
            public bool Contains(Key key)
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

            /// <summary>
            /// search for a node with the specified key<br/><br/>
            /// <b>Throws</b> <c>KeyNotFoundException</c> if a node with this key does not exist in the <c>AVLTree</c>
            /// </summary>
            /// <returns>AVLTreeNode</returns>
            /// <exception cref="KeyNotFoundException"></exception>
            public AVLTreeNode Search(Key key)
            {
                //check if the current node is the target
                if (this.key.CompareTo(key) == 0)
                {
                    return this;
                }
                //binary search for it
                else if (left != null && this.key.CompareTo(key) > 0)
                {
                    return left.Search(key);
                }
                else if (right != null && this.key.CompareTo(key) < 0)
                {
                    return right.Search(key);
                }
                //can't find it
                else throw new KeyNotFoundException("Key not found in the tree");
            }

            ///<summary>
            ///Removes a node from the <c>AVLTree</c><br/><br/>
            ///
            ///<b>Warning:</b> Only works on a node that is in a tree.<br/>
            ///can throw unexpected exceptions if misused
            ///</summary>
            ///<returns>The removed AVLTreeNode</returns>
            public AVLTreeNode Remove()
            {
                AVLTreeNode successor = null;
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
                    AVLTreeNode current = this;
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

            private void FixHeights()
            {
                AVLTreeNode current = this;
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
            /// <returns>AVLTreeNode if there is a successor or <b>null</b> otherwise</returns>
            public AVLTreeNode Successor()
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
                    AVLTreeNode current = parent;
                    while (current != null && key.CompareTo(current.key) > 0)
                    {
                        current = current.parent;
                    }
                    //if (current == null) throw new KeyNotFoundException("Successor doesn't exist");
                    return current;
                }
            }

            /// <summary>
            /// Find the minimum in the <c>AVLTree</c>
            /// </summary>
            /// <returns>AVLTreeNode</returns>
            public AVLTreeNode Minimum()
            {

                // go left until there is more left to go
                AVLTreeNode current = this;
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
            private bool Balance(bool Find_First_Unbalanced_Node)
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
                AVLTreeNode leftRightChild = left.right;
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
                AVLTreeNode rightLeftChild = right.left;
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
        public class AVLTree_InOrder_Data_Enumerator : IEnumerator<Data>
        {
            AVLTreeNode initialPosition;
            AVLTreeNode current;
            AVLTreeNode next;
            public AVLTree_InOrder_Data_Enumerator(AVLTree<Key, Data> tree)
            {
                if (tree.IsEmpty() == false)
                {
                    initialPosition = tree.root.Minimum();
                    PrepareNext();
                }
            }

            Data IEnumerator<Data>.Current => current.Data;

            public object Current => current.Data;

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
    public class DuplicateKeysNotSupported : SystemException
    {
        public DuplicateKeysNotSupported() : base() { }
        public DuplicateKeysNotSupported(string message) : base(message) { }
        public DuplicateKeysNotSupported(string message, Exception innerException) : base(message, innerException) { }
    }
}
