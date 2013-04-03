using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Fusee.Math;
using hsfurtwangen.dsteffen.lfg;
using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.Importer;

namespace hsfurtwangen.dsteffen.lfg
{
    public class KernelController
    {
        private WavefrontImporter<float3> _objImporter;
        private List<GeoFace> _GeoFaces;

        private Geometry<float3, GeoFace, float3> _GeometryContainer;


        /// <summary>
        /// These lists are public so the user can retrieve his handles and work with them.
        /// </summary>
        public List<HandleVertex> _LverticeHndl;
        public List<HandleEdge> _LedgeHndl;
        public List<HandleFace> _LfaceHndl;


        /// <summary>
        /// Constructor for the KernelController class.
        /// </summary>
        public KernelController()
        {
            _objImporter = new WavefrontImporter<float3>();
            _GeoFaces = new List<GeoFace>();

            _LverticeHndl = new List<HandleVertex>();
            _LedgeHndl = new List<HandleEdge>();
            _LfaceHndl = new List<HandleFace>();

            _GeometryContainer = new Geometry<float3, GeoFace, float3>(this);
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
        }


        /// <summary>
        /// Adds a vertex to the geometry container. Can then be controlled by the kernel
        /// </summary>
        /// <param name="val"></param>
        public HandleVertex AddVertex(float3 val)
        {
            HandleVertex hvToAdd = _GeometryContainer.AddVertex(val);
            _LverticeHndl.Add(hvToAdd);
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
                    _LedgeHndl.Add(handleEdge);
                    LtmpEdgesForFace.Add(handleEdge);
                }
                else
                {
                    HandleVertex hvTo = LhFaceVerts[0];
                    HandleEdge handleEdge = _GeometryContainer.AddEdge(hvFrom, hvTo);
                    _LedgeHndl.Add(handleEdge);
                    LtmpEdgesForFace.Add(handleEdge);
                }
            }

            // Update the face handle, so that it points to the first half edge the face consists of.
            _GeometryContainer.UpdateFaceToHedgePtr(LtmpEdgesForFace[0]);

            // Hand over the list of edges that are used for this face. Now build up the connections.
            _GeometryContainer.UpdateCWHedges(LtmpEdgesForFace);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the KernelControllers vertex handle list to be ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleVertex</returns>
        public IEnumerable<HandleVertex> StarIterateVertex(HandleVertex hv)
        {
            return _GeometryContainer.EnStarVertexVertex(hv);
        }

        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of INCOMING halfedge handles.
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the KernelControllers vertex handle list to be ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleHalfEdge</returns>
        public IEnumerable<HandleHalfEdge> StarVertexIncomingHalfEdge(HandleVertex hv)
        {
            return _GeometryContainer.EnStarVertexIncomingHalfEdge(hv);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object.
        /// Returns an enumerable of OUTGOING halfedge handles.
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the KernelControllers vertex handle list to be ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleHalfEdge</returns>
        public IEnumerable<HandleHalfEdge> StarVertexOutgoingHalfEdge(HandleVertex hv)
        {
            return _GeometryContainer.EnStarVertexOutgoingHalfEdge(hv);
        }


        /// <summary>
        /// Serves as an enumerable retriever from the geometry object
        /// Returns an enumerable of adjacent face handles.
        /// </summary>
        /// <param name="hv">A handle to a vertex, should be selected from the KernelControllers vertex handle list to be ensure it's correct.</param>
        /// <returns>IEnumerable of type HandleHalfEdge</returns>
        public IEnumerable<HandleFace> VertexAdjacentFaces(HandleVertex hv)
        {
            return _GeometryContainer.EnVertexAdjacentFaces(hv);
        }
    }
}
