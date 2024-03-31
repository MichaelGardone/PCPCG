using System.Collections;

namespace PCC.Utility.Memory
{
    public class InfiniteBuffer<T> : IMemoryBuffer<T>
    {
        protected List<T> m_memory;

        public T this[int index]
        {
            get { return m_memory[index]; }
        }

        public InfiniteBuffer()
        {
            m_memory = new List<T>();
        }

        public void Add(T item)
        {
            m_memory.Add(item);
        }

        public void Clear()
        {
            m_memory.Clear();
        }

        public T Get(int indx)
        {
            // memory length is always > 0
            int index = indx % m_memory.Count;
            index = index < 0 ? index + m_memory.Count : index;

            return m_memory[index];
        }

        public void Replace(int indx, T item)
        {
            // memory length is always > 0
            int index = indx % m_memory.Count;
            index = index < 0 ? index + m_memory.Count : index;

            m_memory[index] = item;
        }

        public bool IsFull()
        {
            return false; // always never full
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < m_memory.Count; i++)
            {
                yield return m_memory[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            return m_memory.Count;
        }

        public int Size()
        {
            return m_memory.Capacity;
        }
    }
}
