using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fusee.Math;

namespace LinqForGeometry
{
    /*
    class Edge<T>
    {
        public Vertex<T> A { set; get; }
        public Vertex<T> B { set; get; }
        public Face<T> L { set; get; }
        public Face<T> R { set; get; }
    }

    struct Vertex<T> where T : struct
    {
        public T Attr
        {
            set { _geo._vertexAttribs[_inx] = value; }
            get { return _geo._vertexAttribs[_inx]; }
        }

        internal int _inx;
        internal Geometry<T> _geo;

        public IEnumerable<Face<T>> Faces
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<Edge<T>> Edges
        {
            get { throw new NotImplementedException(); }
        }
    }

    class Face<T> where T : struct
    {
        public float3 Normal { get; }

        // the reference to the containing geometry object
        internal Geometry<T> _geo; 
        // A face direcctly holds indices to Vertices
        internal List<int> _vertices; 
        public IEnumerable<Vertex<T>> Vertices
        {
             get 
             {
                 foreach (int vi in _vertices)
                 {
                     yield return new Vertex<T>(){_geo = _geo, _inx = _vertexAttribs[vi];}
                 }    
             }
        }

        public IEnumerable<Edge<T>> Edges
        {
            get { throw new NotImplementedException(); }
        }
    }
    */

    // Some general conventions:
    // o Invalid handles are -1. A handle value of 0 is valid (because it's a valid array index).
    // o Iterating over half edges around a face yields clockwise order.
    // @FIXME: This should be counter clockwise


    /// <summary>
    /// This is the handle struct for a 'full-edge'. 
    /// If invalid the handle is -1. If the value is 0 its valid (because it's a valid array index).
    /// </summary>
    public struct EdgeHandle
    {
        internal int _inx;

        /// <summary>
        /// Implicitly converts the Handle to an integer
        /// </summary>
        /// <param name="e">Expects an 'EdgeHandle' struct as param</param>
        /// <returns>Returns an int 'adress' value</returns>
        public static implicit operator int (EdgeHandle e)
        {
            return e._inx;
        }

        //WTF is this?
        /// <summary>
        /// Is only callable from the same assembly file
        /// </summary>
        /// <param name="i">Expects an integer value as param</param>
        internal EdgeHandle(int i)
        {
            _inx = i;
        }

        /// <summary>
        /// Tests for the EdgeHandle to be valid. The index has to be > 0
        /// </summary>
        public bool IsValid { get { return _inx >= 0; } }
    }


    /// <summary>
    /// This is the handle struct for a 'half-edge'.
    /// If invalid the handle is -1. If the value is 0 its valid (because it's a valid array index).
    /// </summary>
    public struct HalfEdgeHandle
    {
        internal int _inx;

        /// <summary>
        /// Implicitly converts the Handle to an integer
        /// </summary>
        /// <param name="e">Expects a 'HalfEdgeHandle' struct as param</param>
        /// <returns>Returns an int 'adress' value</returns>
        public static implicit operator int(HalfEdgeHandle e)
        {
            return e._inx;
        }
        internal HalfEdgeHandle (int i)
        {
            _inx = i;
        }
        public bool IsValid { get { return _inx >= 0; } }
    }


    /// <summary>
    /// This is the handle struct for a 'face'.
    /// If invalid the handle is -1. If the value is 0 its valid (because it's a valid array index).
    /// </summary>
    public struct FaceHandle
    {
        internal int _inx;

        /// <summary>
        /// Implicitly converts the Handle to an integer
        /// </summary>
        /// <param name="e">Expects a 'FaceHandle' struct as param</param>
        /// <returns>Returns an int 'adress' value</returns>
        public static implicit operator int(FaceHandle e)
        {
            return e._inx;
        }
        internal FaceHandle(int i)
        {
            _inx = i;
        }
        public bool IsValid { get { return _inx >= 0; } }
    }


    /// <summary>
    /// This is the handle struct for a 'Vertex'. 
    /// If invalid the handle is -1. If the value is 0 its valid (because it's a valid array index).
    /// </summary>
    public struct VertexHandle
    {
        internal int _inx;

