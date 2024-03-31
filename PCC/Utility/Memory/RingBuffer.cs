using System.Collections;

namespace PCC.Utility.Memory
{
    public class RingBuffer<T> : IMemoryBuffer<T>
    {
        protected T[] m_memory;

        public T this[int index]
        {
            get { return m_memory[index]; }
        }

        private int m_offset = 0;
        private bool m_full = false;


        public RingBuffer(int capacity = 20)
        {
            if (capacity < 0)
                throw new ArgumentException("Capcity must be greater than 0!");

            m_memory = new T[capacity];
        }

        public void Add(T sample)
        {
            m_memory[m_offset] = sample;
            m_offset++;

            if (m_offset >= m_memory.Length)
            {
                m_offset = 0;
                m_full = true;
            }
        }

        public T Get(int i)
        {
            // memory length is always > 0
            int index = i % m_memory.Length;
            index = index < 0 ? index + m_memory.Length : index;

            return m_memory[index];
        }

        public void Replace(int indx, T item)
        {
            // memory length is always > 0
            int index = indx % m_memory.Length;
            index = index < 0 ? index + m_memory.Length : index;

            m_memory[index] = item;
        }

        public void Clear()
        {
            m_offset = 0;
            m_full = false;
        }

        public bool IsFull()
        {
            return m_full;
        }

        public int GetNextSlotToFill()
        {
            return m_offset;
        }

        public static RingBuffer<T> CreateRingBuffer(int capacity = 20)
        {
            return new RingBuffer<T>(capacity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            int max = m_full ? m_memory.Length : m_offset;

            for (int i = 0; i < max; i++)
            {
                yield return m_memory[i];
            }
        }

        public int Count()
        {
            if (m_full)
                return m_memory.Length;
            else
                return m_offset;
        }

        public int Size()
        {
            return m_memory.Length;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
