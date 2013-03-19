using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fusee.Math;
using hsfurtwangen.dsteffen.lfg;
using hsfurtwangen.dsteffen.lfg.structs.handles;
using hsfurtwangen.dsteffen.lfg.Importer;

namespace hsfurtwangen.dsteffen.lfg
{
    class KernelController
    {
        WavefrontImporter<float3> _objImporter;
        List<GeoFace> _GeoFaces;

        Geometry<float3, GeoFace, float3> _GeometryContainer;

        List<HandleVertex> _LverticeHndl;
        List<HandleEdge> _LedgeHndl;
        List<HandleFace> _LfaceHndl;

        public KernelController()
        {
            _objImporter = new WavefrontImporter<float3>();
            _GeoFaces = new List<GeoFace>();

            _GeometryContainer = new Geometry<float3, GeoFace, float3>();
            _LverticeHndl = new List<HandleVertex>();
            _LedgeHndl = new List<HandleEdge>();
            _LfaceHndl = new List<HandleFace>();
        }

        /// <summary>
        /// Loads an asset specified by the path
        /// </summary>
        /// <param name="path">Path to the wavefront file</param>
        public void LoadAsset(String path)
        {
            List<GeoFace> faceList = _objImporter.LoadAsset(path);

            // Work on the facelist and transform the data structure to the 'half-edge' data structure.
            foreach (GeoFace gf in faceList)
            {
                AddFace(gf);
            }

        }

        /// <summary>
        /// Adds a vertex to the geometry container. Can then be controlled by the kernel
        /// </summary>
        /// <param name="val"></param>
        public void AddVertex(float3 val)
        {
            _LverticeHndl.Add(
                    _GeometryContainer.AddVertex(val)
                );

            /*
            //TODO: Create Half Edge and Edges afterwars here!
            if (_LverticeHndl.Count > 1)
            {
                int hvFromID = _LverticeHndl.Count - 2;
                int hvToID = _LverticeHndl.Count - 1;
                AddEdge(_LverticeHndl[hvFromID], _LverticeHndl[hvToID]);
            }
             * */
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

            foreach (float3 vVal in gf._LFVertices)
            {
                AddVertex(vVal);
            }

            // Now create the hedges for the face, first the inner ones then the outer ones.
            // Here have to be two vertices. does not work otherwise :/
            // Best go through with for loop and count so i can set the second with an id.
            foreach(HandleVertex hv in _LverticeHndl)
            {
                _LedgeHndl.Add(
                    _GeometryContainer.AddEdge(hv)
                );

            }
            // Now don't forget the hedge the face points to, best the first one inserted from the edge handle list or something like that.
        }

    }
}
