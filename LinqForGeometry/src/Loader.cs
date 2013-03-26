/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachlor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.globalinf;
using Fusee.Math;

namespace hsfurtwangen.dsteffen.lfg
{
    class Loader
    {
        static KernelController _lfgSys;

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            _lfgSys = new KernelController();
            stopWatch.Start();
            //_lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/plane_square_1.obj");
            _lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/cube_square_1.obj");
            //_lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/hellknight.obj");
            stopWatch.Stop();
            TimeSpan timeSpan = stopWatch.Elapsed;
            string timeDone = String.Format(LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
            Console.WriteLine("Time needed to import the object: " + timeDone);


            // do something interesting ...
        }

    }
}
