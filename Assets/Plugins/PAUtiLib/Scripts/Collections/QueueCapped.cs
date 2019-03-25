// Author(s): Paul Calande
// A queue that has an upper limit on how many items can be enqueued.
// Clearing this queue essentially resets it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueCapped<T> : IEnumerable<T>
{
    // The underlying collection.
    Queue<T> collection;
    // The maximum number of items that can be enqueued.
    int capacity;
    // The current number of items that have been enqueued.
    int amountEnqueued = 0;

    // Constructs a capped queue with the given maximum capacity.
    public QueueCapped(int capacity)
    {
        this.capacity = capacity;
        // By specifying the underlying collection's default capacity,
        // we won't have to expand the actual queue at runtime.
        collection = new Queue<T>(capacity);
    }

    // Returns true if the queue's enqueued count has reached capacity.
    public bool IsAtCapacity()
    {
        return amountEnqueued == capacity;
    }

    // Tries to enqueue an element.
    // Returns true if the element was enqueued successfully.
    public bool Enqueue(T element)
    {
        if (IsAtCapacity())
        {
            return false;
        }
        else
        {
            collection.Enqueue(element);
            ++amountEnqueued;
            return true;
        }
    }

    // Dequeues an element.
    // Does not decrease the enqueued count.
    public T Dequeue()
    {
        return collection.Dequeue();
    }

    // Clears the collection.
    // This also resets the enqueued count,
    // allowing new enqueues to occur.
    public void Clear()
    {
        collection.Clear();
        amountEnqueued = 0;
    }

    // Returns true if the queue is out of elements.
    public bool IsEmpty()
    {
        return collection.Count == 0;
    }

    // Returns the number of elements currently in the collection.
    public int GetCount()
    {
        return collection.Count;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}