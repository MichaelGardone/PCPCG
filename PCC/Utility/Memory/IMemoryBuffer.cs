namespace PCC.Utility.Memory
{
    public interface IMemoryBuffer<T> : IEnumerable<T>
    {
        public void Add(T item);
        
        public T Get(int indx);

        public void Clear();

        public bool IsFull();

        public int Count();
        public int Size();
    }
}
