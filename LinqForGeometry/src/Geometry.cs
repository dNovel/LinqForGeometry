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
using System.Linq;
using Fusee.Math;

using hsfurtwangen.dsteffen.lfg.structs.ptrcontainer;
using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.Importer;

namespace hsfurtwangen.dsteffen.lfg
{

    /// <summary>
    /// This is a container for the geometry of one mesh.
    /// So if a model is imported, it will be represented in the program as an object of this container class.
    /// </summary>
    public class Geometry<VertexType, FaceType, EdgeType>
    {
        // Vars
        private List<VertexType> _LvertexVal;
        private List<EdgeType> _LedgeVal;

        private List<VertexPtrCont> _LvertexPtrCont;
        private List<HEdgePtrCont> _LhedgePtrCont;
        private List<EdgePtrCont> _LedgePtrCont;
        private List<FacePtrCont> _LfacePtrCont;


        /// <summary>
        /// Initializes a new instance of the <see cref="hsfurtwangen.dsteffen.lfg.Geometry"/> class.
        /// </summary>
        public Geometry()
        {
            _LvertexVal = new List<VertexType>();

            _LvertexPtrCont = new List<VertexPtrCont>();
            _LhedgePtrCont = new List<HEdgePtrCont>();
            _LedgePtrCont = new List<EdgePtrCont>();
            _LfacePtrCont = new List<FacePtrCont>();
        }

        /// <summary>
        /// Adds a Face in form of a 'FacePtrCont' to the geometry container
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public HandleFace AddFace(GeoFace face)
        {
            _LfacePtrCont.Add(
                new FacePtrCont()
                {
                    _h = new HandleHalfEdge()
                    {
                        _DataIndex = -1
                    }
                }
            );
            HandleFace fHndl = new HandleFace();
            fHndl._DataIndex = _LfacePtrCont.Count - 1;
            return fHndl;
        }

        /// <summary>
        /// This updates the half-edge a face points to.
        /// Is called directly after inserting a face and its vertices, edges to the container is done
        /// </summary>
        /// <param name="handleEdge">the Edge Handle "containing" the half-edge the face should point to</param>
        public void UpdateFaceToHedgePtr(HandleEdge handleEdge)
        {
            FacePtrCont faceCont = _LfacePtrCont[_LfacePtrCont.Count - 1];
            faceCont._h._DataIndex = _LedgePtrCont[handleEdge._DataIndex]._he1._he._DataIndex - 1;
            _LfacePtrCont.RemoveAt(_LfacePtrCont.Count - 1);
            _LfacePtrCont.Add(faceCont);
        }

        /// <summary>
        /// Updates the neighbour of the first edge at the face so the CCW rotation can work.
        /// </summary>
        /// <param name="handleEdge">Handle to the first edge of the face</param>
        public void UpdateFirstCCWHedge(HandleEdge handleEdge, HandleEdge lastEdgeHandle) {
            EdgePtrCont edge = _LedgePtrCont[handleEdge._DataIndex];
            edge._he2._nhe._DataIndex = lastEdgeHandle._DataIndex;
            _LedgePtrCont.RemoveAt(handleEdge._DataIndex);
            _LedgePtrCont.Insert(handleEdge._DataIndex, edge);
            // TODO: The half edge has not been fixed. normally they should be fixed the half edge also when its updated. Related to Task ID: 27
        }
    

        /// <summary>
        /// Expects a handle to an edge that belongs to a face. Then it will test if the second hedge in this edge is not yet used and when the first hedge does not yet point
        /// to the face the second hedge should point to it.
        /// </summary>
        /// <param name="handleEdge">Handle to an edge that belongs to a face</param>
        public void IsValidEdge(HandleEdge handleEdge)
        {
            EdgePtrCont edge = _LedgePtrCont[handleEdge._DataIndex];

            Console.WriteLine("h2 is valid before: " +
                edge._he2._f.isValid.ToString()
                );

            if (edge._he1._f.isValid && edge._he2._f.isValid)
            {
                // Both valid, do nothing. Should not appear Oo.
            }
            else if (edge._he1._f.isValid && !edge._he2._f.isValid && edge._he1._f._DataIndex != _LfacePtrCont.Count() - 1)
            {
                // One is valid, two not. So change index at two.
                edge._he2._f._DataIndex = _LfacePtrCont.Count() - 1;
                _LedgePtrCont.RemoveAt(handleEdge._DataIndex);
                _LedgePtrCont.Insert(handleEdge._DataIndex, edge);
            }
            else if (!edge._he1._f.isValid && edge._he2._f.isValid)
            {
                Console.WriteLine(globalinf.LFGMessages.WARNING_INVALIDCASE);
            }

            Console.WriteLine("h2 is valid after: " +
                edge._he2._f.isValid.ToString()
                );
        }

