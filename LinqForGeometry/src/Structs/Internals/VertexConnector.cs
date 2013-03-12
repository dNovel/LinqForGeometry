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

using hsfurtwangen.dsteffen.lfg.structs.handles;

namespace hsfurtwangen.dsteffen.lfg.structs.internals {
	/// <summary>
	/// Every Vertex has a reference to one of his outgoing 'Half-Edges'
	/// </summary>
	internal struct InternalVertex
	{
		internal HHalfEdge _h;
	}
}