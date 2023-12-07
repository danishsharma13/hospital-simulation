// Import necessary libraries
using System;
using System.Collections.Generic;

// Define the main program class
public class Program
{
    // Entry point of the program
    public static void Main(string[] args)
    {
        // Create an instance of SplayTree
        SplayTree<int> tree = new SplayTree<int>();

        // Display menu options and handle user input
        while (true)
        {
            Console.WriteLine("1. Insert");
            Console.WriteLine("2. Remove");
            Console.WriteLine("3. Check if contains");
            Console.WriteLine("4. Undo");
            Console.WriteLine("5. Print tree");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");

            // Validate and process user input
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        // Insert operation
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
                        // Remove operation
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
                        // Check containment operation
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
                        // Undo operation
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
                        // Print tree operation
                        Console.WriteLine("Tree structure:");
                        tree.PrintTree();
                        break;

                    case 6:
                        // Exit the program
                        Console.WriteLine("Exiting program.");
                        return;

                    default:
                        // Invalid choice
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 6.");
                        break;
                }
            }
            else
            {
                // Invalid input
                Console.WriteLine("Invalid input. Please enter a number.");
            }

            Console.WriteLine();
        }
    }
}

// Define a generic SplayTree class
public class SplayTree<T> where T : IComparable<T>
{
    // Define a Node class for the Splay Tree
    public class Node<U>
    {
        public U item = default!;
        public Node<U>? left, right;
    }

    // Private members of the SplayTree class
    private Node<T>? root = null;
    private Node<T>? previousRoot = null;  // Track the previous root for undo operation

    // Public property to access the root of the tree
    public Node<T>? Root => root;

    // Constructor to initialize the SplayTree with a root node
    public SplayTree()
    {
        root = new Node<T>();
    }

    // Helper method to access a node and track the path
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

    // Helper method to perform the Splay operation
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

    // Method to insert a value into the SplayTree
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

    // Method to remove a value from the SplayTree
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

    // Method to check if a value is contained in the SplayTree
    public bool Contains(T item)
    {
        Stack<T> path = Access(item);
        return (path.Count > 0 && item.CompareTo(path.Peek()) == 0);
    }

    // Method to create a clone of the SplayTree
    public object Clone()
    {
        SplayTree<T> clonedTree = new SplayTree<T>();
        CloneHelper(root, ref clonedTree.root);
        return clonedTree;
    }

    // Helper method for cloning the SplayTree
    private void CloneHelper(Node<T>? original, ref Node<T>? cloned)
    {if (original != null)
        {
            cloned = new Node<T>() { item = original.item };
            CloneHelper(original.left, ref cloned.left);
            CloneHelper(original.right, ref cloned.right);
        }
    }

    // Method to compare two SplayTrees for equality
    public override bool Equals(object? obj)
    {
        if (obj is SplayTree<T> otherTree)
        {
            return EqualsHelper(root, otherTree.root);
        }
        return false;
    }

    // Helper method for comparing SplayTrees for equality
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

    // Method to undo the last insertion operation
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

    // Method to print the structure of the SplayTree
    public void PrintTree()
    {
        PrintTree(root, 0);
    }

    // Helper method for printing the structure of the SplayTree
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

    // Method to calculate the hash code for the SplayTree
    public override int GetHashCode()
    {
        // Implement a hash code calculation based on the properties of your class.
        // For simplicity, you can use the hash code of the root node's item.
        return root?.item?.GetHashCode() ?? 0;
    }
}