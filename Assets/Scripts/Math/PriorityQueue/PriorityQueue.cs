using System.Collections.Generic;
using System;

public class PriorityQueue<T>
{
    List<Tuple<T, double>> elements = new List<Tuple<T, double>>();
    bool isMaxPriority;

    public PriorityQueue(bool isMaxPriority = false)
    {
        this.isMaxPriority = isMaxPriority;
    }

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, double priorityValue)
    {
        elements.Add(Tuple.Create(item, priorityValue));
    }

    public T Dequeue()
    {
        if (elements.Count == 0)
        {
            throw new InvalidOperationException("Cannot dequeue an empty queue.");
        }

        int bestPriorityIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (isMaxPriority)
            {
                if (elements[i].Item2 > elements[bestPriorityIndex].Item2)
                {
                    bestPriorityIndex = i;
                }
            }
            else
            {
                if (elements[i].Item2 < elements[bestPriorityIndex].Item2)
                {
                    bestPriorityIndex = i;
                }
            }
        }

        T bestItem = elements[bestPriorityIndex].Item1;
        elements.RemoveAt(bestPriorityIndex);
        return bestItem;
    }

    public T Peek()
    {
        if (elements.Count == 0)
        {
            throw new InvalidOperationException("Cannot peek an empty queue.");
        }

        int bestPriorityIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (isMaxPriority)
            {
                if (elements[i].Item2 > elements[bestPriorityIndex].Item2)
                {
                    bestPriorityIndex = i;
                }
            }
            else
            {
                if (elements[i].Item2 < elements[bestPriorityIndex].Item2)
                {
                    bestPriorityIndex = i;
                }
            }
        }

        T bestItem = elements[bestPriorityIndex].Item1;
        return bestItem;
    }

    public bool IsEmpty()
    {
        return elements.Count == 0;
    }
}