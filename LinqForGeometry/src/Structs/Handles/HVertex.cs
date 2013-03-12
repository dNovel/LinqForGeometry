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

/// <summary>
/// This is the handle struct for Vertices
/// </summary>
namespace hsfurtwangen.dsteffen.lfg.structs.handles {

    /// <summary>
    /// This is a handle struct for a 'Vertex'.
    /// If invalid, the handle is -1. If the value is 0 it is a valid handle. (Because it's a possible valid array index)
    /// </summary>
    public struct HVertex {
        internal int _Index;

        internal HVertex(int v) {
            _Index = v;
        }

        /// <summary>
        /// Implicitly converts the Handle to an integer value.
        /// </summary>
        /// <param name="h">Expects a 'HFace' struct as param.</param>
        /// <returns>Returns an int 'adress' value.</returns>
        public static implicit operator int(HVertex v) {
            return v._Index;
        }

        public bool isValid {
            get { return _Index >= 0; }
        }
    }
}   