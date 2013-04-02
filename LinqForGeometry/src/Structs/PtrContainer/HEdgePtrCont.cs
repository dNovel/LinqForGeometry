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

using hsfurtwangen.dsteffen.lfg.structs.handles;

namespace hsfurtwangen.dsteffen.lfg.structs.ptrcontainer
{
    /// <summary>
    /// This is a half-edge 'Pointer Container'.
    /// The Reference to the next 'object' are stored in here depending on the 'half-edge data strucute'
    /// Every half-edge has the following references:
    /// a reference to it's direct neighbour half-edge
    /// a reference to the next half-edge in clock wise order.
    /// a reference to a vertex the half-edge points to.
    /// a reference to a face the half-edge belongs to.
    /// </summary>
    internal struct HEdgePtrCont
    {
        internal HandleHalfEdge _he;
        internal HandleHalfEdge _nhe;
        // This is optional ...
        // internal HandleHalfEdge _phe;
        internal HandleVertex _v;
        internal HandleFace _f;
    }
}