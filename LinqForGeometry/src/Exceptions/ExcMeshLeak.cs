﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hsfurtwangen.dsteffen.lfg.Exceptions
{
    class MeshLeakException : Exception {
        private const String _defMsg = "The mesh provided to the importer is leaked (Holes in structure). Please double check the mesh for integrity";

        public MeshLeakException(string msg = _defMsg)
        {
        }
    }
}
