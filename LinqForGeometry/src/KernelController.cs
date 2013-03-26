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

            List<HandleVertex> hFaceVerts = new List<HandleVertex>();
            foreach (float3 vVal in gf._LFVertices)
            {
                hFaceVerts.Add(
                        AddVertex(vVal)
                    );
            }


            List<HandleEdge> LtmpEdgesPerFace = new List<HandleEdge>();
            int vertsCount = hFaceVerts.Count;
            for (int i = 0; i < vertsCount; i++)
            {
                HandleVertex hvFrom = hFaceVerts[i];
                if (i + 1 < vertsCount)
                {
                    HandleVertex hvTo = hFaceVerts[i + 1];
                    HandleEdge handleEdge = _GeometryContainer.AddEdge(hvFrom, hvTo);
                    _LedgeHndl.Add(
                            handleEdge
                        );
                    LtmpEdgesPerFace.Add(
                            handleEdge
                        );
                }
                else
                {
                    // TODO: fix here - not add edges if they are already present?
                    HandleVertex hvTo = hFaceVerts[0];
                    HandleEdge handleEdge = _GeometryContainer.AddEdge(hvFrom, hvTo);
                    _LedgeHndl.Add(
                               handleEdge
                            );

                    LtmpEdgesPerFace.Add(
                            handleEdge
                        );
                }
            }

            // Update the face handle, so that it points to the first half edge the face consists of.
            _GeometryContainer.UpdateFaceToHedgePtr(LtmpEdgesPerFace[0]);
            foreach (HandleEdge handleEdge in LtmpEdgesPerFace)
            {
                _GeometryContainer.IsValidEdge(handleEdge);
            }

            _GeometryContainer.UpdateCWHedges(LtmpEdgesPerFace);
            _GeometryContainer.UpdateCCWHedges(LtmpEdgesPerFace);
        }

    }
}
