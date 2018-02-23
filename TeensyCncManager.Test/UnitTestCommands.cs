namespace TeensyCncManager.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TeensyCNCManager.Core.Commands;

    [TestClass]
    public class UnitTestCommands
    {
        [TestMethod]
        public void TestBinaryConversion()
        {
            var databytes = new byte[]
                                {
                                    0, 255, 0, 0, 0, 0, 0, 0, 10, 0, 0, 0, 10, 0, 0, 0, 10, 0, 0, 0, 10, 0, 0, 0, 10, 0, 0,
                                    0, 10, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                    0, 0, 0, 0, 0, 0, 0, 0, 0
                                };

            var state = new StatusReport(databytes);
            var data = state.GetDataBytes();         

            CollectionAssert.AreEqual(databytes, data);

        }
    }
}