        /// <summary>
        /// Adds a vertex to the geometry container.
        /// </summary>
        /// <param name="val">Generic data type value.</param>
        public HandleVertex AddVertex(VertexType val)
        {
            // if does not already exists
            int index = DoesVertexExist(val);
            if (index == -1)
            {
                _LvertexVal.Add(val);

                _LvertexPtrCont.Add(
                    new VertexPtrCont()
                    {
                        _h = new HandleHalfEdge()
                        {
                            _DataIndex = -1
                        }
                    }
                );

                return new HandleVertex() { _DataIndex = _LvertexPtrCont.Count - 1 };
            }
            else
            {
                HandleVertex vHndl = new HandleVertex();
                vHndl._DataIndex = index;
                return vHndl;
            }
        }

        /// <summary>
        /// Checks if a vertex already exists in the value list
        /// </summary>
        /// <returns>boolean, true if vertex does alreadyexist</returns>
        private int DoesVertexExist(VertexType v)
        {
            int index = _LvertexVal.FindIndex(vert => vert.Equals(v));
            return index >= 0 ? index : -1;
        }

        /// <summary>
        /// This method adds a edge to the container. The edge is 'drawn' between two vertices
        /// It first checks if a connection is already present. If so it returns a handle to this connection
        /// If not it will establish a connection between the two input vertices.
        /// </summary>
        /// <param name="hv1">Vertex From</param>
        /// <param name="hv2">Vertex To</param>
        public HandleEdge AddEdge(HandleVertex hvFrom, HandleVertex hvTo)
        {
            HandleEdge hndlEdge;
            GetOrAddConnection(hvFrom, hvTo, out hndlEdge);
            return new HandleEdge() { _DataIndex = hndlEdge._DataIndex };
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
        /// Returns true if a connection already exists and fills the out parameter with a handle to the edge
        /// </summary>
        /// <param name="hv1">HandleVertex From vertex</param>
        /// <param name="hv2">HandleVertex To vertex</param>
        /// <param name="he">HandleEdge is filled when connection already exists with valid index otherwise with -1</param>
        /// <returns></returns>
        public bool GetOrAddConnection(HandleVertex hv1, HandleVertex hv2, out HandleEdge he)
        {
            int indexOfEdge = _LedgePtrCont.FindIndex(edge => edge._he1._v._DataIndex == hv2._DataIndex && edge._he2._v._DataIndex == hv1._DataIndex || edge._he1._v._DataIndex == hv1._DataIndex && edge._he2._v._DataIndex == hv2._DataIndex);
            if (indexOfEdge >= 0)
            {
                he = new HandleEdge() { _DataIndex = indexOfEdge };
                return true;
            }
            else
            {
                he._DataIndex = CreateConnection(hv1, hv2)._DataIndex;
                return false;
            }
        }

        /// <summary>
        /// Establishes a connection between two vertices.
        /// 1) Creates two half-edges
        /// 2) Fills them with information
        /// 3) Creates an edge pointer container and adds it to the geo container.
        /// 4) returns a handle to an edge
        /// </summary>
        /// <param name="hv1"></param>
        /// <param name="hv2"></param>
        public HandleEdge CreateConnection(HandleVertex hvFrom, HandleVertex hvTo)
        {
            HEdgePtrCont hedge1 = new HEdgePtrCont();
            HEdgePtrCont hedge2 = new HEdgePtrCont();

            hedge1._he._DataIndex = _LedgePtrCont.Count == 0 ? 1 : _LhedgePtrCont.Count + 1;
            hedge1._v._DataIndex = hvTo._DataIndex;
            hedge1._f._DataIndex = _LfacePtrCont.Count - 1;
            // TODO: This should be inserted after the face is inserted Task ID: 26
            hedge1._nhe._DataIndex = hvFrom._DataIndex == 0 ? 2 : hvTo._DataIndex == 0 ? 0 : _LhedgePtrCont.Count + 2;

            hedge2._he._DataIndex = _LedgePtrCont.Count == 0 ? 0 : _LhedgePtrCont.Count;
            hedge2._v._DataIndex = hvFrom._DataIndex;
            hedge2._f._DataIndex = -1;
            // TODO: This should be inserted after the face is inserted Task ID: 26
            hedge2._nhe._DataIndex = hedge2._he._DataIndex - 1 < 0 ? 0 /* fix */: hedge2._he._DataIndex - 1;

            _LhedgePtrCont.Add(hedge1);
            _LhedgePtrCont.Add(hedge2);
            _LedgePtrCont.Add(
                new EdgePtrCont()
                {
                    _he1 = hedge1,
                    _he2 = hedge2
                }
            );

            return new HandleEdge() { _DataIndex = _LhedgePtrCont.Count / 2 - 1 };
        }

    }
}

