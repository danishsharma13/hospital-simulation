using System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        SplayTree<int> tree = new SplayTree<int>();

        while (true)
        {
            Console.WriteLine("1. Insert");
            Console.WriteLine("2. Remove");
            Console.WriteLine("3. Check if contains");
            Console.WriteLine("4. Print tree");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter value to insert: ");
                        if (int.TryParse(Console.ReadLine(), out int insertValue))
                        {
                            tree.Insert(insertValue);
                            Console.WriteLine($"Value {insertValue} inserted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid integer.");
                        }
                        break;

                    case 2:
                        Console.Write("Enter value to remove: ");
                        if (int.TryParse(Console.ReadLine(), out int removeValue))
                        {
                            bool removed = tree.Remove(removeValue);
                            Console.WriteLine(removed
                                ? $"Value {removeValue} removed successfully."
                                : $"Value {removeValue} not found in the tree.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid integer.");
                        }
                        break;

                    case 3:
                        Console.Write("Enter value to check if it contains: ");
                        if (int.TryParse(Console.ReadLine(), out int containsValue))
                        {
                            bool contains = tree.Contains(containsValue);
                            Console.WriteLine($"Tree {(contains ? "contains" : "does not contain")} the value.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a valid integer.");
                        }
                        break;

                    case 4:
                        Console.WriteLine("Tree structure (largest to smallest):");
                        tree.PrintTree();
                        break;

                    case 5:
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 5.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }

            Console.WriteLine();
        }
    }
}

public class SplayTree<T> where T : IComparable<T>
{
    private class Node
    {
        public T Value;
        public Node Left, Right;

        public Node(T value)
        {
            Value = value;
            Left = Right = null;
        }
    }

    private Node Root;

    public void Insert(T value)
    {
        Root = InsertRecursive(Root, value);
    }

    private Node InsertRecursive(Node root, T value)
    {
        if (root == null)
            return new Node(value);

        int compareResult = value.CompareTo(root.Value);

        if (compareResult < 0)
        {
            root.Left = InsertRecursive(root.Left, value);
        }
        else if (compareResult > 0)
        {
            root.Right = InsertRecursive(root.Right, value);
        }

        // Splay the inserted node to the root
        return Splay(root, value);
    }

    public bool Remove(T value)
    {
        Root = RemoveRecursive(Root, value);
        return Root != null; // Node found and removed
    }

    private Node RemoveRecursive(Node root, T value)
    {
        if (root == null)
            return null; // Node not found

        int compareResult = value.CompareTo(root.Value);

        if (compareResult < 0)
        {
            root.Left = RemoveRecursive(root.Left, value);
        }
        else if (compareResult > 0)
        {
            root.Right = RemoveRecursive(root.Right, value);
        }
        else
        {
            // Node with the value found, splay the predecessor to the root
            root = SplayPredecessor(root);
        }

        return root;
    }

    private Node SplayPredecessor(Node root)
    {
        if (root.Left == null)
            return root.Right;

        Node predecessor = FindMax(root.Left);
        root.Left = RemoveMax(root.Left);
        predecessor.Left = root.Left;
        predecessor.Right = root.Right;

        return predecessor;
    }

    private Node FindMax(Node root)
    {
        while (root.Right != null)
        {
            root = root.Right;
        }
        return root;
    }

    private Node RemoveMax(Node root)
    {
        if (root.Right == null)
            return root.Left;

        root.Right = RemoveMax(root.Right);
        return root;
    }

    public bool Contains(T value)
    {
        Root = ContainsRecursive(Root, value);
        return Root != null; // Node found
    }

    private Node ContainsRecursive(Node root, T value)
    {
        if (root == null)
            return null; // Node not found

        int compareResult = value.CompareTo(root.Value);

        if (compareResult < 0)
        {
            root.Left = ContainsRecursive(root.Left, value);
        }
        else if (compareResult > 0)
        {
            root.Right = ContainsRecursive(root.Right, value);
        }

        // Splay the accessed node to the root
        return Splay(root, value);
    }

    public void PrintTree()
    {
        PrintTreeRecursive(Root, 0);
    }

    private void PrintTreeRecursive(Node root, int indent)
    {
        if (root != null)
        {
            PrintTreeRecursive(root.Right, indent + 4);
            Console.Write(new string(' ', indent));
            Console.WriteLine(root.Value);
            PrintTreeRecursive(root.Left, indent + 4);
        }
    }

    private Node Splay(Node root, T value)
    {
        if (root == null)
            return null; // Node not found

        int compareResult = value.CompareTo(root.Value);

        if (compareResult < 0)
        {
            if (root.Left == null)
                return root;

            int compareResult2 = value.CompareTo(root.Left.Value);

            if (compareResult2 < 0)
            {
                root.Left.Left = Splay(root.Left.Left, value);
                root = RotateRight(root);
            }
            else if (compareResult2 > 0)
            {
                root.Left.Right = Splay(root.Left.Right, value);
                if (root.Left.Right != null)
                    root.Left = RotateLeft(root.Left);
            }

            return (root.Left == null) ? root : RotateRight(root);
        }
        else if (compareResult > 0)
        {
            if (root.Right == null)
                return root;

            int compareResult2 = value.CompareTo(root.Right.Value);

            if (compareResult2 < 0)
            {
                root.Right.Left = Splay(root.Right.Left, value);
                if (root.Right.Left != null)
                    root.Right = RotateRight(root.Right);
            }
            else if (compareResult2 > 0)
            {
                root.Right.Right = Splay(root.Right.Right, value);
                root = RotateLeft(root);
            }

            return (root.Right == null) ? root : RotateLeft(root);
        }

        return root;
    }

    private Node RotateRight(Node root)
    {
        Node newRoot = root.Left;
        root.Left = newRoot.Right;
        newRoot.Right = root;
        return newRoot;
    }

    private Node RotateLeft(Node root)
    {
        Node newRoot = root.Right;
        root.Right = newRoot.Left;
        newRoot.Left = root;
        return newRoot;
    }
}
