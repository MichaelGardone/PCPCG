namespace PCC.Utility.Memory
{
    public class RingBuffer<T>
    {
        private T[] m_memory;
        
        private int m_offset = 0;
        private bool m_full = false;

        public RingBuffer(int capacity = 20)
        {
            if (capacity < 0)
                throw new ArgumentException("Capcity must be greater than 0!");

            m_memory = new T[capacity];
        }

        public void Enqueue(T sample)
        {
            m_memory[m_offset] = sample;
            m_offset++;

            if (m_offset >= m_memory.Length)
            {
                m_offset = 0;
                m_full = true;
            }
        }

        public T PeekAt(int i)
        {
            // memory length is always > 0
            int index = i % m_memory.Length;
            index = index < 0 ? index + m_memory.Length : index;

            return m_memory[index];
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
    }
}
