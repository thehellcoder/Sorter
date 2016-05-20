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
            Film f0 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");

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

            Assert.AreEqual(0, f0.GetRowsCount());
            Assert.AreEqual(1, f1.GetRowsCount());
            Assert.AreEqual(1, f2.GetRowsCount());
            Assert.AreEqual(2, f3.GetRowsCount());
            Assert.AreEqual(2, f4.GetRowsCount());
        }

        [TestMethod]
        public void TestFormatAssignment()
        {
            Film f0 = new Film("Экипаж IMAX 3D", "", "", "");
            Film f1 = new Film("Паранормальное явление 5: призраки в 3D IMAX 2D", "", "", "");
            Film f2 = new Film("Паранормальное явление 5: призраки в 3D 2D", "", "", "");
            Film f3 = new Film("Варкрафт 3D", "", "", "");

            Assert.AreEqual(f0.Format, "IMAX 3D");
            Assert.AreEqual(f1.Format, "IMAX 2D");
            Assert.AreEqual(f2.Format, "2D");
            Assert.AreEqual(f3.Format, "3D");
        }

        [TestMethod]
        public void TestTrailerFilenameTranslate()
        {
            Film f0 = new Film("Экипаж", "", "", "");
            Film f1 = new Film("#ВСЕ_ИСПРАВИТЬ!?!", "", "", "");

            Assert.AreEqual(f0.TrailerFileName, "ekipaj");
            Assert.AreEqual(f1.TrailerFileName, "vse_ispravit");
        }
    }
}
