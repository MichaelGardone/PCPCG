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
                ringBuffer.Add(i);
                Assert.IsFalse(ringBuffer.IsFull());
            }

            ringBuffer.Add(19);
            Assert.IsTrue(ringBuffer.IsFull());

            for(int i = 0; i < 20; i++)
            {
                Assert.AreEqual(i, ringBuffer.Get(i));
            }

            Assert.AreEqual(19, ringBuffer.Get(-1));
            Assert.AreEqual(0, ringBuffer.Get(-20));

            Assert.AreEqual(0, ringBuffer.Get(20));
            Assert.AreEqual(1, ringBuffer.Get(21));
            Assert.AreEqual(19, ringBuffer.Get(20 * 2 - 1));

            ringBuffer.Add(20);
            Assert.AreNotEqual(0, ringBuffer.Get(0));
            Assert.AreEqual(1, ringBuffer.Get(1));

            for(int i = 21; i < 40; i++)
            {
                ringBuffer.Add(i);
            }

            for (int i = 0; i < 19; i++)
            {
                Assert.AreEqual(i + 20, ringBuffer.Get(i));
            }

            ringBuffer.Clear();
            Assert.IsFalse(ringBuffer.IsFull());

            for (int i = 100; i < 120; i++)
            {
                ringBuffer.Add(i);
            }

            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(i + 100, ringBuffer.Get(i));
            }

            Assert.AreEqual(100, ringBuffer[0]);
            Assert.AreEqual(105, ringBuffer[5]);
        }

    }
}