        /// <summary>
        /// Implicitly converts the Handle to an integer
        /// </summary>
        /// <param name="e">Expects a 'FaceHandle' struct as param</param>
        /// <returns>Returns an int 'adress' value</returns>
        public static implicit operator int(VertexHandle e)
        {
            return e._inx;
        }
        internal VertexHandle(int i)
        {
           _inx = i;
        }
        public bool IsValid { get { return _inx >= 0; } }
    }


    /// <summary>
    /// This class instatiates a geometry object.
    /// Each geometry object represents one 'Mesh'.
    /// This is supposed to be called 'A Generic class'?
    /// </summary>
    /// <typeparam name="VT">These are vertex type attributes. Contains data for corresponding vertices</typeparam>
    /// <typeparam name="ET">These are edge type attributes. Contains data for corresponding edges</typeparam>
    /// <typeparam name="FT">These are face type attributes. Contains data for corresponding faces</typeparam>
    public class Geometry<VT, ET, FT>
    {
        private List<InternalVertex> _vertices; // This is actually the representer list for the vertex data type
        private List<InternalHalfEdge> _halfEdges; // This is actually the representer list for the halfedge  data type
        private List<InternalFace> _faces; // This is actually the representer list for the face data type e.g. Polygons etc...

        private List<VT> _vertexAttribs; // Vertex type attributes
        private List<ET> _edgeAttribs; // Edge type attributes
        private List<FT> _faceAttribs; // Face type attributes

        public Geometry()
        {
            _vertices = new List<InternalVertex>(); // list of vertex structs -> the structs only contain half-edge handles
            _halfEdges = new List<InternalHalfEdge>();
            _faces = new List<InternalFace>();

            _vertexAttribs = new List<VT>(); // E.g. contains a float3 vertex
            _edgeAttribs = new List<ET>();
            _faceAttribs = new List<FT>();
        }


        /// <summary>
        /// Adds a vertex to the internal vertex list '_vertices'.
        /// Adds the corresponding VertexAttributes with the exact same index to the list of the vertex attributes '_vertexAttribs'
        /// </summary>
        /// <param name="vt">Expects vertex type attributes for param e.g. float3 etc...</param>
        /// <returns>Returns a handle to the inserted vertex and indirect to its data</returns>
        public VertexHandle AddVertex(VT vt)
        {
            // Add a new InternalVertex Struct to the Vertices list
            _vertices.Add(
                    new InternalVertex()
                    {
                        /* This struct only contains this HalfEdgeHandle */
                        _h = new HalfEdgeHandle()
                        {
                            _inx = -1 /*Why not use the provided constructor here? And why set it invalid?*/
                        } 
                    }
                );
            _vertexAttribs.Add(vt); // Add the in the param provided float3 whatever vertex to the vertex attr. list.
            
            Debug.Assert(_vertexAttribs.Count == _vertices.Count);
            // Count the _vertices list (should contain 1 now) and substract 1 for array style enumeration
            // Now set the adress for the VertexHandle to the position the vertex struct in the _vertices struct list has
            return new VertexHandle(){_inx = _vertices.Count - 1};
        }

