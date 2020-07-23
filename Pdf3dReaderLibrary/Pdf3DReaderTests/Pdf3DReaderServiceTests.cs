using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using SimilaritySearch.Pdf3DReader;


namespace SimilaritySearch.Pdf3DReaderTests
{

    public enum LastReadElement
    {
        Nichts,
        Occurence,
        Occurences,
        Normals,
        Vertices,
        Indices,
        Point,
        Face
    }
    [TestFixture]
    public class Pdf3DReaderServiceTests
    {


        [Test]
        public void TestInitialization()
        {
            using (var reader = new Pdf3DReader.Pdf3DReaderService())
            {
                // First initialization
                Assert.IsTrue(reader.Init());

                // ...and second one
                Assert.IsTrue(reader.Init());
            }
        }

        [Test]
        public void TestReadPdf3DWasher()
        {
            //var fileName = @"..\..\..\..\Data\NP2751.pdf";


            var fileName = @"..\..\..\..\..\Data\MP1914.pdf";//@"..\..\..\..\Data\simple_cube.prc";

            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
                               fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {


                List<Element3D> allElements = null;
                Assert.AreEqual(0, reader.ReadPdf3D(fileName, out allElements));
                Assert.AreEqual(1, allElements.Count);
                Assert.AreEqual(allElements[0].Type, Element3DType._Part);
                Part extractedPart = (Part)allElements[0];
                string modelFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Washer" + ".xml";
                Part modelPart = (Part)extractElement3Ds(modelFile)[0];
                Assert.AreEqual(extractedPart.Faces.Count, modelPart.Faces.Count);
                for (int i = 0; i < modelPart.Faces.Count; i++)
                {
                    Face extractedFace = extractedPart.Faces[i];
                    Face modelFace = modelPart.Faces[i];
                    Assert.AreEqual(modelFace.Normals.Count, extractedFace.Normals.Count);
                    Assert.AreEqual(modelFace.VertexCoords.Count, extractedFace.VertexCoords.Count);
                    Assert.AreEqual(modelFace.VertexIndices.Count, extractedFace.VertexIndices.Count);
                    for (int j = 0; j < modelFace.Normals.Count; j++)
                    {
                        Point3D extractedNormalPoint = extractedFace.Normals[j];
                        Point3D modelNormalPoint = modelFace.Normals[j];


                        Assert.AreEqual(Math.Round((double)extractedNormalPoint.X, 7), Math.Round((double)modelNormalPoint.X, 7));
                        Assert.AreEqual(Math.Round((double)extractedNormalPoint.Y, 7), Math.Round((double)modelNormalPoint.Y, 7));
                        Assert.AreEqual(Math.Round((double)extractedNormalPoint.Z, 7), Math.Round((double)modelNormalPoint.Z, 7));

                    }

                    for (int k = 0; k < modelFace.VertexCoords.Count; k++)
                    {

                        Point3D extractionvertrexpoint = extractedFace.VertexCoords[k];
                        Point3D modelvertrexpoint = extractedFace.VertexCoords[k];
                        Assert.AreEqual(Math.Round((double)extractionvertrexpoint.X, 7), Math.Round((double)modelvertrexpoint.X, 7));
                        Assert.AreEqual(Math.Round((double)extractionvertrexpoint.Y, 7), Math.Round((double)modelvertrexpoint.Y, 7));
                        Assert.AreEqual(Math.Round((double)extractionvertrexpoint.Z, 7), Math.Round((double)modelvertrexpoint.Z, 7));

                    }
                    for (int l = 0; l < modelFace.VertexIndices.Count; l++)
                    {
                        int extractionindicevertex = extractedFace.VertexIndices[l];
                        int modelindicevertex = modelFace.VertexIndices[l];
                        Assert.AreEqual(extractionindicevertex, modelindicevertex);
                    }

                }
            }
        }

