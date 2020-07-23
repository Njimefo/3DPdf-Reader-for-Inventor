using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Inventor;
using System.IO;
using System.Reflection;
using Addin3DPdf;
using Pdf = SimilaritySearch.Pdf3DReader;
using SimilaritySearch.Pdf3DReader;






namespace InvAddIn
{
    public class InventorSol

    {



        public static TransientBRep oTransBRep = null;


        public static SurfaceBodyDefinition oSurfaceBodyDef = null;

        public static TransientGeometry oTG = null;


        public static LumpDefinition oLumpDef = null;


        public static FaceShellDefinition oShell = null;

        // public static Application oApp = Addin3DPdf.TransAddInServer.MApp;
        public static Document inventordoc = null;
        public static int AnzahlderPartElemente = 1;


        private ClientGraphics mClientGraphics;
        private GraphicsDataSets mGraphicsDataSets;
        private GraphicsNode mGraphicsNode;






 

        static public void StandartTest()
        {

           // Set Inventor Application and "activate it"
            TrAddInServer.MApp = null;
            inventordoc = null;
            // Enable error handling.
           

            TransientBRep oTransBRep = TrAddInServer.MApp.TransientBRep;


            SurfaceBodyDefinition oSurfaceBodyDef = oTransBRep.CreateSurfaceBodyDefinition();

            TransientGeometry oTG = TrAddInServer.MApp.TransientGeometry;


            LumpDefinition oLumpDef = oSurfaceBodyDef.LumpDefinitions.Add();


            FaceShellDefinition oShell = oLumpDef.FaceShellDefinitions.Add();





            // Define the six planes of the box.
            Plane oPosX;
            Plane oNegX;
            Plane oPosY;
            Plane oNegY;
            Plane oPosZ;
            Plane oNegZ;
            oPosX = oTG.CreatePlane(oTG.CreatePoint(1, 0, 0), oTG.CreateVector(1, 0, 0));
            oNegX = oTG.CreatePlane(oTG.CreatePoint(-1, 0, 0), oTG.CreateVector(-1, 0, 0));
            oPosY = oTG.CreatePlane(oTG.CreatePoint(0, 1, 0), oTG.CreateVector(0, 1, 0));
            oNegY = oTG.CreatePlane(oTG.CreatePoint(0, -1, 0), oTG.CreateVector(0, -1, 0));
            oPosZ = oTG.CreatePlane(oTG.CreatePoint(0, 0, 1), oTG.CreateVector(0, 0, 1));
            oNegZ = oTG.CreatePlane(oTG.CreatePoint(0, 0, -1), oTG.CreateVector(0, 0, -1));

            // Create the six faces.
            FaceDefinition oFaceDefPosX;
            FaceDefinition oFaceDefNegX;
            FaceDefinition oFaceDefPosY;
            FaceDefinition oFaceDefNegY;
            FaceDefinition oFaceDefPosZ;
            FaceDefinition oFaceDefNegZ;
            oFaceDefPosX = oShell.FaceDefinitions.Add(oPosX, false);
            oFaceDefNegX = oShell.FaceDefinitions.Add(oNegX, false);
            oFaceDefPosY = oShell.FaceDefinitions.Add(oPosY, false);
            oFaceDefNegY = oShell.FaceDefinitions.Add(oNegY, false);
            oFaceDefPosZ = oShell.FaceDefinitions.Add(oPosZ, false);
            oFaceDefNegZ = oShell.FaceDefinitions.Add(oNegZ, false);

            // Create the vertices.
            VertexDefinition oVertex1;
            VertexDefinition oVertex2;
            VertexDefinition oVertex3;
            VertexDefinition oVertex4;
            VertexDefinition oVertex5;
            VertexDefinition oVertex6;
            VertexDefinition oVertex7;
            VertexDefinition oVertex8;
            oVertex1 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(1, 1, 1));
            oVertex2 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(1, 1, -1));
            oVertex3 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(-1, 1, -1));
            oVertex4 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(-1, 1, 1));
            oVertex5 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(1, -1, 1));
            oVertex6 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(1, -1, -1));
            oVertex7 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(-1, -1, -1));
            oVertex8 = oSurfaceBodyDef.VertexDefinitions.Add(oTG.CreatePoint(-1, -1, 1));

            // Define the edges at intersections of the defined planes.
            EdgeDefinition oEdgeDefPosXPosY;
            EdgeDefinition oEdgeDefPosXNegZ;
            EdgeDefinition oEdgeDefPosXNegY;
            EdgeDefinition oEdgeDefPosXPosZ;
            EdgeDefinition oEdgeDefNegXPosY;
            EdgeDefinition oEdgeDefNegXNegZ;
            EdgeDefinition oEdgeDefNegXNegY;
            EdgeDefinition oEdgeDefNegXPosZ;
            EdgeDefinition oEdgeDefPosYNegZ;
            EdgeDefinition oEdgeDefPosYPosZ;
            EdgeDefinition oEdgeDefNegYNegZ;
            EdgeDefinition oEdgeDefNegYPosZ;
            oEdgeDefPosXPosY = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex1, oVertex2, oTG.CreateLineSegment(oVertex1.Position, oVertex2.Position));
            oEdgeDefPosXNegZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex2, oVertex6, oTG.CreateLineSegment(oVertex2.Position, oVertex6.Position));
            oEdgeDefPosXNegY = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex6, oVertex5, oTG.CreateLineSegment(oVertex6.Position, oVertex5.Position));
            oEdgeDefPosXPosZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex5, oVertex1, oTG.CreateLineSegment(oVertex5.Position, oVertex1.Position));
            oEdgeDefNegXPosY = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex4, oVertex3, oTG.CreateLineSegment(oVertex4.Position, oVertex3.Position));
            oEdgeDefNegXNegZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex3, oVertex7, oTG.CreateLineSegment(oVertex3.Position, oVertex7.Position));
            oEdgeDefNegXNegY = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex7, oVertex8, oTG.CreateLineSegment(oVertex7.Position, oVertex8.Position));
            oEdgeDefNegXPosZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex8, oVertex4, oTG.CreateLineSegment(oVertex8.Position, oVertex4.Position));
            oEdgeDefPosYNegZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex2, oVertex3, oTG.CreateLineSegment(oVertex2.Position, oVertex3.Position));
            oEdgeDefPosYPosZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex4, oVertex1, oTG.CreateLineSegment(oVertex4.Position, oVertex1.Position));
            oEdgeDefNegYNegZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex7, oVertex6, oTG.CreateLineSegment(oVertex7.Position, oVertex6.Position));
            oEdgeDefNegYPosZ = oSurfaceBodyDef.EdgeDefinitions.Add(oVertex5, oVertex8, oTG.CreateLineSegment(oVertex5.Position, oVertex8.Position));

            // Define the loops on the faces.

            EdgeLoopDefinition oPosXLoop = oFaceDefPosX.EdgeLoopDefinitions.Add();
            oPosXLoop.EdgeUseDefinitions.Add(oEdgeDefPosXPosY, true);
            oPosXLoop.EdgeUseDefinitions.Add(oEdgeDefPosXNegZ, true);
            oPosXLoop.EdgeUseDefinitions.Add(oEdgeDefPosXNegY, true);
            oPosXLoop.EdgeUseDefinitions.Add(oEdgeDefPosXPosZ, true);


            EdgeLoopDefinition oNegXLoop = oFaceDefNegX.EdgeLoopDefinitions.Add();
            oNegXLoop.EdgeUseDefinitions.Add(oEdgeDefNegXPosY, false);
            oNegXLoop.EdgeUseDefinitions.Add(oEdgeDefNegXNegZ, false);
            oNegXLoop.EdgeUseDefinitions.Add(oEdgeDefNegXNegY, false);
            oNegXLoop.EdgeUseDefinitions.Add(oEdgeDefNegXPosZ, false);


            EdgeLoopDefinition oPosYLoop = oFaceDefPosY.EdgeLoopDefinitions.Add();
            oPosYLoop.EdgeUseDefinitions.Add(oEdgeDefPosXPosY, false);
            oPosYLoop.EdgeUseDefinitions.Add(oEdgeDefPosYNegZ, false);
            oPosYLoop.EdgeUseDefinitions.Add(oEdgeDefNegXPosY, true);
            oPosYLoop.EdgeUseDefinitions.Add(oEdgeDefPosYPosZ, false);


            EdgeLoopDefinition oNegYLoop = oFaceDefNegY.EdgeLoopDefinitions.Add();
            oNegYLoop.EdgeUseDefinitions.Add(oEdgeDefPosXNegY, false);
            oNegYLoop.EdgeUseDefinitions.Add(oEdgeDefNegYPosZ, false);
            oNegYLoop.EdgeUseDefinitions.Add(oEdgeDefNegXNegY, true);
            oNegYLoop.EdgeUseDefinitions.Add(oEdgeDefNegYNegZ, false);

            EdgeLoopDefinition oPosZLoop = oFaceDefPosZ.EdgeLoopDefinitions.Add();
            oPosZLoop.EdgeUseDefinitions.Add(oEdgeDefNegXPosZ, true);
            oPosZLoop.EdgeUseDefinitions.Add(oEdgeDefNegYPosZ, true);
            oPosZLoop.EdgeUseDefinitions.Add(oEdgeDefPosXPosZ, false);
            oPosZLoop.EdgeUseDefinitions.Add(oEdgeDefPosYPosZ, true);


            EdgeLoopDefinition oNegZLoop = oFaceDefNegZ.EdgeLoopDefinitions.Add();
            oNegZLoop.EdgeUseDefinitions.Add(oEdgeDefNegXNegZ, true);
            oNegZLoop.EdgeUseDefinitions.Add(oEdgeDefNegYNegZ, true);
            oNegZLoop.EdgeUseDefinitions.Add(oEdgeDefPosXNegZ, false);
            oNegZLoop.EdgeUseDefinitions.Add(oEdgeDefPosYNegZ, true);

            //Create a transient surface body.
            NameValueMap oErrors;

            SurfaceBody oNewBody = oSurfaceBodyDef.CreateTransientSurfaceBody(out oErrors);

            // Create client graphics to display the transient body.

            PartDocument oDoc = (PartDocument)TrAddInServer.MApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject);


            PartComponentDefinition oDef = oDoc.ComponentDefinition;


            ClientGraphics oClientGraphics = oDef.ClientGraphicsCollection.Add("Sample3DGraphicsID");

            // Create a new graphics node within the client graphics objects.
            GraphicsNode oSurfacesNode = oClientGraphics.AddNode(1);

            // Create client graphics based on the transient body
            SurfaceGraphics oSurfaceGraphics = oSurfacesNode.AddSurfaceGraphics(oNewBody);

            // Update the view.
            TrAddInServer.MApp.ActiveView.Update();











        }



        /// <summary>
        /// mit der methode ist es möglich ein Bauteil (PART) als Sammlung von Linien in Inventor abzubilden...
        /// </summary>
        /// <param name="Bauteil"></param>
        /// 


        private List<double> mVertices = new List<double>();
        private List<int> mIndices = new List<int>();
        private List<double> mNormals = new List<double>();





        public static List<string> PartsName = new List<string>();
        long i = 0;
        public void Draw3D(Part Bauteil, string Name)
        {






            foreach (var mmface in Bauteil.Faces)
            {

                foreach (var point in mmface.VertexCoords)
                {
                    mVertices.Add((double)point.X);
                    mVertices.Add((double)point.Y);
                    mVertices.Add((double)point.Z);

                }

                var N = mmface.VertexIndices.Count; //  Nomber of Indices
                foreach (var indice in mmface.VertexIndices)
                {

                    var i0 = indice;
                    var i1 = i0 + N;

                    var t0 = i0;
                    var t1 = i1;

                    // NOTE: The extra +1 on each line is because Inventor expects 1 - based indices.

                    mIndices.Add(t1 + 1 + 1);
                    mIndices.Add(t0 + 1);



                    mIndices.Add(t0 + 1 + 1);
                    //  mIndices.Add(t1 + 1 + 1);
                    //  mIndices.Add(t0 + 1);

                    mIndices.Add(t1 + 1);







                }
                foreach (var Norm in mmface.Normals)
                {
                    mNormals.Add((double)Norm.X);
                    mNormals.Add((double)Norm.Y);
                    mNormals.Add((double)Norm.Z);


                }
            }









            if (null != mGraphicsDataSets)
            {
                mGraphicsDataSets.Delete();
                mGraphicsDataSets = null;

                mGraphicsNode.Delete();
                mGraphicsNode = null;

                Addin3DPdf.TrAddInServer.MApp.ActiveView.Update();
                return;
            }

            PartDocument
                doc = Addin3DPdf.TrAddInServer.MApp.Documents.Add(DocumentTypeEnum.kPartDocumentObject,
                    Addin3DPdf.TrAddInServer.MApp.FileManager.GetTemplateFile(DocumentTypeEnum.kPartDocumentObject),
                    true) as PartDocument;

            //    PartDocument doc = Addin3DPdf.TransAddInServer.MApp.ActiveDocument as PartDocument;
            if (null == mClientGraphics)
            {
                mClientGraphics = doc.ComponentDefinition.ClientGraphicsCollection.Add(Name);
            }

            mGraphicsDataSets = doc.GraphicsDataSetsCollection.Add(Name);





            double[] A_Verties = mVertices.ToArray();
            int[] A_Indices = mIndices.ToArray();
            double[] A_Normals = mNormals.ToArray();
            try
            {


                var gcs = mGraphicsDataSets.CreateCoordinateSet(1) as GraphicsCoordinateSet;
                gcs.PutCoordinates(A_Verties);



                var gis = mGraphicsDataSets.CreateIndexSet(2) as GraphicsIndexSet;
                gis.PutIndices(A_Indices);



                var gns = mGraphicsDataSets.CreateNormalSet(3) as GraphicsNormalSet;
                gns.PutNormals(A_Normals);


                mGraphicsNode = mClientGraphics.AddNode(1) as GraphicsNode;
                var triangles = mGraphicsNode.AddTriangleGraphics() as TriangleGraphics;

                triangles.CoordinateSet = gcs;
                triangles.CoordinateIndexSet = gis;
                triangles.NormalSet = gns;
                triangles.NormalBinding = NormalBindingEnum.kPerItemNormals;
                triangles.NormalIndexSet = gis;



                bool inActiveDocument;
                var brassAsset = GetAsset("Silver", out inActiveDocument);
                if (null != brassAsset)
                {
                    if (!inActiveDocument)
                    {
                        brassAsset = brassAsset.CopyTo(doc);
                    }
                    mGraphicsNode.Appearance = brassAsset;
                }



            }
            catch (Exception e)
            {

            }
           doc.SaveAs(Name, false);

            Addin3DPdf.TrAddInServer.MApp.ActiveView.Update();



        }
        private Asset GetAsset(string partialName, out bool inActiveDocument)
        {
            inActiveDocument = false;
            var upperPartialName = partialName.ToUpper();

            // Look in the active document first.
            var partDoc = Addin3DPdf.TrAddInServer.MApp.ActiveDocument as PartDocument;
            if (null != partDoc)
            {
                foreach (Asset asset in partDoc.AppearanceAssets)
                {
                    if (asset.DisplayName.ToUpper().Contains(upperPartialName))
                    {
                        inActiveDocument = true;
                        return asset;
                    }
                }
            }

            foreach (AssetLibrary assetLib in Addin3DPdf.TrAddInServer.MApp.AssetLibraries)
            {
                foreach (Asset asset in assetLib.AppearanceAssets)
                {
                    if (asset.DisplayName.ToUpper().Contains(upperPartialName))
                    {
                        return asset;
                    }
                }
            }

            return null;
        }




        List<Element3D> getAllelement(object o)  //must be implement
        {
            List<Element3D> allElements = null;
            try
            {
                //   reader.ReadPdf3D(O as Part, out allElements);
            }
            catch (Exception)
            {

                //   reader.ReadPdf3D(O as Assembly, out allElements);
            }

            return allElements;
        }






        public void DrawAssembly(string PdfFile)
        {

            AssemblyDocument oDoc = (AssemblyDocument)Addin3DPdf.TrAddInServer.MApp.ActiveDocument;

            // Set a reference to the assembly component definition.
            AssemblyComponentDefinition oAsmCompDef = oDoc.ComponentDefinition;

            // Set a reference to the transient geometry object.
            TransientGeometry oTG = Addin3DPdf.TrAddInServer.MApp.TransientGeometry;

            string A = ".iam";
            string P = ".ipt";
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                List<string> Olist = new List<string>();
                reader.ReadPdf3D(PdfFile, out allElements);

                for (int i = 0; i < allElements.Count; i++)
                {
                    try
                    {

                        Draw3D((Part)allElements[i], "Part" + i + P);
                        Olist.Add("Part" + i + P);

                    }

                    //if (allElements[i].Type.Equals(Element3DType._Assembly))



                    catch (Exception)
                    {
                        //Get Parts of Assembly
                        //Draw Parts
                        //Make Assemby of Parts
                        List<Element3D> allElements1 = getAllelement(allElements[i]);
                        List<string> Assemblylist1 = new List<string>();  // list of Assemblies     / May be a Dictionary with the Matrix...!!!!
                        for (int j = 0; j < allElements1.Count; j++)
                        {

                            // if (allElements[j].Type.Equals(Element3DType._Assembly))
                            try
                            {




                                Draw3D((Part)allElements[j], "Part" + i + "_" + j + P);
                                Assemblylist1.Add("Part" + i + "_" + j + P);

                            }
                            catch (Exception)
                            {
                                //Get Parts of Assembly
                                //Draw Parts
                                //Make Assemby of Parts
                                List<Element3D> allElements2 = getAllelement(allElements[j]);
                                List<string> Assemblylist2 = new List<string>();  // list of Assemblies
                                for (int k = 0; k < allElements2.Count; k++)
                                {

                                    //  if (allElements[k].Type.Equals(Element3DType._Assembly))
                                    try
                                    {

                                        Draw3D((Part)allElements[k], "Part" + i + "_" + j + "_" + k + P);
                                        Assemblylist2.Add("Part" + i + "_" + j + "_" + k + P);

                                    }
                                    catch (Exception)
                                    {

                                        //Get Parts of Assembly
                                        //Draw Parts
                                        //Make Assemby of Parts
                                        List<Element3D> allElements3 = getAllelement(allElements[k]);
                                        List<string> Assemblylist3 = new List<string>(); // list of Assemblies
                                        for (int l = 0; l < allElements3.Count; l++)
                                        {
                                            //if (allElements[l].Type.Equals(Element3DType._Assembly))
                                            try
                                            {

                                                Draw3D((Part)allElements[l],
                                                    "Part" + i + "_" + j + "_" + k + "_" + l + P);
                                                //Save part
                                                Assemblylist3.Add("Part" + i + "_" + j + "_" + k + "_" + l + P);
                                            }
                                            catch (Exception)
                                            {
                                                /// Stop or End of occurenses


                                            }

                                        }


                                        // Make a joint ofAssemblylist3

                                       
                                        // save a Occurence to a Name // "Assembly" + i + "_" + j+"_"+k
                                        Assemblylist2.Add("Assembly" + i + "_" + j + "_" + k + A);

                                    }



                                }

                                // Make a joint Assemblylist2
                                // save a Occurence to a Name // "Assembly" + i+"_"+j
                                Assemblylist1.Add("Assembly" + i + "_" + j + A);
                            }





                        }




                        // Make a joint of Assemblylist1
                        // save a Occurence to a Name // "Assembly" + i
                        Olist.Add("Assembly" + i + A);




                    }


                }



            }

            // Make a joint of Olist

            //save Document

        }




        public void joint3DElement(Dictionary<string,Matrix> Objs, string Name)
        {

            foreach (var Occ in Objs)
            {


                AssemblyDocument asmDoc =
                    (AssemblyDocument) Addin3DPdf.TrAddInServer.MApp.Documents.Add
                    (DocumentTypeEnum.kAssemblyDocumentObject,
                        Addin3DPdf.TrAddInServer.MApp.FileManager.GetTemplateFile
                            (DocumentTypeEnum.kAssemblyDocumentObject));

                AssemblyComponentDefinition asmDef =
                    asmDoc.ComponentDefinition;

                Matrix trans =
                    Addin3DPdf.TrAddInServer.MApp.TransientGeometry.CreateMatrix();



                ComponentOccurrence occ1 =asmDef.Occurrences.Add(Occ.Key,Occ.Value);


            }

        }






        public void AddOccurrence(string OccName, Matrix Position)
        {
            Application oApp = Addin3DPdf.TrAddInServer.MApp;

            // Set a reference to the active assembly document.



            //// Create a matrix.  A new matrix is initialized with an identity matrix.
            //Matrix oMatrix = oTG.CreateMatrix();

            //// Set the rotation of the matrix for a 45 degree rotation about the Z axis.
            //oMatrix.SetToRotation(3.14159265358979 / 4,
            //    oTG.CreateVector(0, 0, 1), oTG.CreatePoint(0, 0, 0));

            //// Set the translation portion of the matrix so the part will be positioned
            //// at (3,2,1).
            //oMatrix.SetTranslation(oTG.CreateVector(3, 2, 1), true);

            // Add the occurrence.
            //   ComponentOccurrence oOcc = oAsmCompDef.Occurrences.Add(OccName, Position);

        }


    }
}
