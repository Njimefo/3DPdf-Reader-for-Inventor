using System;
using System.Runtime.InteropServices;
using Inventor;


using System.Collections.Generic;

using SimilaritySearch.Pdf3DReader;




namespace Addin3DPdf
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("5a2d6e09-72a1-454e-a12c-83671073a027")]
    public class TrAddInServer : Inventor.TranslatorAddInServer
    {

        // Inventor  objects.

        static public Application MApp;
     


        public TrAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            MApp = addInSiteObject.Application;
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            MApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public void ShowOpenOptions(DataMedium SourceData, TranslationContext Context, NameValueMap ChosenOptions)
        {
         
        }

        public void Open(DataMedium SourceData, TranslationContext Context, NameValueMap Options, ref object TargetObject)
        {
            InvAddIn.InventorSol S = new InvAddIn.InventorSol();
          

            using (var reader = new Pdf3DReaderService())
            {
                List<Element3D> allElements = null;
                List<string> Olist = new List<string>();
                reader.ReadPdf3D(SourceData.FileName, out allElements);
                S.Draw3D((Part)allElements[0],"test");

            }




            //// InvAddIn.InventorSol.Start(Pdf_File);

            //InvAddIn.InventorSol S = new InvAddIn.InventorSol();
            //S.Start(SourceData.FileName);

            ////InvAddIn.InventorSol.StandartTest();


        }

        public void ShowSaveCopyAsOptions(object SourceObject, TranslationContext Context, NameValueMap ChosenOptions)
        {
            
        }

        public void SaveCopyAs(object SourceObject, TranslationContext Context, NameValueMap Options, DataMedium TargetData)
        {
           
        }

        public object GetThumbnail(DataMedium SourceData)
        {
            return null;
        }

        public bool get_HasOpenOptions(DataMedium SourceData, TranslationContext Context, NameValueMap DefaultOptions)
        {
            return false;
        }

        public bool get_HasSaveCopyAsOptions(object SourceObject, TranslationContext Context, NameValueMap DefaultOptions)
        {
            return false;
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
