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
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Fusee.Math;

namespace hsfurtwangen.dsteffen.lfg.Importer
{
    /// <summary>
    /// This is an importer for the wavefront '.obj' computer graphics file
    /// To use it just create an instance and pass the file path to the LoadAsset() method
    /// </summary>
    class WavefrontImporter<VertexType>
    {
        // Sytem File related
        String _SassetFileContent;
        String[] _SendOfLine = { "\n" };

        // Geometry related
        internal List<GeoFace> _LgeoFaces;
        internal List<VertexType> _LvertsTest;

        public WavefrontImporter()
        {
            // File related
            _SassetFileContent = "";

            // Geometry related
            _LgeoFaces = new List<GeoFace>();
            _LvertsTest = new List<VertexType>();
        }

        /// <summary>
        /// This loads a file to the importer object
        /// </summary>
        /// <param name="pathToAsset">The path to the asset to be loaded in the system</param>
        private List<String> LoadFile(String pathToAsset)
        {
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
            // Clear the content before the import
            _SassetFileContent = "";

            // Helper Lists
            List<String> LvertexHelper = new List<string>();
            List<String> LfaceHelper = new List<string>();

            // String related
            String[] splitChar = { " " };
            String[] splitChar2 = { "/" };

            // Load the file information to the memory
            List<String> LgeoValues = LoadFile(pathToAsset);
            if (LgeoValues != null)
            {
                List<float3> LvertexAttr = new List<float3>();

                foreach (String line in LgeoValues)
                {
                    string lineStart = line.Substring(0, 2);

                    if (lineStart.Equals("v "))
                    {
                        // vertex
                        string[] lineSplitted = line.Split(splitChar, StringSplitOptions.None);
                        List<Double> tmpSave = new List<double>();
                        foreach (string str in lineSplitted)
                        {
                            if (!str.StartsWith("v"))
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
                        // vertex texture offset
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
                        // face
                        // there are faces, faces with texture coord, faces with vertex normals and faces with text and normals
                        Console.WriteLine("Face found, values: " + line);
                        string[] lineSplitted = line.Split(splitChar, StringSplitOptions.None);
                        List<Double> tmpSave = new List<double>();

                        GeoFace geoF = new GeoFace();
                        geoF._LFVertices = new List<float3>();
                        foreach (string str in lineSplitted)
                        {
                            if (!str.StartsWith("f"))
                            {
                                string[] faceSplit = str.Split(splitChar2, StringSplitOptions.None);
                                string s = faceSplit[0];
                                Console.WriteLine("Here is s: " + s);
                                if (s != null || s != "" || !s.Equals("") || !s.Equals(" ") || s != " " || !s.Equals("\n") || s != "\n" || s != "\0" || !s.Equals("\0") || !s.Equals("\r") || s != "\r")
                                {
                                    try
                                    {
                                        int fv = int.Parse(s, CultureInfo.InvariantCulture);
                                        geoF._LFVertices.Add(LvertexAttr[fv - 1]);
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("This is not a valid value: " + s);
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

            if (_LgeoFaces != null)
            {
                return _LgeoFaces;
            }
            else
            {
                return null;
            }
        }
    }
}