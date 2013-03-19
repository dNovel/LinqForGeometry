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

using hsfurtwangen.dsteffen.lfg.structs.handles;
using Fusee.Math;

namespace hsfurtwangen.dsteffen.lfg
{

    class Loader
    {

        static KernelController _lfgSys;

        static void Main(string[] args)
        {
            _lfgSys = new KernelController();
            _lfgSys.LoadAsset("C:/Users/dominik/DevelopmentTEMP/LinqForGeometry/LinqForGeometry/assets/cube_square_1.obj");

            // do something interesting ...
        }

    }
}
