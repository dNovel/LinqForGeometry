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

namespace hsfurtwangen.dsteffen.lfg
{

    /// <summary>
    /// This is a container for the geometry of one mesh.
    /// So if a model is imported, it will be represented in the program as an object of this container class.
    /// </summary>
    public class Geometry<VertexType, EdgeType, FaceType>
    {
        // Vars
        List<VertexType> _LvertexVal;
        List<EdgeType> _LedgeVal;

        List<VertexPtrCont> _LvertexPtrCont;
        List<HEdgePtrCont> _LhedgePtrCont;
        List<EdgePtrCont> _LedgePtrCont;


        /// <summary>
        /// Initializes a new instance of the <see cref="hsfurtwangen.dsteffen.lfg.Geometry"/> class.
        /// </summary>
        public Geometry()
        {
            _LvertexVal = new List<VertexType>();
            _LvertexPtrCont = new List<VertexPtrCont>();
            _LhedgePtrCont = new List<HEdgePtrCont>();
            _LedgePtrCont = new List<EdgePtrCont>();
        }

        /// <summary>
        /// Adds a vertex to the geometry container.
        /// </summary>
        /// <param name="val">Generic data type value.</param>
        public HandleVertex AddVertex(VertexType val)
        {
            _LvertexVal.Add(val);

            VertexPtrCont vPtrCont = new VertexPtrCont();
            vPtrCont._h = new HandleHalfEdge();
            vPtrCont._h._DataIndex = -1;
            _LvertexPtrCont.Add(vPtrCont);

            HandleVertex vHndl = new HandleVertex();
            vHndl._DataIndex = _LvertexPtrCont.Count - 1;

            return vHndl;
        }

        /// <summary>
        /// This method adds a edge to the container. The edge is 'drawn' between two vertices
        /// </summary>
        /// <param name="hv1">Vertex From</param>
        /// <param name="hv2">Vertex To</param>
        public HandleEdge AddEdge(HandleVertex hvFrom, HandleVertex hvTo)
        {
            HEdgePtrCont hedge1 = new HEdgePtrCont();
            hedge1._v = hvFrom;

            HEdgePtrCont hedge2 = new HEdgePtrCont();
            hedge2._v = hvTo;


            // TODO: Add the faces connection here, to? -> Call some lambda linq collection on the geo stack.


            hedge1._he = new HandleHalfEdge();
            hedge1._he._DataIndex = (_LhedgePtrCont.Count + 2) - 1;

            hedge2._he = new HandleHalfEdge();
            hedge2._he._DataIndex = (_LhedgePtrCont.Count + 2) - 2;

            _LhedgePtrCont.Add(hedge1);
            _LhedgePtrCont.Add(hedge2);

            // TODO: Update the half-edge id in the correct vertexPtrCont from the list. until now it should be -1! that's not good
            // TODO: There is something wrong with the relations. Check whats up with the vectors. If there is already one connection, dont create another one?
            // TODO: This is stragen ... Check the 'Half-Edge-Datastructure' definition!
            VertexPtrCont vertpc1 = _LvertexPtrCont[hvFrom._DataIndex];
            if (vertpc1._h._DataIndex == -1)
            { 
                vertpc1._h._DataIndex = _LhedgePtrCont.Count - 2;
                _LvertexPtrCont.RemoveAt(hvFrom._DataIndex);
                _LvertexPtrCont.Insert(hvFrom._DataIndex, vertpc1);
            }

            VertexPtrCont vertpc2 = _LvertexPtrCont[hvTo._DataIndex];
            if (vertpc2._h._DataIndex == -1)
            { 
                vertpc2._h._DataIndex = _LhedgePtrCont.Count - 1;
                _LvertexPtrCont.RemoveAt(hvTo._DataIndex);
                _LvertexPtrCont.Insert(hvTo._DataIndex, vertpc2);
            }
            //TODO Strange ends here
            
            EdgePtrCont edge = new EdgePtrCont();
            edge._he1 = hedge1;
            edge._he2 = hedge2;
            _LedgePtrCont.Add(edge);

            HandleEdge edgehndl = new HandleEdge();
            edgehndl._DataIndex = (_LhedgePtrCont.Count / 2) - 1;
            return edgehndl;
        }

        /// <summary>
        /// Returns the data corresponding to a vertex handle
        /// </summary>
        /// <param name="hv">HandleVertex with ID the data is wanted to be retrieved</param>
        /// <returns></returns>
        public VertexType GetVertexData(HandleVertex hv)
        {
            return _LvertexVal[hv._DataIndex];
        }

        /// <summary>
        /// Returns the two vectors of an edge
        /// </summary>
        /// <param name="he">An edge to get the two vectors from</param>
        /// <returns></returns>
        public VertexType[] GetEdgePoints(HandleEdge he)
        {
            int idFirst = he._DataIndex * 2;
            int idSec = he._DataIndex * 2 + 1;

            int vert1ID = _LhedgePtrCont[idFirst]._v._DataIndex;
            int vert2ID = _LhedgePtrCont[idSec]._v._DataIndex;

            VertexType vert1 = _LvertexVal[vert1ID];
            VertexType vert2 = _LvertexVal[vert2ID];

            VertexType[] points = { vert1, vert2 };

            return points;
        }


        /// <summary>
        /// Can test if there is already a connection between two vertice handles present
        /// </summary>
        /// <param name="hv1"></param>
        /// <param name="hv2"></param>
        /// <returns></returns>
        public Boolean GetConnection(HandleVertex hv1, HandleVertex hv2)
        {
            //TODO: Lambda LINQ stuff here.
            return false;
        }

    }
}

