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

namespace hsfurtwangen.dsteffen.lfg.structs.ptrcontainer {
	/// <summary>
	/// This is a half-edge 'Pointer Container'.
	/// The Reference to the next 'object' are stored in here depending on the 'half-edge data strucute'
	/// Every Vertex has a reference to one of his outgoing 'Half-Edges'
	/// </summary>
	internal struct HEdgePtrCont
	{
		internal HHalfEdge _he;
		internal HVertex _v;
		internal HFace _f;
	}
}