using System;
using System.Linq;

namespace PCC.Utility.Memory
{
    public class RingBuffer
    {
        private int[] m_memory;
        
        private int m_offset = 0;
        private bool m_full = false;

        public RingBuffer(int capacity = 20)
        {
            if (capacity < 0)
                throw new ArgumentException("Capcity must be greater than 0!");

            m_memory = new int[capacity];
        }

        public void Enqueue(int sample)
        {
            m_memory[m_offset] = sample;
            m_offset++;

            if (m_offset >= m_memory.Length)
            {
                m_offset = 0;
                m_full = true;
            }
        }

        public int DequeueAt(int i)
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
