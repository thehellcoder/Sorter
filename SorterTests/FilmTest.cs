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
            Film f1 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            f1.AddSession("10:00", "100500", "IMAX");

            Film f2 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            for (int i = 1; i <= 9; i++)
            {
                f2.AddSession("10:00", "100500", "IMAX");
            }

            Film f3 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            for (int i = 1; i <= 10; i++)
            {
                f3.AddSession("10:00", "100500", "IMAX");
            }

            Film f4 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            for (int i = 1; i <= 18; i++)
            {
                f4.AddSession("10:00", "100500", "IMAX");
            }

            Assert.AreEqual(1, f1.getRowsCount());
            Assert.AreEqual(1, f2.getRowsCount());
            Assert.AreEqual(2, f3.getRowsCount());
            Assert.AreEqual(2, f4.getRowsCount());
        }
    }
}
