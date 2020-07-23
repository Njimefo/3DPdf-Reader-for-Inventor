using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimilaritySearch.Pdf3DReader;
using QuantumConcepts.Formats.StereoLithography;

namespace SimilaritySearch.Visualization
{
    public class StlConverter
    {


        public Dictionary<int, object> occurenceRef { get; private set; }

        public bool IsPart { get; private set; }

        //AppDomain.CurrentDomain.BaseDirectory
        public StlConverter(Element3D ojbect3D, string Name)
        {
            if ((Element3DType)ojbect3D.Type == Element3DType._Assembly)
            {
                AssemblyDefinition extractedAss = (AssemblyDefinition)ojbect3D;
                int position = 0;
                string FolderPath = Path.GetTempPath() + "\\" + Name;
                Directory.CreateDirectory(FolderPath);
                occurenceRef = new Dictionary<int, object>();
                foreach (Occurence occ in extractedAss.Occurences)
                {
                    if (occ.PartIsReferenced)
                    {
                        WritePartAsSTL(occ.referencedPart, Name, FolderPath + "\\" + Name + "_" + position + ".stl");
                        occurenceRef.Add(position,position);

                    }
                    else
                    {
                        Dictionary<int, object> sub_Elt_Ref = new Dictionary<int, object>();
                        WriteOccurencesAsStl(occ, ref position, Name, FolderPath, ref sub_Elt_Ref);
                        //position += 1;
                        occurenceRef.Add(position, sub_Elt_Ref);
                    }

                    position += 1;
                }

            }
            else if ((Element3DType)ojbect3D.Type == Element3DType._Part)
            {


                WritePartAsSTL((Part)ojbect3D, Name);

            }


        }

        void WritePartAsSTL(Part newPart, string Name, string path = null)
        {
            List<Facet> facets = new List<Facet>();
            foreach (Face face in newPart.Faces)
            {


                int counter = 0;
                int normalCounter = 0;
                List<Vertex> vertices = new List<Vertex>();
                foreach (Point3D point3D in face.VertexCoords)
                {
                    if (counter == 0 || counter == 1)
                    {

                        vertices.Add(new Vertex((float)((double)point3D.X), (float)((double)point3D.Y),
                            (float)((double)point3D.Z)));
                        counter += 1;
                    }
                    else if (counter == 2)
                    {
                        vertices.Add(new Vertex((float)((double)point3D.X), (float)((double)point3D.Y),
                            (float)((double)point3D.Z)));
                        Normal newNormal = new Normal((float)((double)face.Normals[normalCounter].X),
                            (float)((double)face.Normals[normalCounter].Y),
                            (float)((double)face.Normals[normalCounter].Z));
                        Facet newFacet = new Facet(newNormal, vertices, 0);
                        facets.Add(newFacet);
                        vertices = new List<Vertex>();
                        normalCounter += 1;
                        counter = 0;
                    }
                }

            }
            STLDocument newStlFile = new STLDocument(Name, facets);

            string pfad = path ?? Path.Combine(Path.GetTempPath(), Name + ".stl");
            newStlFile.SaveAsText(pfad);
        }

        void WriteOccurencesAsStl(Occurence occ, ref int position, string Name, string FolderPath, ref Dictionary<int, object> subEle)
        {
            foreach (var Occ in occ.referencedAssemblyDefinition.Occurences)
            {


                if (Occ.PartIsReferenced)
                {
                    WritePartAsSTL(Occ.referencedPart, Name, FolderPath + "\\" + Name + "_" + position + ".stl");
                    subEle.Add(position, position);

                }
                else
                {
                    Dictionary<int, object> sub_Elt_Ref = new Dictionary<int, object>();
                    WriteOccurencesAsStl(Occ, ref position, Name, FolderPath, ref sub_Elt_Ref);
                    position += 1;
                    subEle.Add(position,sub_Elt_Ref);

                }

                position += 1;

            }
        }
    }
}
