using System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        // Create an instance of SplayTree
        SplayTree<int> tree = new SplayTree<int>();

        while (true)
        {
            Console.WriteLine("1. Insert");
            Console.WriteLine("2. Remove");
            Console.WriteLine("3. Check if contains");
            Console.WriteLine("4. Undo");
            Console.WriteLine("5. Print tree");
            Console.WriteLine("6. Exit");
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
                            Console.WriteLine("Value inserted successfully.");
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
                            tree.Remove(removeValue);
                            Console.WriteLine("Value removed successfully.");
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
                        try
                        {
                            tree.Undo();
                            Console.WriteLine("Undo successful.");
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                    case 5:
                        Console.WriteLine("Tree structure:");
                        tree.PrintTree();
                        break;

                    case 6:
                        Console.WriteLine("Exiting program.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 6.");
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
    public class Node<U>
    {
        public U item = default!;
        public Node<U>? left, right;
    }

    private Node<T>? root = null;
    private Node<T>? previousRoot = null;  // Track the previous root for undo operation

    public Node<T>? Root => root;

    public SplayTree()
    {
        // Initialize the root node
        root = new Node<T>();
    }

    private Stack<T> Access(T item)
    {
        Stack<T> path = new Stack<T>();
        Node<T>? current = root;

        while (current != null)
        {
            path.Push(current.item);

            int compareResult = item.CompareTo(current.item);

            if (compareResult == 0)
            {
                break;
            }
            else if (compareResult < 0)
            {
                current = current.left;
            }
            else
            {
                current = current.right;
            }
        }

        return path;
    }

    private void Splay(Node<T>? p, Stack<T> S)
    {
        while (S.Count > 0 && p != null)
        {
            T x = S.Pop();

            int compareResult = x.CompareTo(p.item);

            if (compareResult == 0)
            {
                break;
            }
            else if (compareResult < 0)
            {
                if (p.left != null)
                {
                    Node<T>? q = p.left;
                    p.left = q.right;
                    q.right = p;
                    p = q;
                }
            }
            else
            {
                if (p.right != null)
                {
                    Node<T>? q = p.right;
                    p.right = q.left;
                    q.left = p;
                    p = q;
                }
            }
        }

        root = p;
    }

    public void Insert(T item)
    {
        if (root == null)
        {
            root = new Node<T>() { item = item };
            previousRoot = null;
            return;
        }

        previousRoot = root;  // Save the current root for potential undo
        Stack<T> path = Access(item);

        Node<T>? newNode = new Node<T>() { item = item };

        if (item.CompareTo(path.Peek()) < 0)
        {
            newNode.left = root.left;
            newNode.right = root;
            root.left = null;
        }
        else
        {
            newNode.right = root.right;
            newNode.left = root;
            root.right = null;
        }

        root = newNode;
    }

    public void Remove(T item)
    {
        Stack<T> path = Access(item);

        if (item.CompareTo(path.Peek()) != 0)
        {
            return; // item not found
        }

        Node<T>? leftSubtree = root?.left;
        Node<T>? rightSubtree = root?.right;

        if (leftSubtree != null)
        {
            Node<T>? newRoot = leftSubtree;
            root = leftSubtree;

            Access(item); // move item to the root of the left subtree

            root.right = rightSubtree; // attach the right subtree
        }
        else
        {
            root = rightSubtree; // no left subtree, simply move right subtree up
        }
    }

    public bool Contains(T item)
    {
        Stack<T> path = Access(item);
        return (path.Count > 0 && item.CompareTo(path.Peek()) == 0);
    }

    public object Clone()
    {
        SplayTree<T> clonedTree = new SplayTree<T>();
        CloneHelper(root, ref clonedTree.root);
        return clonedTree;
    }

    private void CloneHelper(Node<T>? original, ref Node<T>? cloned)
    {
        if (original != null)
        {
            cloned = new Node<T>() { item = original.item };
            CloneHelper(original.left, ref cloned.left);
            CloneHelper(original.right, ref cloned.right);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is SplayTree<T> otherTree)
        {
            return EqualsHelper(root, otherTree.root);
        }
        return false;
    }

    private bool EqualsHelper(Node<T>? node1, Node<T>? node2)
    {
        if (node1 == null && node2 == null)
        {
            return true;
        }

        if (node1 == null || node2 == null)
        {
            return false;
        }

        return node1.item.Equals(node2.item) &&
               EqualsHelper(node1.left, node2.left) &&
               EqualsHelper(node1.right, node2.right);
    }

    public SplayTree<T> Undo()
    {
        if (previousRoot == null)
        {
            throw new InvalidOperationException("Undo is only supported after an insertion.");
        }

        root = previousRoot;
        previousRoot = null;  // Reset previousRoot after undoing the operation

        return this;
    }

    public void PrintTree()
    {
        PrintTree(root, 0);
    }

    private void PrintTree(Node<T>? root, int indent)
    {
        if (root != null)
        {
            PrintTree(root.right, indent + 4);
            Console.Write(new string(' ', indent));
            Console.WriteLine(root.item);
            PrintTree(root.left, indent + 4);
        }
    }

    public override int GetHashCode()
    {
        // Implement a hash code calculation based on the properties of your class.
        // For simplicity, you can use the hash code of the root node's item.
        return root?.item?.GetHashCode() ?? 0;
    }
}