        [Test]
        public void TestReadPdf3DCube()
        {
            var fileName = @"..\..\..\..\..\Data\186455.pdf";//@"..\..\..\..\Data\simple_cube.prc";
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                Assert.AreEqual(0, reader.ReadPdf3D(fileName, out allElements));
                Assert.AreEqual(1, allElements.Count);
                Assert.AreEqual(allElements[0].Type, Element3DType._Part);
                Part extractedPart = (Part)allElements[0];
                string modelFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Cube" + ".xml";
                Part modelPart = (Part)extractElement3Ds(modelFile)[0];
                CompareParts(extractedPart, modelPart);
            }
        }
        // hier wollen wir die dateien von fahradteil_pdf speichern
        [Test]
        public void TestReadPdf3Dfahreadteil()
        {
            //var fileName = @"..\..\..\..\Data\NP2751.pdf";
            var fileName = @"..\..\..\..\Data\Fahrradteilelement.pdf";/*"..\..\..\..\Data\BP0370.pdf";*/

            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                Assert.AreEqual(0, reader.ReadPdf3D(fileName, out allElements));
                Assert.AreNotEqual(allElements, null);
                Assert.AreNotEqual(allElements.Count, 0);
                Assert.AreEqual(allElements[0].Type, Element3DType._Assembly);
                AssemblyDefinition extractedAssembly = (AssemblyDefinition)allElements[0];
                AssemblyDefinition modelAssembly = ExtractAssembly("Fahrradteilelement");
                Assert.AreNotEqual(modelAssembly, null);
                CompareTwoAssembly(extractedAssembly, modelAssembly);
            }
        }

        AssemblyDefinition ExtractAssembly(string name)
        {
            string folderPfad = AppDomain.CurrentDomain.BaseDirectory + "\\" + name;
            if (!Directory.Exists(folderPfad))
                throw new NullReferenceException("Folder does not exist");
            string path = folderPfad + "\\" + name + "_Main.xml";
            AssemblyDefinition assembly = null;


            if (!File.Exists(path)) throw new Exception("File does not exist ");




            XmlTextReader read = new XmlTextReader(path);
            if (read.ReadState == ReadState.Error) throw new Exception("File could not be read");
            else
            {
                try
                {

                    List<Occurence> allOccurences = new List<Occurence>();

                    while (read.Read())
                    {
                        //   tests if the current content node is a start tag or empty element tag.
                        if (read.IsStartElement("Occurence"))
                        {
                            int occurence_Position = int.Parse(read.ReadString());
                            string newPath = folderPfad + "\\Occurence_" + occurence_Position + ".xml";

                            Occurence occ = GetOccurence(newPath, folderPfad);
                            allOccurences.Add(occ);
                        }
                    }
                    assembly = new AssemblyDefinition(allOccurences);
                }
                catch

                {

                }
            }

            return assembly;
        }

        //[Test]
        void CompareTwoAssembly(AssemblyDefinition Ass1, AssemblyDefinition Ass2)
        {
            Assert.AreEqual(Ass1.Occurences.Count, Ass2.Occurences.Count);

            for (int i = 0; i < Ass1.Occurences.Count; i++)
            {
                Occurence occ1 = Ass1.Occurences[i];
                Occurence occ2 = Ass2.Occurences[i];
                Assert.AreEqual(occ1.PartIsReferenced, occ2.PartIsReferenced);
                if (occ1.PartIsReferenced && occ2.PartIsReferenced)
                {

                    CompareParts(occ1.referencedPart, occ2.referencedPart);
                }
                else if (!occ1.PartIsReferenced && !occ2.PartIsReferenced)
                {
                    CompareTwoAssembly(occ1.referencedAssemblyDefinition, occ2.referencedAssemblyDefinition);
                }

            }


        }

        //[Test]
        void CompareParts(Part modelPart, Part extractedPart)
        {
            Assert.AreEqual(extractedPart.Faces.Count, modelPart.Faces.Count);
            for (int i = 0; i < modelPart.Faces.Count; i++)
            {
                Face extractedFace = extractedPart.Faces[i];
                Face modelFace = modelPart.Faces[i];
                Assert.AreEqual(modelFace.Normals.Count, extractedFace.Normals.Count);
                Assert.AreEqual(modelFace.VertexCoords.Count, extractedFace.VertexCoords.Count);
                Assert.AreEqual(modelFace.VertexIndices.Count, extractedFace.VertexIndices.Count);
                for (int j = 0; j < modelFace.Normals.Count; j++)
                {
                    Point3D extractedNormalPoint = extractedFace.Normals[j];
                    Point3D modelNormalPoint = modelFace.Normals[j];


                    Assert.AreEqual(Math.Round((double)extractedNormalPoint.X, 7), Math.Round((double)modelNormalPoint.X, 7));
                    Assert.AreEqual(Math.Round((double)extractedNormalPoint.Y, 7), Math.Round((double)modelNormalPoint.Y, 7));
                    Assert.AreEqual(Math.Round((double)extractedNormalPoint.Z, 7), Math.Round((double)modelNormalPoint.Z, 7));

                }

                for (int k = 0; k < modelFace.VertexCoords.Count; k++)
                {

                    Point3D extractionvertrexpoint = extractedFace.VertexCoords[k];
                    Point3D modelvertrexpoint = extractedFace.VertexCoords[k];
                    Assert.AreEqual(Math.Round((double)extractionvertrexpoint.X, 7), Math.Round((double)modelvertrexpoint.X, 7));
                    Assert.AreEqual(Math.Round((double)extractionvertrexpoint.Y, 7), Math.Round((double)modelvertrexpoint.Y, 7));
                    Assert.AreEqual(Math.Round((double)extractionvertrexpoint.Z, 7), Math.Round((double)modelvertrexpoint.Z, 7));

                }
                for (int l = 0; l < modelFace.VertexIndices.Count; l++)
                {
                    int extractionindicevertex = extractedFace.VertexIndices[l];
                    int modelindicevertex = modelFace.VertexIndices[l];
                    Assert.AreEqual(extractionindicevertex, modelindicevertex);
                }

            }
        }

        Occurence GetOccurence(string path, string rootFolderPath)
        {
            Occurence occ;
            XmlTextReader read = new XmlTextReader(path);
            try
            {
                bool subAssRef = false;
                AssemblyDefinition subAss;
                List<Occurence> subAssembOcc = new List<Occurence>();
                int occurencesCounter = 0;
                while (read.Read())
                {
                    if (!subAssRef && read.IsStartElement("Occurences"))
                    {
                        subAssRef = true;
                        continue;
                    }
                    if (read.IsStartElement("Occurence") && subAssRef)
                    {


                        int OccPosition = int.Parse(read.ReadString());
                        string newPfad = rootFolderPath + "\\Occurence_" + OccPosition + ".xml";
                        Occurence newOcc = GetOccurence(newPfad, rootFolderPath);
                        subAssembOcc.Add(newOcc);
                        occurencesCounter += 1;
                        continue;

                    }
                    else if (read.IsStartElement("Occurence") && !subAssRef)
                    {
                        Part extractedPart = (Part)extractElement3Ds(path)[0];
                        occ = new Occurence(extractedPart);
                        return occ;
                    }
                    if (occurencesCounter > 0)
                    {
                        subAss = new AssemblyDefinition(subAssembOcc);
                        occ = new Occurence(subAss);
                        return occ;
                    }
                    return null;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        AssemblyDefinition assemblyExtraction(string path)
        {
            AssemblyDefinition extractedAssembly = null;


            if (!File.Exists(path)) throw new Exception("File does not exist ");
            List<Element3D> allElements = new List<Element3D>();



            XmlTextReader read = new XmlTextReader(path);
            if (read.ReadState == ReadState.Error) throw new Exception("File could not be read");
            else
            {
                try
                {
                    LastReadElement LastreadElement = LastReadElement.Nichts;
                    Part actualPart = null;
                    List<Face> faces = new List<Face>();
                    Face currentFace = null;
                    bool initialized = false;
                    List<Point3D> vertices = new List<Point3D>();
                    List<Point3D> normals = new List<Point3D>();
                    List<int> indices = new List<int>();
                    Point3D currePoint3D;
                    Occurence actualOccurence;
                    AssemblyDefinition actualSubAssembly = null;
                    bool partIsReferenced = false;
                    List<Occurence> allOccurences = new List<Occurence>();
                    List<Occurence> actualSubOccurences = new List<Occurence>();
                    bool SubAssemblyOpened = false;
                    double X = 0;
                    double Z = 0;
                    double Y = 0;


                    while (read.Read())
                    {
                        //   tests if the current content node is a start tag or empty element tag.
                        if (read.IsStartElement())
                        {// if it is readable
                            switch (read.Name)
                            {
                                case "Part":
                                    continue;
                                case "Occurence":
                                    LastreadElement = LastReadElement.Occurence;
                                    if (!initialized) initialized = true;
                                    else
                                    {
                                        if (partIsReferenced)
                                        {
                                            actualOccurence = new Occurence(actualPart);
                                        }
                                        else
                                        {
                                            actualOccurence = new Occurence(actualSubAssembly);
                                        }
                                        if (!SubAssemblyOpened)
                                            allOccurences.Add(actualOccurence);
                                        else actualSubOccurences.Add(actualOccurence);
                                    }
                                    break;

                                case "SubAssembly":
                                    SubAssemblyOpened = true;
                                    break;

                                case "Reference":
                                    string typ = read.ReadString();
                                    partIsReferenced = typ == "_Part" ? true : false;
                                    if (!partIsReferenced) SubAssemblyOpened = true;

                                    break;

                                case "Face":
                                    LastreadElement = LastReadElement.Face;
                                    //wieso die initialized 
                                    if (!initialized)
                                    {
                                        initialized = true;

                                        //currentFace = new Face(vertices, normals, indices);
                                        //faces.Add(currentFace);
                                    }
                                    else
                                    {
                                        currentFace = new Face(vertices, normals, indices);
                                        faces.Add(currentFace);
                                    }
                                    break;
                                case "Normals":
                                    LastreadElement = LastReadElement.Normals;
                                    normals = new List<Point3D>();
                                    break;
                                case "Vertex":
                                    LastreadElement = LastReadElement.Vertices;
                                    vertices = new List<Point3D>();
                                    break;
                                case "Point":
                                    // LastreadElement = LastReadElement.Point;
                                    break;
                                case "Indices":
                                    LastreadElement = LastReadElement.Indices;
                                    indices = new List<int>();
                                    break;
                                case "I":
                                    indices.Add(Convert.ToInt32(read.ReadString()));
                                    break;
                                case "X":
                                    X = Convert.ToDouble(read.ReadString());
                                    break;
                                case "Y":
                                    Y = Convert.ToDouble(read.ReadString());
                                    break;
                                case "Z":
                                    Z = Convert.ToDouble(read.ReadString());

                                    currePoint3D = new Point3D(X, Y, Z);


                                    if (LastreadElement == LastReadElement.Normals) normals.Add(currePoint3D);
                                    else if (LastreadElement == LastReadElement.Vertices) vertices.Add(currePoint3D);
                                    break;

                            }

                        }


                    }
                    if (initialized)
                    {
                        currentFace = new Face(vertices, normals, indices);
                        faces.Add(currentFace);
                    }
                    actualPart = new Part(faces);
                    allElements.Add(actualPart);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }


            return extractedAssembly;
        }
        [Test]
        public void TestReadPdf3DScrew()
        {
            var fileName = @"..\..\..\..\Data\BP0370.pdf";/*"..\..\..\..\Data\BP0370.pdf";*/
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                Assert.AreEqual(0, reader.ReadPdf3D(fileName, out allElements));
                Assert.AreEqual(1, allElements.Count);
                Assert.AreEqual(allElements[0].Type, Element3DType._Part);
                Part extractedPart = (Part)allElements[0];
                string modelFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Screw" + ".xml";
                Part modelPart = (Part)extractElement3Ds(modelFile)[0];
                CompareParts(modelPart, extractedPart);

            }
        }

        [Test]
        public void TestReadPdf3DTheReplicator()
        {
            var fileName = @"..\..\..\..\Data\MP2378.pdf";
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                Assert.AreEqual(0, reader.ReadPdf3D(fileName, out allElements));
                Assert.AreEqual(1, allElements.Count);
                Assert.AreEqual(allElements[0].Type, Element3DType._Part);

                Part extractedPart = (Part)allElements[0];
                string modelFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Replicator" + ".xml";
                Part modelPart = (Part)extractElement3Ds(modelFile)[0];
                CompareParts(modelPart, extractedPart);

            }
        }
        [Test]
        public void TestLangeSchraube()
        {
            var fileName = @"..\..\..\..\Data\SP1947.pdf";//@"..\..\..\..\Data\simple_cube.prc";
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;//MP1914
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            List<Element3D> allElementsbasis = new List<Element3D>();
            Pdf3DReaderService reader = new Pdf3DReaderService();
            Assert.AreEqual(0, reader.ReadPdf3D(fileName, out allElementsbasis));
            Assert.AreEqual(1, allElementsbasis.Count);
            Assert.AreEqual(allElementsbasis[0].Type, Element3DType._Part);
            Part extractedPart = (Part)allElementsbasis[0];
            string modelFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + "LangeSchraube" + ".xml";
            Part modelPart = (Part)extractElement3Ds(modelFile)[0];
            CompareParts(modelPart, extractedPart);

        }
        List<Element3D> extractElement3Ds(string path)

        {

            if (!File.Exists(path)) throw new Exception("File does not exist ");
            List<Element3D> allElements = new List<Element3D>();



            XmlTextReader read = new XmlTextReader(path);
            if (read.ReadState == ReadState.Error) throw new Exception("File could not be read");
            else
            {
                try
                {
                    LastReadElement LastreadElement = LastReadElement.Nichts;
                    Part actualPart;
                    List<Face> faces = new List<Face>();
                    Face currentFace;
                    bool initialized = false;
                    List<Point3D> vertices = new List<Point3D>();
                    List<Point3D> normals = new List<Point3D>();
                    List<int> indices = new List<int>();
                    Point3D currePoint3D;

                    double X = 0;
                    double Z = 0;
                    double Y = 0;


                    while (read.Read())
                    {
                        //   tests if the current content node is a start tag or empty element tag.
                        if (read.IsStartElement())
                        {// if it is readable
                            switch (read.Name)
                            {
                                case "Part":
                                    continue;

                                case "Face":
                                    LastreadElement = LastReadElement.Face;
                                    //wieso die initialized 
                                    if (!initialized)
                                    {
                                        initialized = true;

                                        //currentFace = new Face(vertices, normals, indices);
                                        //faces.Add(currentFace);
                                    }
                                    else
                                    {
                                        currentFace = new Face(vertices, normals, indices);
                                        faces.Add(currentFace);
                                    }
                                    break;
                                case "Normals":
                                    LastreadElement = LastReadElement.Normals;
                                    normals = new List<Point3D>();
                                    break;
                                case "Vertex":
                                    LastreadElement = LastReadElement.Vertices;
                                    vertices = new List<Point3D>();
                                    break;
                                case "Point":
                                    // LastreadElement = LastReadElement.Point;
                                    break;
                                case "Indices":
                                    LastreadElement = LastReadElement.Indices;
                                    indices = new List<int>();
                                    break;
                                case "I":
                                    indices.Add(Convert.ToInt32(read.ReadString()));
                                    break;
                                case "X":
                                    X = Convert.ToDouble(read.ReadString());
                                    break;
                                case "Y":
                                    Y = Convert.ToDouble(read.ReadString());
                                    break;
                                case "Z":
                                    Z = Convert.ToDouble(read.ReadString());

                                    currePoint3D = new Point3D(X, Y, Z);


                                    if (LastreadElement == LastReadElement.Normals) normals.Add(currePoint3D);
                                    else if (LastreadElement == LastReadElement.Vertices) vertices.Add(currePoint3D);
                                    break;
                            }

                        }

                    }
                    if (initialized)
                    {
                        currentFace = new Face(vertices, normals, indices);
                        faces.Add(currentFace);
                    }
                    actualPart = new Part(faces);
                    allElements.Add(actualPart);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return allElements;
        }

    }
}

