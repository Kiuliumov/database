using System;
namespace Database.Shared.DataTypes;

class Node
{
    public string Key;
    public object Value;
    public Node Next;

    public Node(string key, object value)
    {
        Key = key;
        Value = value;
        Next = null;
    }
}
class HashTable
{
    private const int SIZE = 10000;

    private Node[] _buckets;


    public HashTable()
    {
        _buckets = new Node[SIZE];
    }


    public void Add(string key, string value)
    {
        int index = GetAddress(key);
        Node newNode = new Node(key, value);

        if (_buckets[index] == null)
        {
            _buckets[index] = newNode;
        }
        else
        {
            Node current = _buckets[index];
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
    }

    public void Remove(string key)
    {
        int index = GetAddress(key);
        Node current = _buckets[index];
        Node previous = null;

        while (current != null)
        {
            if (current.Key == key)
            {
                if (previous == null)
                {
                    _buckets[index] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
                return;
            }
            previous = current;
            current = current.Next;
        }
    }
    
    public string Get(string key)
    {
        int address = GetAddress(key);
        Node curr = _buckets[address];
        while (curr != null && curr.Key != key)
        {
            curr = curr.Next;
        }
        return curr != null ? curr.Value : null; 
    }

    public int GetAddress(string key)
    {
        int hash = 0;
        foreach (char c in key)
        {
            hash += c;
        }
        return hash % SIZE;
    }


    static string ToString(HashTable hashTable)
    {
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < SIZE; i++)
        {
            Node current = hashTable._buckets[i];
            while (current != null)
            {
                result.AppendLine($"Key: {current.Key}, Value: {current.Value}");
                current = current.Next;
            }
        }
        return result.ToString();
    }
}