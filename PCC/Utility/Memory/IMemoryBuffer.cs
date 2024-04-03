﻿namespace PCC.Utility.Memory
{
    public interface IMemoryBuffer<T>
    {
        public void Add(T item);
        
        public T Get(int indx);

        public void Replace(int indx, T item);

        public void Clear();

        public bool IsFull();

        public int Count();
        public int Size();
    }
}