        //FIXME
        /// <summary>
        /// Trys to add a face defined from an array of vertices.
        /// </summary>
        /// <param name="vertices">Expects an array of vertex handles as param</param>
        /// <returns>Does not return anything yet</returns>
        public FaceHandle AddFace(VertexHandle[] vertices/*, out EdgeHandle[] edges*/)
        {
            // Check if params correct
            if (vertices == null)
            {
                throw new ArgumentNullException("No vertices added in AddFace()");
            }

            // Retrieve an enumerator from the 'vertices' array so we can traverse the array
            var enVerts = (IEnumerator<VertexHandle>) vertices.GetEnumerator();

            //FIXME Should be changed so there is correct exception handling
            // If this returns false we should throw an exception because then it is the end of the enumerator
            // With this if clause im preventing an InvalidOperationException
            if(enVerts.MoveNext())/*Move Next so the Enumerator pointer is on the first element*/
            {
                VertexHandle vStart = enVerts.Current; // retrieve the current element
                VertexHandle vPrev = vStart; // Set the prev. element to the current so we have a ring

                //This is supposed to get always the handle for the 'nextnext' vertex from the vertexhandle list
                for (enVerts.MoveNext(); enVerts.MoveNext();)
                {
                    EdgeHandle eh = GetOrAddEdge(vPrev, enVerts.Current);
                    if (_halfEdges[eh.])
                        vPrev = enVerts.Current;
                }

                //FIXME i dont get it for now
                for (int i = 0; i < vertices.Length; i++)
                {
                    int iNxt = (i + 1) % vertices.Length;
                    if (VertexVertex(vertices[i]).(vertices[iNxt]))
                    {
                    
                    }
                }
            }
            else
            {
                //FIXME We better not try to call Current(here) as it would throw an InvalidOperationException
            }
            //REMOVEME VertexHandle vStart = enVerts.Current;
            //REMOVEME VertexHandle vPrev = vStart;
        }


        /// <summary>
        /// WTF Should add an edge between two vertices if there is no one?
        /// Will return the existing edge if there is one.
        /// </summary>
        /// <param name="vFrom">A vertex handle from where to start</param>
        /// <param name="vTo">A vertex hangle for where to go</param>
        /// <returns>Returns a handle to the added or existing edge</returns>
        public EdgeHandle GetOrAddEdge(VertexHandle vFrom, VertexHandle vTo)
        {
            EdgeHandle eh = GetConnection(vFrom, vTo); // Get the connection between the two vertices
            if (!eh.IsValid) /* If the connection is invalid or that means not present create it*/
            {
                eh = AddEdge(vFrom, vTo);
            }
            return eh;
        }

        /// <summary>
        /// This adds an edge between two vertices where definitly is not an edge until now.
        /// Important! Should not be called alone, is invoked from other methods for now.
        /// </summary>
        /// <param name="vFrom">A vertex handle from where to start</param>
        /// <param name="vTo">A vertex hangle for where to go</param>
        /// <returns>Returns an 'EdgeHandle' to the edge just added</returns>
        public EdgeHandle AddEdge(VertexHandle vFrom, VertexHandle vTo)
        {
            //TODO retrieve the edge just added and return it, So something like this
            
            var fillInEdge1 = new InternalHalfEdge() { };
            fillInEdge1._v = vTo;

            var fillInEdge2 = new InternalHalfEdge() { };
            fillInEdge2._v = vFrom;
            
            _halfEdges.Add(fillInEdge1);
            _halfEdges.Add(fillInEdge2);

            //TODO retrieve the handle for the added edges
            // I know this is not correct but seems to represent the theory
            InternalEdge newEdge = new InternalEdge() { };
            newEdge._h0 = fillInEdge1;
            newEdge._h0 = fillInEdge2;

            // Now build up an EdgeHandle for the InternalEdge Struct
            EdgeHandle eh = new EdgeHandle()
                                    {
                                        _inx = (_halfEdges.Count()) / 2
                                    };
            
            /*
            _halfEdges.Add(
                    new InternalHalfEdge()
                    {
                        _v = vTo
                    }
                );
             */
            return eh; //FIXME just to get rid of the intellisense error - i might think this should be half-edge handle not edge
        }

        /// <summary>
        /// WTF This method retrieves a connection between two vertices.
        /// </summary>
        /// <param name="vFrom">VertexHandle to start from</param>
        /// <param name="vTo">VertexHandle to go to</param>
        /// <returns>Returns an 'EdgeHandle'</returns>
        public EdgeHandle GetConnection(VertexHandle vFrom, VertexHandle vTo)
        {
            int iHH = HalfEdgeHalfEdgeVertexInt(_vertices[vFrom]._h).DefaultIfEmpty(-1).First(i => _halfEdges[i]._v == vTo);
            return new EdgeHandle((iHH == -1) ? -1 : iHH / 2);
        }

