using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarPlateRecon.Process;
namespace CarPlateReconTest
{
    [TestClass]
    public class UnitTest1
    {
       MatchHistogram Match = new MatchHistogram();
        [TestMethod]
        public void TestMethod1()
        {
            Match.test();
        }
    }
}
