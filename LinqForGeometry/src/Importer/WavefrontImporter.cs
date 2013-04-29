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
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Fusee.Math;
using hsfurtwangen.dsteffen.lfg.globalinf;

namespace hsfurtwangen.dsteffen.lfg.Importer
{
    /// <summary>
    /// This is an importer for the <a href="http://en.wikipedia.org/wiki/Wavefront_.obj_file">Wavefront obj</a> computer graphics file
    /// To use it just create an instance and pass the file path to the LoadAsset() method
    /// </summary>
    class WavefrontImporter<VertexType>
    {
        // Sytem File related
        String _SassetFileContent;
        String[] _SendOfLine = { "\n" };

        // GeometryData related
        internal List<GeoFace> _LgeoFaces;
        internal List<float2> _LuvCoords;
        public List<KeyValuePair<int, int>> _LKVuvandvert;

        public WavefrontImporter()
        {
            // File related
            _SassetFileContent = "";

            // GeometryData related
            _LgeoFaces = new List<GeoFace>();
            _LuvCoords = new List<float2>();
            _LKVuvandvert = new List<KeyValuePair<int, int>>();
        }

        /// <summary>
        /// This loads a file to the importer object
        /// </summary>
        /// <param name="pathToAsset">The path to the asset to be loaded in the system</param>
        private List<String> LoadFile(String pathToAsset)
        {
            Console.WriteLine(globalinf.LFGMessages.INFO_IMPORTERDISCLAIMER);
            if (File.Exists(pathToAsset))
            {
                StreamReader assetFile = new StreamReader(pathToAsset, System.Text.Encoding.Default);
                _SassetFileContent = assetFile.ReadToEnd();
                assetFile.Close();
                string[] contentAsLines = _SassetFileContent.Split(_SendOfLine, StringSplitOptions.RemoveEmptyEntries);

                List<String> LitemList = new List<string>();
                foreach (string line in contentAsLines)
                {
                    if (!line.StartsWith("#"))
                    {
                        LitemList.Add(line);
                    }
                }
                return LitemList;
            }
            return null;
        }

        /// <summary>
        /// Loads an asset to the kernel
        /// </summary>
        /// <param name="pathToAsset"></param>
        public List<GeoFace> LoadAsset(String pathToAsset)
        {
            _SassetFileContent = "";

            List<String> LvertexHelper = new List<string>();
            List<String> LfaceHelper = new List<string>();

            String[] splitChar = { " " };
            String[] splitChar2 = { "/" };

            // Load the file information to the memory
            List<String> LgeoValues = LoadFile(pathToAsset);
            if (LgeoValues != null)
            {
                List<float3> LvertexAttr = new List<float3>();

                foreach (String line in LgeoValues)
                {
                    string lineStart = line.Length > 2 ? line.Substring(0, 2) : line.Substring(0, line.Length);

                    if (lineStart.Equals("v "))
                    {
                        // vertex
                        string[] lineSplitted = line.Split(splitChar, StringSplitOptions.None);
                        List<Double> tmpSave = new List<double>();
                        foreach (string str in lineSplitted)
                        {
                            if (!str.StartsWith("v") && !str.Equals(""))
                            {
                                tmpSave.Add(float.Parse(str, CultureInfo.InvariantCulture));
                            }
                        }
                        float3 fVal = new float3(
                                (float)tmpSave[0],
                                (float)tmpSave[1],
                                (float)tmpSave[2]
                        );
                        LvertexAttr.Add(fVal);
                    }
                    else if (lineStart.Equals("vt"))
                    {
                        string[] lineSplitted = line.Split(splitChar, StringSplitOptions.None);

                        // vertex texture coordinates
                        if (globalinf.LFGMessages._DEBUGOUTPUT)
                        {
                            Console.WriteLine(LFGMessages.INFO_UVFOUND + line);
                        }

                        List<Double> tmpSave = new List<double>();
                        foreach (string str in lineSplitted)
                        {
                            if (!str.StartsWith("vt") && !str.Equals(""))
                            {
                                tmpSave.Add(float.Parse(str, CultureInfo.InvariantCulture));
                            }
                        }
                        float2 uvVal = new float2(
                            (float)tmpSave[0],
                            (float)tmpSave[1]
                            );
                        _LuvCoords.Add(uvVal);
                    }
                    else if (lineStart.Equals("vn"))
                    {
                        // vertex normals
                    }
                    else if (lineStart.Equals("p "))
                    {
                        // point
                    }
                    else if (lineStart.Equals("l "))
                    {
                        // line
                    }
                    else if (lineStart.Equals("f "))
                    {
                        // there are faces, faces with texture coord, faces with vertex normals and faces with text and normals
                        if (globalinf.LFGMessages._DEBUGOUTPUT)
                        {
                            Console.WriteLine(LFGMessages.INFO_FACEFOUND + line);
                        }
                        string[] lineSplitted = line.Split(splitChar, StringSplitOptions.None);
                        List<Double> tmpSave = new List<double>();

                        GeoFace geoF = new GeoFace();
                        geoF._LFVertices = new List<float3>();
                        geoF._UV = new List<float2>();
                        foreach (string str in lineSplitted)
                        {
                            if (!str.StartsWith("f"))
                            {
                                string[] faceSplit = str.Split(splitChar2, StringSplitOptions.None);
                                string s = faceSplit[0];

                                if (globalinf.LFGMessages._DEBUGOUTPUT)
                                {
                                    Console.WriteLine(LFGMessages.INFO_VERTEXIDFORFACE + s);
                                }
                                if (s != null || s != "" || !s.Equals("") || !s.Equals(" ") || s != " " || !s.Equals("\n") || s != "\n" || s != "\0" || !s.Equals("\0") || !s.Equals("\r") || s != "\r")
                                {
                                    try
                                    {
                                        int fv = int.Parse(s, CultureInfo.InvariantCulture);
                                        geoF._LFVertices.Add(LvertexAttr[fv - 1]);

                                        if (faceSplit.Length >= 1)
                                        {
                                            string uvIndex = faceSplit[1];
                                            int uvAdress = int.Parse(uvIndex, CultureInfo.InvariantCulture);
                                            geoF._UV.Add(_LuvCoords[uvAdress - 1]);
                                            _LKVuvandvert.Add(new KeyValuePair<int, int>(uvAdress - 1, fv - 1));
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        if (globalinf.LFGMessages._DEBUGOUTPUT)
                                        {
                                            Console.WriteLine(LFGMessages.WARNING_INVALIDCHAR + s);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                        _LgeoFaces.Add(geoF);
                    }
                    else if (lineStart.Equals("g "))
                    {
                        // group
                    }
                    else if (lineStart.Equals("usemtl"))
                    {
                        // use material
                    }
                    else if (lineStart.Equals("mtllib"))
                    {
                        // material lib
                    }
                }
            }


            // Clear the content after the import is done
            _SassetFileContent = "";

            if (_LgeoFaces != null) {
                return _LgeoFaces;
            }
            else
            {
                return null;
            }
        }
    }
}