        /// <summary>
        /// WTF This method retrieves a connection between two faces
        /// </summary>
        /// <param name="fFrom">FaceHandle to start from</param>
        /// <param name="fTo">FaceHandle to go to</param>
        /// <returns></returns>
        public EdgeHandle GetConnection(FaceHandle fFrom, FaceHandle fTo)
        {
            int iHH = HalfEdgeHalfEdgeFaceInt(_faces[fFrom]._h).DefaultIfEmpty(-1).First(i => _halfEdges[i]._f == fTo);
            return new EdgeHandle((iHH == -1) ? -1 : iHH / 2);
        }



        /*
        // This method should get inlined by the JIT. At least it serves all requirements.
        // (see: http://www.ademiller.com/blogs/tech/2008/08/c-inline-methods-and-optimization/)
        internal int GetHalfEdgeAt(int h)
        {

        }
        */

        public IEnumerable<HalfEdgeHandle> HalfEdgeHalfEdgeFace(HalfEdgeHandle hh)
        {
            return HalfEdgeHalfEdgeFaceInt(hh).Select(h => new HalfEdgeHandle(h));
        }

        internal IEnumerable<int> HalfEdgeHalfEdgeFaceInt(int h)
        {
            if (h < 0 || h >= _halfEdges.Count)
                yield break;

            for (int i = h; i != h; i = _halfEdges[i]._next)
                yield return i;
        }

        public IEnumerable<HalfEdgeHandle> HalfEdgeHalfEdgeVertex(HalfEdgeHandle hh)
        {
            return HalfEdgeHalfEdgeFaceInt(hh).Select(h => new HalfEdgeHandle(h));
            // return from HalfEdgeHalfEdgeFaceInt(hh) select (HalfEdgeHandle)h;
        }

        internal IEnumerable<int> HalfEdgeHalfEdgeVertexInt(int h)
        {
            if (h < 0 || h >= _halfEdges.Count)
                yield break;

            for (int i = h; i != h; i = _halfEdges[i]._next)
            {
                yield return i;
                // i ^ 1 flips to the corresponding half edge. If i is odd, i ^ 1 is the lower even number. If i is even, it adds 1 to i.
                // The same results from (i / 2) * 2 + (1 - i % 2)
                i = i ^ 1;
                yield return i;
            }
        }


        /// <summary>
        /// Circulate around a given vertex and enumerate all vertices connected by a direct edge.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the vertices connected to a given vertex by a direct edge.
        /// <code>
        /// Geometry myGeo = ...;
        /// VertexHandle v = ...;
        /// foreach (VertexHandle c in myGeo.VertexVertex(v))
        ///     Console.WriteLine(v + " is connected to " + c);
        /// </code>
        /// </example>
        /// <param name="vh">A handle to the vertex to circulate around.</param>
        /// <returns>An Enumerator of <see cref="VertexHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<VertexHandle> VertexVertex(VertexHandle vh)
        {
            return HalfEdgeHalfEdgeVertexInt(_vertices[vh]._h).Select(i => _halfEdges[i]._v);
        }


        /// <summary>
        /// Circulate around a given vertex and enumerate all connected directed half edges pointing towards the given point.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected incoming half edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// VertexHandle v = ...;
        /// foreach (HalfEdgeHandle h in myGeo.VertexHalfEdgeIncoming(v))
        ///     Console.WriteLine(h + " ends in " + v);
        /// </code>
        /// </example>
        /// <param name="vh">A handle to the vertex to circulate around.</param>
        /// <returns>An Enumerator of <see cref="HalfEdgeHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<HalfEdgeHandle> VertexHalfEdgeIncoming(VertexHandle vh)
        {
            // Return every odd halfedge (where inx & 1 == 1)
            return HalfEdgeHalfEdgeVertexInt(_vertices[vh]._h).Where((iH, inx) => (inx & 1) == 1).Select(i => new HalfEdgeHandle(i));
        }


