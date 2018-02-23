using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TeensyCncManager.Test
{
    using System.Linq;    

    using TeensyCNCManager.Core.GCode;

    [TestClass]

    public class UnitTestGCodeProcess
    {
        [TestMethod]
        public void ParserIncompatibleParameters()
        {
            var gcodes = @"G00 X1 Y1";

            try
            {
                GParser.Parse(gcodes, null);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
            }

            gcodes = "";

            try
            {
                GParser.Parse(gcodes, null);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
            }
        }

        [TestMethod]
        public void ParseG00()
        {
            var gcodes = @"G00 X1 Y2 Z3 F4";

            var parsed = GParser.Parse(gcodes, new G00());

            Assert.IsInstanceOfType(parsed, typeof(G00));
            Assert.AreEqual(1, parsed.XDestination);
            Assert.AreEqual(2, parsed.YDestination);
            Assert.AreEqual(3, parsed.ZDestination);
            Assert.AreEqual(4, ((G00)parsed).FSpeed);
        }

        [TestMethod]
        public void ParseG01()
        {
            var gcodes = @"G01 X1 Y2 Z3 F4";

            var parsed = GParser.Parse(gcodes, new G00());

            Assert.IsInstanceOfType(parsed, typeof(G01));
            Assert.AreEqual(1, parsed.XDestination);
            Assert.AreEqual(2, parsed.YDestination);
            Assert.AreEqual(3, parsed.ZDestination);
            Assert.AreEqual(4, ((G00)parsed).FSpeed);
        }

        [TestMethod]
        public void ParseG02()
        {
            var gcodes = @"G02 X30 Y17.32050808 I20 J0";

            var parsed = GParser.Parse(gcodes, new G00 { FSpeed = 200, XDestination = 0, YDestination = 0, ZDestination = 0 });

            Assert.IsInstanceOfType(parsed, typeof(G02));

            var xpands = ((G02)parsed).Expand(new G00(), 200, 0.03, 0.001).ToList();

            Assert.IsTrue(xpands.Count > 2);            
        }
    }
}
