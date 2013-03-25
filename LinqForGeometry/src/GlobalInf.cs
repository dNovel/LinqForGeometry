﻿/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachlor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System;

namespace hsfurtwangen.dsteffen.lfg.globalinf
{
    /// <summary>
    /// This class just handles some messages i use to generate output in the program.
    /// It's just easier to handle the messages this way than searching for all strings to change.
    /// </summary>
    static class LFGMessages
    {
        // Keywords
        private static string _INFO = "INFO: ";
        private static string _WARNING = "WARNING: ";
        private static string _ERROR = "ERROR: ";

        // Messages used for text output
        public static string WARNING_INVALIDCHAR = _WARNING + "Invalid character in .obj file found.";
        public static string WARNING_INVALIDCASE = _WARNING + "Invalid case in control sequence";

        public static string INFO_VERTEXIDFORFACE = _INFO + "Vertex ID connected to current face: ";
        public static string INFO_FACEFOUND = _INFO + "Face found in importer, values: ";

        public static string UTIL_STOPWFORMAT = "{0:00}sec, {1:00}ms";
    }
}