        /// <summary>
        /// Circulate around a given vertex and enumerate all connected directed half edges pointing from the given vertex towards adjacent vertices.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected outgoing half edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// VertexHandle v = ...;
        /// foreach (HalfEdgeHandle h in myGeo.VertexHalfEdgeOutgoing(v))
        ///     Console.WriteLine(h + " starts in " + v);
        /// </code>
        /// </example>
        /// <param name="vh">A handle to the vertex to circulate around.</param>
        /// <returns>An Enumerator of <see cref="HalfEdgeHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<HalfEdgeHandle> VertexHalfEdgeOutgoing(VertexHandle vh)
        {
            // Take every even halfedge (where inx & 1 == 0)
            return HalfEdgeHalfEdgeVertexInt(_vertices[vh]._h).Where((iH, inx) => (inx & 1) == 0).Select(i => new HalfEdgeHandle(i));
        }


        /// <summary>
        /// Circulate around a given vertex and enumerate all connected edges shared with adjacent vertices.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// VertexHandle v = ...;
        /// foreach (EdgeHandle e in myGeo.VertexEdge(v))
        ///     Console.WriteLine(e + " connects with " + v);
        /// </code>
        /// </example>
        /// <param name="vh">A handle to the vertex to circulate around.</param>
        /// <returns>An Enumerator of <see cref="EdgeHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<EdgeHandle> VertexEdge(VertexHandle vh)
        {
            // Same as circulating over even halfedges (where inx & 1 == 0)
            return HalfEdgeHalfEdgeVertexInt(_vertices[vh]._h).Where((iH, inx) => (inx & 1) == 0).Select(i => new EdgeHandle(i / 2));
        }


        /// <summary>
        /// Circulate around a given vertex and enumerate all faces connected to the vertex.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// VertexHandle v = ...;
        /// foreach (FaceHandle f in myGeo.VertexFace(v))
        ///     Console.WriteLine(v + " is part of " + f);
        /// </code>
        /// </example>
        /// <param name="vh">A handle to the vertex to circulate around.</param>
        /// <returns>An Enumerator of <see cref="FaceHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<FaceHandle> VertexFace(VertexHandle vh)
        {
            // Star circulation. Filter out invalid face handles.
            return HalfEdgeHalfEdgeVertexInt(_vertices[vh]._h).Where(iH => _halfEdges[iH]._f >= 0).Select(i => new FaceHandle(_halfEdges[i]._f));
        }


        /// <summary>
        /// Circulate around a given face and enumerate all vertices connected to it.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected vertices.
        /// <code>
        /// Geometry myGeo = ...;
        /// FaceHandle f = ...;
        /// foreach (VertexHandle v in myGeo.FaceVertex(f))
        ///     Console.WriteLine(v + " is part of " + f);
        /// </code>
        /// </example>
        /// <param name="fh">A handle to the face to circulate around.</param>
        /// <returns>An Enumerator of <see cref="VertexHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<VertexHandle> FaceVertex(FaceHandle fh)
        {
            return HalfEdgeHalfEdgeFaceInt(_faces[fh]._h).Select(i => new VertexHandle(_halfEdges[i]._v));
        }


        /// <summary>
        /// Circulate around a given face and enumerate all edges connected to it.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// FaceHandle f = ...;
        /// foreach (EdgeHandle e in myGeo.FaceEdge(f))
        ///     Console.WriteLine(e + " is part of " + f);
        /// </code>
        /// </example>
        /// <param name="fh">A handle to the face to circulate around.</param>
        /// <returns>An Enumerator of <see cref="EdgeHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<EdgeHandle> FaceEdge(FaceHandle fh)
        {
            return HalfEdgeHalfEdgeFaceInt(_faces[fh]._h).Select(i => new EdgeHandle(i / 2));
        }


        /// <summary>
        /// Circulate around a given face and enumerate all "inner" halfedges connected to it. Inner half edges are considered to
        /// be in clockwise order.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected inner half edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// FaceHandle f = ...;
        /// foreach (HalfEdgeHandle e in myGeo.FaceHalfEdgeCW(f))
        ///     Console.WriteLine(e + " is part of " + f);
        /// </code>
        /// </example>
        /// <param name="fh">A handle to the face to circulate around.</param>
        /// <returns>An Enumerator of <see cref="HalfEdgeHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<HalfEdgeHandle> FaceHalfEdgeCW(FaceHandle fh)
        {
            return HalfEdgeHalfEdgeFaceInt(_faces[fh]._h).Select(i => new HalfEdgeHandle(i));
        }
        

