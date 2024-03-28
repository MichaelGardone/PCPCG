using PCC.Utility.Memory;

namespace PCCTester.Memory
{
    [TestClass]
    public class RingBufferTests
    {
        [TestMethod]
        public void RingBufferFunctionalityTest()
        {
            RingBuffer<int> ringBuffer = new RingBuffer<int>();
            
            for(int i = 0; i < 19; i++)
            {
                ringBuffer.Enqueue(i);
                Assert.IsFalse(ringBuffer.IsFull());
            }

            ringBuffer.Enqueue(19);
            Assert.IsTrue(ringBuffer.IsFull());

            for(int i = 0; i < 20; i++)
            {
                Assert.AreEqual(i, ringBuffer.PeekAt(i));
            }

            Assert.AreEqual(19, ringBuffer.PeekAt(-1));
            Assert.AreEqual(0, ringBuffer.PeekAt(-20));

            Assert.AreEqual(0, ringBuffer.PeekAt(20));
            Assert.AreEqual(1, ringBuffer.PeekAt(21));
            Assert.AreEqual(19, ringBuffer.PeekAt(20 * 2 - 1));

            ringBuffer.Enqueue(20);
            Assert.AreNotEqual(0, ringBuffer.PeekAt(0));
            Assert.AreEqual(1, ringBuffer.PeekAt(1));

            for(int i = 21; i < 40; i++)
            {
                ringBuffer.Enqueue(i);
            }

            for (int i = 0; i < 19; i++)
            {
                Assert.AreEqual(i + 20, ringBuffer.PeekAt(i));
            }

            ringBuffer.Clear();
            Assert.IsFalse(ringBuffer.IsFull());

            for (int i = 100; i < 120; i++)
            {
                ringBuffer.Enqueue(i);
            }

            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(i + 100, ringBuffer.PeekAt(i));
            }
        }

    }
}
