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
using System.Diagnostics;
using System.Collections.Generic;
using Fusee.Math;

using hsfurtwangen.dsteffen.lfg.structs.ptrcontainer;
using hsfurtwangen.dsteffen.lfg.structs.handles;

namespace hsfurtwangen.dsteffen.lfg {

    /// <summary>
    /// This is a container for the geometry of one mesh.
    /// So if a model is imported, it will be represented in the program as an object of this container class.
    /// </summary>
    public class Geometry<VT>
    {
        List<VT> _LvertexVal;

        List<VertexPtrCont> _LvertexPtrCont;
        List<EdgePtrCont> _LedgePtrCont;

        /// <summary>
        /// Initializes a new instance of the <see cref="hsfurtwangen.dsteffen.lfg.Geometry"/> class.
        /// </summary>
        public Geometry () {
            _LvertexVal = new List<VT>();
            
            _LvertexPtrCont = new List<VertexPtrCont>();
            _LedgePtrCont = new List<EdgePtrCont>();
        }

        /// <summary>
        /// Adds a vertex to the geometry container.
        /// </summary>
        /// <param name="val">Generic data type value.</param>
        public HVertex AddVertex(VT val) {

            _LvertexPtrCont.Add(
                new VertexPtrCont() {
                    _h = new HHalfEdge() {
                        _Index = _LvertexVal.Count
                    }
                }
            );
            _LvertexVal.Add(val);

            return new HVertex() { _Index = _LvertexVal.Count - 1 };
        }

        /// <summary>
        /// This method adds a edge to the container. The edge is 'drawn' between two vertices
        /// </summary>
        /// <param name="hv1"></param>
        /// <param name="hv2"></param>
        public void AddEdge(HVertex hvFrom, HVertex hvTo) {
            EdgePtrCont edge = new EdgePtrCont();

            HHalfEdge hedge1 = new HHalfEdge();
            hedge1._Index = hvFrom._Index;
            HHalfEdge hedge2 = new HHalfEdge();
            hedge2._Index = hvTo._Index;

            edge._he1 = hedge1;
            edge._he2 = hedge2;

            _LedgePtrCont.Add(edge);
        }

        /// <summary>
        /// Some Debug method for development
        /// </summary>
        public void ConsoleDebugListOut() {
            int c1 = _LvertexPtrCont.Count;
            int c2 = _LvertexVal.Count;
            Console.WriteLine("Count List ptr: " + c1);
            Console.WriteLine("Count List val: " + c2);

            foreach (VertexPtrCont vptr in _LvertexPtrCont)
            {
                VT val2 = _LvertexVal[vptr._h._Index];
                Console.WriteLine("Index of vertex is: " + vptr._h._Index + " value of corresponding item: " + val2);
            }

            Console.WriteLine("###");
            Console.WriteLine("###");

            foreach (EdgePtrCont edgeptr in _LedgePtrCont) {
                VT v1 = _LvertexVal[edgeptr._he1._Index];
                VT v2 = _LvertexVal[edgeptr._he2._Index];

                Console.WriteLine("Edge between: " + v1 + " and " + v2);
            }
        }

    }
}

