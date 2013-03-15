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

namespace hsfurtwangen.dsteffen.lfg {
	
	class Loader {

        static KernelController<float3, float3, float3> _lfgSys;

		static void Main(string[] args) {
            _lfgSys = new KernelController<float3, float3, float3>();
			
            // TODO: this has to come from the importer not hardcoded ;)
            _lfgSys.AddVertex(new float3(0f, 0f, 0f));
            _lfgSys.AddVertex(new float3(1f, 1f, 1f));
            _lfgSys.AddVertex(new float3(2f, 2f, 2f));

            // IMPORTANT remember this for later. Don't jump over edges!
            //g1.AddEdge(h0, h1);
            //g1.AddEdge(h1, h2);
            //g1.AddEdge(h2, h3);
            
            // g1.ConsoleDebugListOut();
		}

	}
}
