using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sorter;

namespace SorterTests
{
    [TestClass]
    public class FilmTest
    {
        [TestMethod]
        public void TestGetRowsCount()
        {
            Film tmp = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            for(int i = 1; i <= 9; i++)
            {
                tmp.AddSession("10:00", "100500", "IMAX");
            }
            Assert.AreEqual(1, tmp.getRowsCount());
        }
    }
}
