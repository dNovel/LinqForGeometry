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
        /// Is called directly after inserting a face and it's vertices, edges to the container is done
        /// </summary>
        /// <param name="handleEdge">the Edge Handle "containing" the half-edge the face should point to</param>
        public void UpdateFaceToHedgePtr(HandleEdge handleEdge)
        {
            FacePtrCont faceCont = _LfacePtrCont.Count - 1 < 0 ? _LfacePtrCont[0] : _LfacePtrCont[_LfacePtrCont.Count - 1];
            // TODO: This seems odd. Try looking up if the face the he1 points to is our current face, if not take a look at he2.
            HEdgePtrCont hEdgePtrCont1 = _LhedgePtrCont[_LedgePtrCont[handleEdge._DataIndex]._he1._DataIndex];
            HEdgePtrCont hEdgePtrCont2 = _LhedgePtrCont[_LedgePtrCont[handleEdge._DataIndex]._he2._DataIndex];

            if (hEdgePtrCont1._f._DataIndex == _LfacePtrCont.Count - 1)
            {
                faceCont._h._DataIndex = _LedgePtrCont[handleEdge._DataIndex]._he1._DataIndex;
            }
            else
            {
                faceCont._h._DataIndex = _LedgePtrCont[handleEdge._DataIndex]._he2._DataIndex;
            }
            _LfacePtrCont.RemoveAt(_LfacePtrCont.Count - 1);
            _LfacePtrCont.Add(faceCont);
        }


        /// <summary>
        /// Updates the "inner" half edges clockwise so the next pointers are correct.
        /// Is called after a face is inserted.
        /// </summary>
        /// <param name="edgeList">A list of edges that belong to a specific face</param>
        public void UpdateCWHedges(List<HandleEdge> edgeList)
        {
            var enumEdges = edgeList.GetEnumerator();
            bool end = false;
            bool firstDone = false;
            while (!end)
            {
                if (firstDone)
                {
                    EdgePtrCont edgePtrCont = _LedgePtrCont[enumEdges.Current._DataIndex];
                    HEdgePtrCont hedge1 = _LhedgePtrCont[edgePtrCont._he1._DataIndex];
                    int index = enumEdges.Current._DataIndex;
                    if (enumEdges.MoveNext())
                    {
                        hedge1._nhe._DataIndex = _LhedgePtrCont[_LedgePtrCont[enumEdges.Current._DataIndex]._he1._DataIndex]._he._DataIndex - 1;
                        // save to global list
                        _LhedgePtrCont.RemoveAt(edgePtrCont._he1._DataIndex);
                        _LhedgePtrCont.Insert(edgePtrCont._he1._DataIndex, hedge1);
                        //_LedgePtrCont.RemoveAt(index);
                        //_LedgePtrCont.Insert(index, edgePtrCont);
                    }
                    else
                    {
                        hedge1._nhe._DataIndex = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he1._DataIndex]._he._DataIndex - 1;
                        // save to global list
                        _LhedgePtrCont.RemoveAt(edgePtrCont._he1._DataIndex);
                        _LhedgePtrCont.Insert(edgePtrCont._he1._DataIndex, hedge1);
                        //_LedgePtrCont.RemoveAt(index);
                        //_LedgePtrCont.Insert(index, edgePtrCont);
                        end = true;
                    }
                }
                else
                {
                    enumEdges.MoveNext();
                    EdgePtrCont edgePtrCont = _LedgePtrCont[enumEdges.Current._DataIndex];
                    HEdgePtrCont hedge1 = _LhedgePtrCont[edgePtrCont._he1._DataIndex];
                    int index = enumEdges.Current._DataIndex;
                    enumEdges.MoveNext();
                    hedge1._nhe._DataIndex = _LhedgePtrCont[_LedgePtrCont[enumEdges.Current._DataIndex]._he1._DataIndex]._he._DataIndex - 1;
                    // save to global list
                    _LhedgePtrCont.RemoveAt(edgePtrCont._he1._DataIndex);
                    _LhedgePtrCont.Insert(edgePtrCont._he1._DataIndex, hedge1);
                    //_LedgePtrCont.RemoveAt(index);
                    //_LedgePtrCont.Insert(index, edgePtrCont);
                    firstDone = true;
                }
            }
        }

        /// <summary>
        /// Updates the "outer" half edges COUNTER clockwise so the next pointers are correct.
        /// Is called after a face is inserted.
        /// </summary>
        /// <param name="edgeList">A list of edges that belong to a specific face</param>
        public void UpdateCCWHedges(List<HandleEdge> edgeList)
        {
            edgeList.Reverse();
            var enumEdges = edgeList.GetEnumerator();
            bool end = false;
            bool firstDone = false;
            while (!end)
            {
                if (firstDone)
                {
                    EdgePtrCont edgePtrCont = _LedgePtrCont[enumEdges.Current._DataIndex];
                    HEdgePtrCont hedge2 = _LhedgePtrCont[edgePtrCont._he2._DataIndex];
                    int index = enumEdges.Current._DataIndex;
                    if (enumEdges.MoveNext())
                    {
                        hedge2._nhe._DataIndex = _LhedgePtrCont[_LedgePtrCont[enumEdges.Current._DataIndex]._he2._DataIndex]._he._DataIndex + 1;
                        // save to global list
                        _LhedgePtrCont.RemoveAt(edgePtrCont._he2._DataIndex);
                        _LhedgePtrCont.Insert(edgePtrCont._he2._DataIndex, hedge2);
                        //_LedgePtrCont.RemoveAt(index);
                        //_LedgePtrCont.Insert(index, edgePtrCont);
                    }
                    else
                    {
                        hedge2._nhe._DataIndex = _LhedgePtrCont[_LedgePtrCont[edgeList[0]._DataIndex]._he2._DataIndex]._he._DataIndex + 1;
                        // save to global list
                        _LhedgePtrCont.RemoveAt(edgePtrCont._he2._DataIndex);
                        _LhedgePtrCont.Insert(edgePtrCont._he2._DataIndex, hedge2);
                        //_LedgePtrCont.RemoveAt(index);
                        //_LedgePtrCont.Insert(index, edgePtrCont);
                        end = true;
                    }
                }
                else
                {
                    enumEdges.MoveNext();
                    EdgePtrCont edgePtrCont = _LedgePtrCont[enumEdges.Current._DataIndex];
                    HEdgePtrCont hedge2 = _LhedgePtrCont[edgePtrCont._he2._DataIndex];
                    int index = enumEdges.Current._DataIndex;
                    enumEdges.MoveNext();
                    hedge2._nhe._DataIndex = _LhedgePtrCont[_LedgePtrCont[enumEdges.Current._DataIndex]._he2._DataIndex]._he._DataIndex + 1;
                    // save to global list
                    _LhedgePtrCont.RemoveAt(edgePtrCont._he2._DataIndex);
                    _LhedgePtrCont.Insert(edgePtrCont._he2._DataIndex, hedge2);
                    //_LedgePtrCont.RemoveAt(index);
                    //_LedgePtrCont.Insert(index, edgePtrCont);
                    firstDone = true;
                }
            }
        }


        /// <summary>
        /// Expects a handle to an edge that belongs to a face.
        /// If the first hedge does point to the current face but another and the second does not point to any face it will point to the current face.
        /// </summary>
        /// <param name="handleEdge">Handle to an edge that belongs to the current processed face</param>
        public void IsValidEdge(HandleEdge handleEdge)
        {
            EdgePtrCont edge = _LedgePtrCont[handleEdge._DataIndex];
            HEdgePtrCont hedge1 = _LhedgePtrCont[edge._he1._DataIndex];
            HEdgePtrCont hedge2 = _LhedgePtrCont[edge._he2._DataIndex];

            int indexOfhedge2 = edge._he2._DataIndex; // problem starts here

            if (globalinf.LFGMessages._DEBUGOUTPUT)
            {
                Console.WriteLine("");
                Console.WriteLine("h2 is valid before: " +
                                  _LhedgePtrCont[edge._he2._DataIndex]._f.isValid.ToString()
                    );
            }

            if (_LhedgePtrCont[edge._he1._DataIndex]._f.isValid && _LhedgePtrCont[edge._he2._DataIndex]._f.isValid)
            {
                // Both valid, do nothing. Should not appear Oo.
            }
            else if (_LhedgePtrCont[edge._he1._DataIndex]._f.isValid && !_LhedgePtrCont[edge._he2._DataIndex]._f.isValid && _LhedgePtrCont[edge._he1._DataIndex]._f._DataIndex != _LfacePtrCont.Count() - 1)
            {
                // One is valid, two not. So change index at two.
                hedge2._f._DataIndex = _LfacePtrCont.Count() - 1;

                _LhedgePtrCont.RemoveAt(indexOfhedge2); // crash here
                _LhedgePtrCont.Insert(indexOfhedge2, hedge2);

                edge._he2._DataIndex = indexOfhedge2;
                if (globalinf.LFGMessages._DEBUGOUTPUT)
                {
                    Console.Write("Index of he2 = " + indexOfhedge2);
                }
                _LedgePtrCont.RemoveAt(handleEdge._DataIndex);
                _LedgePtrCont.Insert(handleEdge._DataIndex, edge);
            }
            else if (!_LhedgePtrCont[edge._he1._DataIndex]._f.isValid && _LhedgePtrCont[edge._he2._DataIndex]._f.isValid)
            {
                Console.WriteLine(globalinf.LFGMessages.WARNING_INVALIDCASE);
            }

            if (globalinf.LFGMessages._DEBUGOUTPUT)
            {
                Console.WriteLine("h2 is valid after: " +
                                  _LhedgePtrCont[edge._he2._DataIndex]._f.isValid.ToString()
                    );
            }
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
        /// Returns true if a connection already exists and fills the out parameter with a handle to the edge
        /// </summary>
        /// <param name="hv1">HandleVertex From vertex</param>
        /// <param name="hv2">HandleVertex To vertex</param>
        /// <param name="he">HandleEdge is filled when connection already exists with valid index otherwise with -1</param>
        /// <returns></returns>
        public bool GetOrAddConnection(HandleVertex hv1, HandleVertex hv2, out HandleEdge he)
        {
            int index = -1;
            if (_LedgePtrCont.Count != 0 && _LhedgePtrCont.Count != 0)
            {
                index = _LedgePtrCont.FindIndex(
                    edgePtrCont => _LhedgePtrCont[edgePtrCont._he1._DataIndex]._v._DataIndex == hv1._DataIndex && _LhedgePtrCont[edgePtrCont._he2._DataIndex]._v._DataIndex == hv2._DataIndex
                        ||
                    _LhedgePtrCont[edgePtrCont._he1._DataIndex]._v._DataIndex == hv2._DataIndex && _LhedgePtrCont[edgePtrCont._he2._DataIndex]._v._DataIndex == hv1._DataIndex
                    );
            }

            if (index >= 0)
            {
                if (globalinf.LFGMessages._DEBUGOUTPUT)
                {
                    Console.WriteLine("Existing Connection found!");
                }

                // TODO: Update the faces here? Should update the outside edge so it points to the current face i assume.
                // Update hedge 1 or two hmmm
                HEdgePtrCont hedgeToUpdate = _LhedgePtrCont[_LedgePtrCont[index]._he2._DataIndex];
                if (hedgeToUpdate._f._DataIndex == -1)
                {
                    hedgeToUpdate._f._DataIndex = _LfacePtrCont.Count - 1;
                    _LhedgePtrCont.RemoveAt(_LedgePtrCont[index]._he2._DataIndex);
                    _LhedgePtrCont.Insert(_LedgePtrCont[index]._he2._DataIndex, hedgeToUpdate);
                }

                he = new HandleEdge() { _DataIndex = index };
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
            hedge1._nhe._DataIndex = -1;

            hedge2._he._DataIndex = _LedgePtrCont.Count == 0 ? 0 : _LhedgePtrCont.Count;
            hedge2._v._DataIndex = hvFrom._DataIndex;
            hedge2._f._DataIndex = -1;
            hedge2._nhe._DataIndex = -1;

            _LhedgePtrCont.Add(hedge1);
            _LhedgePtrCont.Add(hedge2);
            _LedgePtrCont.Add(
                new EdgePtrCont()
                {
                    _he1 = new HandleHalfEdge() { _DataIndex = _LedgePtrCont.Count > 0 ? _LedgePtrCont.Count * 2 : 0 },
                    _he2 = new HandleHalfEdge() { _DataIndex = _LedgePtrCont.Count > 0 ? _LedgePtrCont.Count * 2 + 1 : 1 }
                }
            );

            // Update the vertices so they point to the correct hedges.
            VertexPtrCont vertFrom = _LvertexPtrCont[hvFrom._DataIndex];
            VertexPtrCont vertTo = _LvertexPtrCont[hvTo._DataIndex];

            if (vertFrom._h._DataIndex == -1)
            {
                vertFrom._h._DataIndex = _LedgePtrCont[_LedgePtrCont.Count - 1]._he1._DataIndex;
                _LvertexPtrCont.RemoveAt(hvFrom._DataIndex);
                _LvertexPtrCont.Insert(hvFrom._DataIndex, vertFrom);
            }
            if (vertTo._h._DataIndex == -1)
            {
                vertTo._h._DataIndex = _LedgePtrCont[_LedgePtrCont.Count - 1]._he2._DataIndex;
                _LvertexPtrCont.RemoveAt(hvTo._DataIndex);
                _LvertexPtrCont.Insert(hvTo._DataIndex, vertTo);
            }

            return new HandleEdge() { _DataIndex = _LhedgePtrCont.Count / 2 - 1 };
        }

    }
}

