﻿using System;
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
                f2.AddSession(String.Format("{0:00}:00", i), "100500", "IMAX");
            }

            Film f3 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            for (int i = 1; i <= 10; i++)
            {
                f3.AddSession(String.Format("{0:00}:00", i), "100500", "IMAX");
            }

            Film f4 = new Film("Экипаж", "trailer.mp4", "3:00", "6+");
            for (int i = 1; i <= 18; i++)
            {
                f4.AddSession(String.Format("{0:00}:00", i), "100500", "IMAX");
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
            Film f2 = new Film("Паранормальное явление 5: Призраки в 3D IMAX 2D", "", "", "");
            Film f3 = new Film("Золото (2016) 2D", "", "", "");
            Film f4 = new Film("МУЛЬТ в кино. Выпуск №49 2D", "", "", "");

            Assert.AreEqual(f0.TrailerFileName, "ekipaj.mp4");
            Assert.AreEqual(f1.TrailerFileName, "vse_ispravit.mp4");
            Assert.AreEqual(f2.TrailerFileName, "paranormalnoe_iavlenie_5_prizraki_v_3d.mp4");
            Assert.AreEqual(f3.TrailerFileName, "zoloto.mp4");
            Assert.AreEqual(f4.TrailerFileName, "mult_v_kino.mp4");
        }
    }
}
