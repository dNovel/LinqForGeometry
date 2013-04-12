using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Fusee.Math;
using Fusee.Engine;
using hsfurtwangen.dsteffen.lfg;
using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.Importer;

namespace hsfurtwangen.dsteffen.lfg
{
    public class Geometry
    {
        private WavefrontImporter<float3> _objImporter;
        private List<GeoFace> _GeoFaces;

        private GeometryData _GeometryContainer;


        /// <summary>
        /// These lists are public so the user can retrieve his handles and work with them.
        /// </summary>
        public List<HandleVertex> _LverticeHndl;
        public List<HandleEdge> _LedgeHndl;
        public List<HandleFace> _LfaceHndl;
        public List<short> _LtriangleList;

        /// <summary>
        /// Constructor for the GeometryData class.
        /// </summary>
        public Geometry()
        {
            _objImporter = new WavefrontImporter<float3>();
            _GeoFaces = new List<GeoFace>();

            _LverticeHndl = new List<HandleVertex>();
            _LedgeHndl = new List<HandleEdge>();
            _LfaceHndl = new List<HandleFace>();

            _LtriangleList = new List<short>();

            _GeometryContainer = new GeometryData(this);
        }


        /// <summary>
        /// Loads an asset specified by the path
        /// </summary>
        /// <param name="path">Path to the wavefront file</param>
        public void LoadAsset(String path)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<GeoFace> faceList = _objImporter.LoadAsset(path);

            // Convert a x-poly model to a triangular poly model because FUSEE only can handle triangular polys for now.
            if (globalinf.LFGMessages.FLAG_FUSEE_TRIANGLES)
            {
                List<GeoFace> newFaces = ConvertFacesToTriangular(faceList);
                faceList.Clear();
                faceList = newFaces;
            }

            TimeSpan timeSpan = stopWatch.Elapsed;
            string timeDone = String.Format(globalinf.LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
            Console.WriteLine("\n\n     Time needed to import the .obj file: " + timeDone);
            stopWatch.Restart();

            if (globalinf.LFGMessages._DEBUGOUTPUT)
            {
                Console.WriteLine(globalinf.LFGMessages.INFO_PROCESSINGDS);
            }

            // Work on the facelist and transform the data structure to the 'half-edge' data structure.
            foreach (GeoFace gf in faceList)
            {
                AddFace(gf);
            }

            stopWatch.Stop();
            timeSpan = stopWatch.Elapsed;
            timeDone = String.Format(globalinf.LFGMessages.UTIL_STOPWFORMAT, timeSpan.Seconds, timeSpan.Milliseconds);
            Console.WriteLine("\n\n     Time needed to convert the object to the HES: " + timeDone);

            // TODO: Calc the vertex normals.
            _GeometryContainer._LVertexNormals = EnAllVertices().Select(handleVert => _GeometryContainer.CalcVertexNormal(handleVert)).ToList();
            if (globalinf.LFGMessages._DEBUGOUTPUT && 1 == 0)
            {
                Console.WriteLine("Vertex Normals: ");
                foreach (float3 lVertexNormal in _GeometryContainer._LVertexNormals)
                {
                    Console.WriteLine("     $" + lVertexNormal.ToString());
                }
            }
        }


