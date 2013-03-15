using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fusee.Math;
using hsfurtwangen.dsteffen.lfg;
using hsfurtwangen.dsteffen.lfg.structs.handles;

namespace hsfurtwangen.dsteffen.lfg
{
    class KernelController<VertexType, EdgeType, FaceType>
    {
        Geometry<VertexType, EdgeType, FaceType> geo;

        List<HandleVertex> _LverticeHndl;
        List<HandleEdge> _LedgeHndl;
        List<FaceType> _LfaceHndl;

        public KernelController()
        {
            geo = new Geometry<VertexType, EdgeType, FaceType>();
            _LverticeHndl = new List<HandleVertex>();
            _LedgeHndl = new List<HandleEdge>();
            _LfaceHndl = new List<FaceType>();
        }

        /// <summary>
        /// Adds a vertex to the geometry container. Can then be controlled by the kernel
        /// </summary>
        /// <param name="val"></param>
        public void AddVertex(VertexType val)
        {
            _LverticeHndl.Add(
                    geo.AddVertex(val)
                );

            //TODO: Create Half Edge and Edges afterwars here!
            if (_LverticeHndl.Count > 1)
            {
                int hvFromID = _LverticeHndl.Count - 2;
                int hvToID = _LverticeHndl.Count - 1;
                AddEdge(_LverticeHndl[hvFromID], _LverticeHndl[hvToID]);
            }
        }

        /// <summary>
        /// Adds an edge to the geometry container. Can then be controlled by the kernel
        /// </summary>
        /// <param name="hv1">From Vertex</param>
        /// <param name="hv2">To Vertex</param>
        public void AddEdge(HandleVertex hvFrom, HandleVertex hvTo)
        {
            // TODO: First ask if there is already a connection to prevent redundant data
            if (!geo.GetConnection(hvFrom, hvTo))
            {
                _LedgeHndl.Add(
                        geo.AddEdge(hvFrom, hvTo)
                    );
            }
        }

    }
}