        /// <summary>
        /// Circulate around a given face and enumerate all "outer" halfedges connected to it. Outer half edges are considered to
        /// be in counterclockwise order.
        /// </summary>
        /// <example>
        /// This examples shows how to access all the connected outer half edges.
        /// <code>
        /// Geometry myGeo = ...;
        /// FaceHandle f = ...;
        /// foreach (HalfEdgeHandle e in myGeo.FaceHalfEdgeCCW(f))
        ///     Console.WriteLine(e + " is part of " + f);
        /// </code>
        /// </example>
        /// <param name="fh">A handle to the face to circulate around.</param>
        /// <returns>An Enumerator of <see cref="HalfEdgeHandle"/> to be used e.g. in foreach statements.</returns>
        public IEnumerable<HalfEdgeHandle> FaceHalfEdgeCCW(FaceHandle fh)
        {
            return HalfEdgeHalfEdgeFaceInt(_faces[fh]._h).Select(i => new HalfEdgeHandle(i ^ 1));
        }


        /// <summary>
        /// Iterate over the two vertices connected to an edge.
        /// </summary>
        /// <param name="eh">The edge in question.</param>
        /// <returns>An Enumerator of <see cref="VertexHandle"/> enumerating two vertices.</returns>
        public IEnumerable<VertexHandle> EdgeVertex(EdgeHandle eh) 
        { 
            if (_halfEdges[2 * eh._inx]._v >= 0)
                yield return new VertexHandle(_halfEdges[2 * eh._inx]._v);
            if (_halfEdges[2 * eh._inx + 1]._v >= 0)
                yield return new VertexHandle(_halfEdges[2 * eh._inx + 1]._v);
        }

        /// <summary>
        /// Iterate over the (maximum of) two faces connected to an edge.
        /// </summary>
        /// <param name="eh">The edge in question.</param>
        /// <returns>An Enumerator of <see cref="FaceHandle"/> enumerating one or two faces.</returns>
        public IEnumerable<FaceHandle> EdgeFace(EdgeHandle eh)
        {
            if (_halfEdges[2 * eh._inx]._f >= 0)
                yield return new FaceHandle(_halfEdges[2 * eh._inx]._f);
            if (_halfEdges[2 * eh._inx + 1]._f >= 0)
                yield return new FaceHandle(_halfEdges[2 * eh._inx + 1]._f);
        }

    
    }


    /// <summary>
    /// Every 'Edge' has references to its two 'Half-Edges'
    /// </summary>
    internal struct InternalEdge
    {
        internal InternalHalfEdge _h0;
        internal InternalHalfEdge _h1;
    }


    /// <summary>
    /// Every 'Half-Edge' has references to:
    /// The vertex its pointing to _v
    /// The face it belongs to _f
    /// The next 'Half-Edge' counter clockwise _next
    /// </summary>
    internal struct InternalHalfEdge
    {
        internal VertexHandle _v;
        internal FaceHandle _f;
        internal HalfEdgeHandle _next;
        // internal HalfEdgeHandle _prev;
    }

    
    /// <summary>
    /// Every Vertex has a reference to one of his outgoing 'Half-Edges'
    /// </summary>
    internal struct InternalVertex
    {
        internal HalfEdgeHandle _h;
    }


    /// <summary>
    /// Every face has a reference to one of its 'Half-Edges'
    /// </summary>
    internal struct InternalFace
    {
        internal HalfEdgeHandle _h;
    }



    class Program
    {
        static void Main(string[] args)
        {
            //FIXME Just to get rid of the intellisense error
            Geometry<float3, float3, float3> geo = new Geometry<float3, float3, float3>();
            //Geometry<float3> geo = new Geometry<float3>();

            // Use cases:
            // Get all Verts/Edges/Points connected to v/e/p
            //var q = from v in geo.Vertices 
                //where v.

            // Calculate vertexnormals based on a smoothing angle

            // Implement Bevelling/Catmull-Clark subdivision, ....

        }
    }
}
