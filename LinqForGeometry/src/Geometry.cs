// /*
// 	Author: Dominik Steffen
// 	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
// 	Bachlor Thesis Summer Semester 2013
// 	'Computer Science in Media'
// 	Project: LinqForGeometry
// 	Professors:
// 	Mr. Prof. C. Müller
// 	Mr. Prof. W. Walter
// */
using System;
using Fusee.Math;

using hsfurtwangen.dsteffen.lfg.structs.internals;

namespace hsfurtwangen.dsteffen.lfg {

	/// <summary>
	/// This is a container for the geometry of one mesh.
	/// So if a model is imported, it will be represented in the program as an object of this container class.
	/// </summary>
	public class Geometry
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="hsfurtwangen.dsteffen.lfg.Geometry"/> class.
		/// </summary>
		public Geometry () {
			// TODO remove this ...
			float3 d1 = new float3(1,2,3);
			Console.WriteLine(d1.x + ", " + d1.y + ", " + d1.z);
		}
	}
}