        /// <summary>
        /// This method converts a quadrangular polygon mesh to a triangular polygon mesh
        /// </summary>
        /// <param name="faces">List of GeoFace</param>
        /// <returns>List of GeoFaces</returns>
        private List<GeoFace> ConvertFacesToTriangular(List<GeoFace> faces)
        {
            int secondVert = 0;
            List<GeoFace> triangleFaces = new List<GeoFace>();

            foreach (GeoFace face in faces)
            {
                int faceVertCount = face._LFVertices.Count;

                if (faceVertCount == 3)
                {
                    GeoFace newFace = new GeoFace() { _LFVertices = new List<float3>() };
                    newFace._LFVertices.Add(face._LFVertices[0]);
                    newFace._LFVertices.Add(face._LFVertices[1]);
                    newFace._LFVertices.Add(face._LFVertices[2]);
                    triangleFaces.Add(newFace);
                }
                else if (faceVertCount > 3)
                {
                    secondVert++;
                    while (secondVert != faceVertCount - 1)
                    {
                        GeoFace newFace = new GeoFace() { _LFVertices = new List<float3>() };
                        newFace._LFVertices.Add(face._LFVertices[0]);
                        newFace._LFVertices.Add(face._LFVertices[secondVert]);
                        newFace._LFVertices.Add(face._LFVertices[secondVert + 1]);
                        triangleFaces.Add(newFace);
                        secondVert++;
                    }
                    secondVert = 0;
                }
                else if (faceVertCount < 3)
                {
                    // TODO: Error? Face with less than 3 vertices does not exist.
                }
            }
            return triangleFaces;
        }


        /// <summary>
        /// Converts the geometry to a fusee mesh object.
        /// </summary>
        /// <returns>Fusee Mesh</returns>
        public Mesh ToMesh()
        {
            Mesh mesh = new Mesh();
            mesh.Vertices = _GeometryContainer._LvertexVal.ToArray();
            mesh.Normals = _GeometryContainer._LVertexNormals.ToArray();

            // TODO: Meh, not that good sort of a code ;) ... Can do better.
            foreach (var handleVertex in _LfaceHndl.Select(handleFace => FaceSurroundingVertices(handleFace)).SelectMany(vertsFace => vertsFace))
            {
                _LtriangleList.Add((short)handleVertex._DataIndex);
            }
            mesh.Triangles = _LtriangleList.ToArray();

            return mesh;
        }

        /// <summary>
        /// Adds a vertex to the geometry container. Can then be controlled by the kernel
        /// </summary>
        /// <param name="val"></param>
        public HandleVertex AddVertex(float3 val)
        {
            HandleVertex hvToAdd = _GeometryContainer.AddVertex(val);

            if (!_LverticeHndl.Contains(hvToAdd))
            {
                _LverticeHndl.Add(hvToAdd);
            }
            else
            {
                Console.WriteLine("$$$ Vertex has been already inserted!");
            }
            return hvToAdd;
        }


