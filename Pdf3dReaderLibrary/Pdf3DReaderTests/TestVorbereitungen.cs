using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SimilaritySearch.Pdf3DReader;


namespace SimilaritySearch.Pdf3DReaderTests
{
    //  we want to save the different data  (Indicce,  Faces,point, vertrex  )of the 3Delement
    //in the  xml file
    // the file name is SP1947
    [TestFixture]
    public class TestVorbereitungen
    {

        [Test]
        public void TestVorbereitungLangeSchraube()
        {
            var fileName = @"..\..\..\..\Data\SP1947.pdf";//@"..\..\..\..\Data\simple_cube.prc";
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);

            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> elemnts = null;
                reader.ReadPdf3D(fileName, out elemnts);

                StartSavingDataPart((Part)elemnts[0], "LangeSchraube");

            }
        }
        //   MP1914

        [Test]
        public void TestVorbereitungWasher()
        {
            var fileName = @"..\..\..\..\Data\MP1914.pdf"; //@"..\..\..\..\Data\simple_cube.prc";
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                reader.ReadPdf3D(fileName, out allElements);
                StartSavingDataPart((Part) allElements[0], "Wsher");

            }
        }

        [Test]
        public void TestVorbereitungCube()
        {
            var fileName = @"..\..\..\..\Data\186455.pdf";//@"..\..\..\..\Data\simple_cube.prc";
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                reader.ReadPdf3D(fileName, out allElements);
                Part testPart = (Part)allElements[0];
                StartSavingDataPart(testPart, "Cube");

            }
        }


        void SaveAssembly(AssemblyDefinition ass, string name)
        {
            string pfad = AppDomain.CurrentDomain.BaseDirectory + "\\" + name;


            if (!Directory.Exists(pfad))
                Directory.CreateDirectory(pfad);
            else return;
            XmlTextWriter writer = new XmlTextWriter(pfad + "\\" + name + "_Main" + ".xml", System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("MainAssembly");
            writer.WriteStartElement("Occurences");
            for (int i = 0; i < ass.Occurences.Count; i++)
            {
                writer.WriteStartElement("Occurence");
                SvaeOcc(ass.Occurences[i], pfad, i + 1);
                writer.WriteString((i + 1).ToString());
                writer.WriteEndElement();



            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        void SvaeOcc(Occurence occ, string pfad, int nbre)
        {

            //fall die occurence ist ein part  wir erstellen eine xml datei  fur diese occurence .also mit start element occurence 
            if (occ.PartIsReferenced)
            {
                XmlTextWriter writer = new XmlTextWriter(pfad + "\\Occurence_" + nbre + ".xml", System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Occurence");
                // hier ist doccurence .part geschrieben 
                StartSavingDataPart(occ.referencedPart, writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            // fall die occurence ist nicht ein part ist ein xml trotzdem erstellen aber nur mit der references .das startelement hier ist occurence

            else
            {
                string newNbre = nbre.ToString() + "1";
                int newNbre2 = int.Parse(newNbre);
                //occurence?
                XmlTextWriter writer = new XmlTextWriter(pfad + "\\Occurence_" + nbre + ".xml", System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Occurences");
                for (int i = 0; i < occ.referencedAssemblyDefinition.Occurences.Count; i++)
                {
                    writer.WriteStartElement("Occurence");
                    newNbre2 += 1;
                    //
                    SvaeOcc(occ.referencedAssemblyDefinition.Occurences[i], pfad, newNbre2);
                    writer.WriteString(newNbre2.ToString());
                    writer.WriteEndElement();

                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }
        [Test]
        public void TestVorbereitungReplicator()
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
                reader.ReadPdf3D(fileName, out allElements);
                Part testPart = (Part)allElements[0];

                StartSavingDataPart(testPart, "Replicator");
            }
        }

        [Test]
        public void TestVorbereitungScrew()
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
                reader.ReadPdf3D(fileName, out allElements);
                Part testPart = (Part)allElements[0];
                StartSavingDataPart(testPart, "Screw");
            }
        }

        private void StartSavingDataPart(Part part, string fileName, bool directPath = false)
        {
            XmlTextWriter writer;
            if (!directPath)
                writer = new XmlTextWriter(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName + ".xml", System.Text.Encoding.UTF8);
            else writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);

            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("Part");
            foreach (var face in part.Faces)
            {
                createNode(face, writer);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();

        }
        private void StartSavingDataPart(Part part, XmlTextWriter writer)
        {

            writer.WriteStartElement("Part");
            foreach (var face in part.Faces)
            {
                createNode(face, writer);
            }

            writer.WriteEndElement();


        }
        [Test]
        public void TestAssemblyFahrradteilelement()
        {
            var fileName = @"..\..\..\..\Data\TestOwnAssembly7.pdf";/*"..\..\..\..\Data\BP0370.pdf";*/
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            fileName = Path.Combine(Path.GetDirectoryName(path), fileName);
            Assert.IsTrue(File.Exists(fileName), $"3D-PDF file {fileName} does not exist.");
            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                reader.ReadPdf3D(fileName, out allElements);
                AssemblyDefinition tesAssembly = (AssemblyDefinition)allElements[0];
                SaveAssembly(tesAssembly, "Fahrradteilelement");

                // StartSavingDataAssembly(tesAssembly, "Fahrradteilelement");
            }
        }
        //hier wollen wir assambly speichern und dieses assembly ist einfach ein menge von occuences .
        //die funktion occreateoccurence  ist fur das schreiben eines  occurences   in xml datei  
        void StartSavingDataAssembly(AssemblyDefinition assembly, string fileName)
        {
            XmlTextWriter writer = new XmlTextWriter(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName + ".xml", System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("MainAssembly");
            writer.WriteStartElement("Occurences");
            foreach (var occ in assembly.Occurences)
            {
                createOccurences(occ, writer);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
        //
        private void createOccurences(Occurence occ, XmlTextWriter writer)
        {
            writer.WriteStartElement("Occurence");
            writer.WriteStartElement("Reference");
            writer.WriteString((bool)occ.PartIsReferenced ? "_Part" : "_Assembly");
            writer.WriteEndElement();
            if ((bool)occ.PartIsReferenced)
            {

                StartSavingDataPart((Part)occ.referencedPart, writer);
            }
            else
            {
                writer.WriteStartElement("SubAssembly");
                writer.WriteStartElement("Occurences");
                foreach (Occurence occurence in occ.referencedAssemblyDefinition.Occurences)
                {
                    createOccurences(occurence, writer);
                }
                writer.WriteEndElement();
                writer.WriteEndElement();

            }
            writer.WriteEndElement();


        }

        private void createNode(Face face, XmlTextWriter writer)
        {

            writer.WriteStartElement("Face");

            writer.WriteStartElement("Normals");
            foreach (var pt in face.Normals)
            {
                writer.WriteStartElement("Point");

                writer.WriteStartElement("X");
                writer.WriteString(pt.X.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Y");
                writer.WriteString(pt.Y.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Z");
                writer.WriteString(pt.Z.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();

            }

            writer.WriteEndElement();

            writer.WriteStartElement("Vertex");

            foreach (var pt in face.VertexCoords)
            {
                writer.WriteStartElement("Point");


                writer.WriteStartElement("X");
                writer.WriteString(pt.X.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Y");
                writer.WriteString(pt.Y.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Z");
                writer.WriteString(pt.Z.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();

            }
            writer.WriteEndElement();

            writer.WriteStartElement("Indices");

            foreach (var pt in face.VertexIndices)
            {

                writer.WriteStartElement("I");
                writer.WriteString(pt.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndElement();


        }
    }
}
