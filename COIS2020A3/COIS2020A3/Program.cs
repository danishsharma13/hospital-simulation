using System;
using System.Collections.Generic;

public class SplayTree<T> : ICloneable
{
    private class Node
    {
        public T Data;
        public Node Left, Right;

        public Node(T data)
        {
            Data = data;
            Left = Right = null;
        }
    }

    private Node root;

    public void Insert(T item)
    {
        if (root == null)
        {
            root = new Node(item);
            return;
        }
    }

    public bool Remove(T item)
    {
        if (root == null)
        {
            // Tree is empty, item not found
            return false;
        }

        // Step 1: Perform regular binary search to find the node containing the item
        Splay(null, Access(item));

        if (root.Data.Equals(item))
        {
            // Item found at the root, remove it
            if (root.Left == null)
            {
                root = root.Right;
            }
            else
            {
                // Find the maximum item in the left subtree
                Node maxLeft = FindMax(root.Left);
                // Splay the maxLeft node to the root
                Splay(maxLeft, Access(item));

                // Attach the right subtree to the right of the left subtree
                root.Left.Right = root.Right;
                root = root.Left;
            }

            return true; // Item removed
        }

        return false; // Item not found
    }

    private Node FindMax(Node node)
    {
        while (node.Right != null)
        {
            node = node.Right;
        }

        return node;
    }

    public bool Contains(T item)
    {
        if (root == null)
        {
            // Tree is empty, item not found
            return false;
        }

        // Step 1: Perform regular binary search to find the node containing the item
        Splay(null, Access(item));

        // Step 2: Check if the item is now at the root
        return root.Data.Equals(item);
    }

    private Stack<Node> Access(T item)
    {
        Stack<Node> accessPath = new Stack<Node>();
        Node current = root;

        while (current != null)
        {
            accessPath.Push(current);

            int compareResult = Comparer<T>.Default.Compare(item, current.Data);

            if (compareResult == 0)
            {
                // Item found, break out of the loop
                break;
            }
            else if (compareResult < 0)
            {
                // Item is smaller, move to the left subtree
                current = current.Left;
            }
            else
            {
                // Item is larger, move to the right subtree
                current = current.Right;
            }
        }

        return accessPath;
    }

    private void Splay(Node p, Stack<Node> S)
    {
        while (S.Count > 0 && S.Peek() != p)
        {
            Node grandparent = S.Pop();
            Node parent = S.Pop();

            if (p == parent.Left)
            {
                if (grandparent.Left == parent)
                {
                    // Zig-Zig (Left-Left)
                    RotateRight(grandparent);
                    RotateRight(parent);
                }
                else
                {
                    // Zig-Zag (Right-Left)
                    RotateRight(parent);
                    RotateLeft(grandparent);
                }
            }
            else
            {
                if (grandparent.Right == parent)
                {
                    // Zig-Zig (Right-Right)
                    RotateLeft(grandparent);
                    RotateLeft(parent);
                }
                else
                {
                    // Zig-Zag (Left-Right)
                    RotateLeft(parent);
                    RotateRight(grandparent);
                }
            }

            // Push the grandparent back onto the stack for the next iteration
            S.Push(grandparent);
        }

        // If p is still not at the root, perform a final rotation
        if (S.Count == 0 && p != root)
        {
            if (p == root.Left)
            {
                // Zig (Left)
                RotateRight(root);
            }
            else
            {
                // Zag (Right)
                RotateLeft(root);
            }
        }
    }

    private void RotateLeft(Node x)
    {
        Node y = x.Right;
        x.Right = y.Left;
        y.Left = x;
        x = y;
    }

    private void RotateRight(Node y)
    {
        Node x = y.Left;
        y.Left = x.Right;
        x.Right = y;
        y = x;
    }

    public object Clone()
    {
        if (root == null)
        {
            return null; // Return null for an empty tree
        }

        SplayTree<T> clonedTree = new SplayTree<T>();
        clonedTree.root = CloneNode(root);

        return clonedTree;
    }

    private Node CloneNode(Node originalNode)
    {
        if (originalNode == null)
        {
            return null;
        }

        Node newNode = new Node(originalNode.Data);
        newNode.Left = CloneNode(originalNode.Left);
        newNode.Right = CloneNode(originalNode.Right);

        return newNode;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SplayTree<T> otherTree = (SplayTree<T>)obj;
        return Equals(root, otherTree.root);
    }

    private bool Equals(Node node1, Node node2)
    {
        if (node1 == null && node2 == null)
        {
            return true;
        }

        if (node1 == null || node2 == null)
        {
            return false;
        }

        return
            EqualityComparer<T>.Default.Equals(node1.Data, node2.Data) &&
            Equals(node1.Left, node2.Left) &&
            Equals(node1.Right, node2.Right);
    }

    public SplayTree<T> Undo()
    {
        if (root == null)
        {
            // Nothing to undo in an empty tree
            return null;
        }

        // Clone the current tree for each attempt to undo
        SplayTree<T> cloneTree = (SplayTree<T>)Clone();

        // Try different rotations to bring the last inserted item to a leaf position
        while (cloneTree.root.Right != null)
        {
            Node lastInsertedNode = cloneTree.FindMax(cloneTree.root.Left);
            Stack<Node> accessPath = cloneTree.Access(lastInsertedNode.Data);
            cloneTree.Splay(lastInsertedNode, accessPath);
        }

        // Remove the last inserted item at the leaf position
        cloneTree.root = cloneTree.root.Left;

        return cloneTree;
    }

    public static void Main()
    {
        // Example usage
        SplayTree<int> tree = new SplayTree<int>();

        tree.Insert(10);
        tree.Insert(5);
        tree.Insert(15);

        Console.WriteLine(tree.Contains(5));  // Output: True
        Console.WriteLine(tree.Contains(20)); // Output: False

        tree.Remove(10);

        SplayTree<int> clonedTree = (SplayTree<int>)tree.Clone();
        Console.WriteLine(clonedTree.Equals(tree)); // Output: True

        SplayTree<int> undoTree = tree.Undo();
        Console.WriteLine(undoTree.Equals(clonedTree)); // Output: True
    }
}