        /// <summary>
        /// Adds a face from the importer to the geometry container
        /// </summary>
        /// <param name="gf">GeoFace object from the importer</param>
        private void AddFace(GeoFace gf)
        {
            _LfaceHndl.Add(
                _GeometryContainer.AddFace(gf)
                );

            List<HandleVertex> LhFaceVerts = new List<HandleVertex>();
            foreach (float3 vVal in gf._LFVertices)
            {
                LhFaceVerts.Add(
                        AddVertex(vVal)
                    );
            }

            List<HandleEdge> LtmpEdgesForFace = new List<HandleEdge>();
            int vertsCount = LhFaceVerts.Count;
            for (int i = 0; i < vertsCount; i++)
            {
                HandleVertex hvFrom = LhFaceVerts[i];
                if (i + 1 < vertsCount)
                {
                    HandleVertex hvTo = LhFaceVerts[i + 1];
                    HandleEdge handleEdge = _GeometryContainer.AddEdge(hvFrom, hvTo);
                    if (!_LedgeHndl.Contains(handleEdge))
                    {
                        _LedgeHndl.Add(handleEdge);
                    }
                    else
                    {
                        Console.WriteLine("$$$ Edge has been already inserted!");
                    }
                    LtmpEdgesForFace.Add(handleEdge);
                }
                else
                {
                    HandleVertex hvTo = LhFaceVerts[0];
                    HandleEdge handleEdge = _GeometryContainer.AddEdge(hvFrom, hvTo);
                    if (!_LedgeHndl.Contains(handleEdge))
                    {
                        _LedgeHndl.Add(handleEdge);
                    }
                    else
                    {
                        Console.WriteLine("$$$ Edge has been already inserted!");
                    }
                    LtmpEdgesForFace.Add(handleEdge);
                }
            }

            // Update the face handle, so that it points to the first half edge the face consists of.
            _GeometryContainer.UpdateFaceToHedgePtr(LtmpEdgesForFace[0]);

            // Hand over the list of edges that are used for this face. Now build up the connections.
            _GeometryContainer.UpdateCWHedges(LtmpEdgesForFace);

            // Calculate and add the face normal to a list here
            int lastFaceIndex = _LfaceHndl.Count - 1;
            _GeometryContainer.AddFaceNormal(_LfaceHndl[lastFaceIndex]);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the GeometryData's vertex handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleVertex</returns>
        public IEnumerable<HandleVertex> StarIterateVertex(HandleVertex hv)
        {
            return _GeometryContainer.EnStarVertexVertex(hv);
        }

        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of INCOMING halfedge handles.
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the GeometryData's vertex handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleHalfEdge</returns>
        public IEnumerable<HandleHalfEdge> StarVertexIncomingHalfEdge(HandleVertex hv)
        {
            return _GeometryContainer.EnStarVertexIncomingHalfEdge(hv);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of OUTGOING halfedge handles.
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the GeometryData's vertex handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleHalfEdge</returns>
        public IEnumerable<HandleHalfEdge> StarVertexOutgoingHalfEdge(HandleVertex hv)
        {
            return _GeometryContainer.EnStarVertexOutgoingHalfEdge(hv);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object
        /// Returns an enumerable of adjacent face handles.
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the GeometryData's vertex handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleFace</returns>
        public IEnumerable<HandleFace> VertexAdjacentFaces(HandleVertex hv)
        {
            return _GeometryContainer.EnVertexAdjacentFaces(hv);
        }

        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of surrounding halfedges specific to a center face.
        /// </summary>
        /// <param name="hf">A handle to a face, should be selected from the GeometryData's face handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleHalfEdge</returns>
        public IEnumerable<HandleHalfEdge> FaceSurroundingHalfEdges(HandleFace hf)
        {
            return _GeometryContainer.EnFaceHalfEdges(hf);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of surrounding vertices specific to a center face.
        /// </summary>
        /// <param name="hf">A handle to a face, should be selected from the GeometryData's face handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleVertex</returns>
        public IEnumerable<HandleVertex> FaceSurroundingVertices(HandleFace hf)
        {
            return _GeometryContainer.EnFaceVertices(hf);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of surrounding vertices specific to a center face.
        /// </summary>
        /// <param name="hf">A handle to a face, should be selected from the GeometryData's face handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleEdge</returns>
        public IEnumerable<HandleEdge> FaceSurroundingEdges(HandleFace hf)
        {
            return _GeometryContainer.EnFaceEdges(hf);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of surrounding faces specific to a center face.
        /// </summary>
        /// <param name="hf">A handle to a face, should be selected from the GeometryData's face handle list to ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleFace</returns>
        public IEnumerable<HandleFace> FaceSurroundingFaces(HandleFace hf)
        {
            return _GeometryContainer.EnFaceFaces(hf);
        }



        /* Standard Circle Iterators over all elemets of the geometry object */

        /// <summary>
        /// Returns an enumerable of all vertices handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleVertex</returns>
        public IEnumerable<HandleVertex> EnAllVertices()
        {
            return _LverticeHndl.AsEnumerable();
        }


        /// <summary>
        /// Returns an enumerable of all edge handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleEdge</returns>
        public IEnumerable<HandleEdge> EnAllEdges()
        {
            return _LedgeHndl.AsEnumerable();
        }


        /// <summary>
        /// Returns an enumerable of all face handles in the geometry structure.
        /// </summary>
        /// <returns>IEnumerable of type HandleFace</returns>
        public IEnumerable<HandleFace> EnAllFaces()
        {
            return _LfaceHndl.AsEnumerable();
        }


    }
}
