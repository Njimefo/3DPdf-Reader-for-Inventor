using System;

using Inventor;
using System.Reflection;

using System.Runtime.InteropServices;
namespace InvAddIn
{
    class Entwürfe
    {
        public void AssemblyJoint()
        {


            #region inventor öffnen

            Application inventorApp = null;
            Document inventordoc = null;
            // Enable error handling.
            try
            {
                // Try to connect to a running instance of Inventor.
                inventorApp = (Application)Marshal.GetActiveObject("Inventor.Application");
            }
            catch (Exception ex)
            {
                // Connecting to a running instance failed so try to start Inventor.
                try
                {

                    inventorApp = (Application)Activator.CreateInstance(Type.GetTypeFromProgID("Inventor.Application"), true);

                    inventorApp.Visible = true;
                }
                catch (Exception ex2)
                {
                    // Unable to start Inventor.
                    Console.WriteLine("Unable to connect to or start Inventor.");
                    return;
                }
            }
            //System.Threading.Thread.Sleep(5000);
            //inventordoc = inventorApp.Documents.Add(Inventor.DocumentTypeEnum.kPartDocumentObject, "C:\\Users\\Public\\Documents\\Autodesk\\Inventor 2017\\Templates\\English\\Sheet Metal (in).ipt", true);


            #endregion




            #region Create a new assembly document.
            AssemblyDocument asmDoc =
        (AssemblyDocument)inventorApp.Documents.Add
              (DocumentTypeEnum.kAssemblyDocumentObject,
             inventorApp.FileManager.GetTemplateFile
            (DocumentTypeEnum.kAssemblyDocumentObject));

            AssemblyComponentDefinition asmDef =
                             asmDoc.ComponentDefinition;

            Matrix trans =
        inventorApp.TransientGeometry.CreateMatrix();
            #endregion


            #region die Bauelemente
            // Place an occurrence into the assembly.
            ComponentOccurrence occ1 =
                asmDef.Occurrences.Add
        ("Pfad or element like c\\:...",
                                                 trans);

            // Place a second occurrence with the matrix
            //adjusted so it fits correctly with the
            //first occurrence.
            trans.Cell[1, 4] = 6 * 2.54;
            ComponentOccurrence occ2 =
                asmDef.Occurrences.Add
        ("Pfad or element like c\\:...",
                                                 trans);
            #endregion






            // Get Face 1 from occ1 and
            // create a FaceProxy.
            Face face1 =
                (Face)GetNamedEntity(occ1, "Face1");

            // Get Face 2 from occ2 and
            // create a FaceProxy.
            Face face2 =
                (Face)GetNamedEntity(occ2, "Face2");

            // Get Edge 1 from occ2 and 
            // create an EdgeProxy.
            Edge Edge1 =
                (Edge)GetNamedEntity(occ2, "Edge1");

            // Get Edge 3 from occ1 and
            // create an EdgeProxy.
            Edge Edge3 =
                (Edge)GetNamedEntity(occ1, "Edge3");





            // Create an intent to the
            // center of Edge1.
            GeometryIntent edge1Intent =
                asmDef.CreateGeometryIntent
            (Edge1, PointIntentEnum.kMidPointIntent);

            // Create an intent to the center of Edge3.
            GeometryIntent edge3Intent =
                asmDef.CreateGeometryIntent
                (Edge3, PointIntentEnum.kMidPointIntent);

            // Create two intents to define
            // the geometry for the joint.
            GeometryIntent intentOne =
                asmDef.CreateGeometryIntent
                                (face2, edge1Intent);
            GeometryIntent intentTwo =
                asmDef.CreateGeometryIntent
                                (face1, edge3Intent);

            // Create a rotation joint between the two parts.
            AssemblyJointDefinition jointDef =
               asmDef.Joints.CreateAssemblyJointDefinition
               (AssemblyJointTypeEnum.kRotationalJointType,
                                     intentOne, intentTwo);

            jointDef.FlipAlignmentDirection = false;
            jointDef.FlipOriginDirection = true;
            AssemblyJoint joint =
                           asmDef.Joints.Add(jointDef);

            // Make the joint visible.
            joint.Visible = true;

            // Drive the joint to animate it.
            joint.DriveSettings.StartValue = "0 deg";
            joint.DriveSettings.EndValue = "180 deg";
            joint.DriveSettings.GoToStart();
            joint.DriveSettings.PlayForward();
            joint.DriveSettings.PlayReverse();

        }


        // This finds the entity associated with
        // an iMate of a specified name.  This
        // allows iMates to be used as a generic
        // naming mechansim.
        private object GetNamedEntity
            (ComponentOccurrence Occurrence, string Name)
        {
            // Look for the iMate that has the
            // specified name in the referenced file.
            PartComponentDefinition partDef =
       (PartComponentDefinition)Occurrence.Definition;

            object resultEntity = null;
            resultEntity = null;

            foreach (iMateDefinition iMate
                        in partDef.iMateDefinitions)
            {
                // Check to see if this iMate has
                // the correct name
                if (iMate.Name.ToUpper() ==
                                    Name.ToUpper())
                {
                    // Get the geometry associated
                    // with the iMate. Using InvokeMember
                    // because the iMateDefinition is the
                    // base class and does not have an
                    // Entity property
                    object entity = null;
                    entity = iMate.GetType().InvokeMember
                                 ("Entity",
                                  BindingFlags.GetProperty,
                                  null, iMate, null);

                    // Create a proxy.
                    Occurrence.CreateGeometryProxy
                        (entity, out resultEntity);
                    break;
                }
            }

            // Return the found entity, or Nothing if a match wasn't found.
            return resultEntity;
        }
    }
}
