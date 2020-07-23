// PDF3dReader.h

#pragma once
#pragma 
#include <string>
#include <iostream>
#include "Element3D.h"
#include "ColorDefinition.h"
#define INITIALIZE_A3D_API
#include <A3DSDKIncludes.h>
#include <A3DSDKLicenseKey.h>

#include <msclr\marshal_cppstd.h>
//#include <msclr/marshal.h>
#include "Face.h"
#include "Occurence.h"
#include "Part.h"

using namespace msclr::interop;
using namespace System;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Text;
using namespace System::Runtime::InteropServices;
using namespace System::Reflection;

#define CHECK_RET(function_call) {\
	iRet = function_call; if(iRet != A3D_SUCCESS) { std::cout << "Error number=" << iRet << std::endl; return iRet; }\
}

namespace SimilaritySearch
{
	namespace Pdf3DReader
	{

		public ref class Pdf3DReaderService
		{
		public:

			Pdf3DReaderService()
			{
			}

			~Pdf3DReaderService()
			{
				if (stOut && stOut != stdout)
					fclose(stOut);
				stOut = stdout;
				Terminate();
			}

			!Pdf3DReaderService()
			{
				Terminate();
			}

			int ReadPdf3D(String^ pdf3DFileName, [Out] List<Element3D^>^% elements)
			{
				if (!Init())
				{
					printf("Cannot initialize HOOPS Exchange toolkit\n");
					return A3D_ERROR;
				}

				A3DAsmModelFile* pModelFile = sHoopsExchangeLoader->m_psModelFile;

				A3DStream3DPDFData* pStream3DPDFData;
				A3DInt32 iNumStreams;
				A3DGet3DPDFStreams(marshal_as<std::string>(pdf3DFileName).c_str(), &pStream3DPDFData, &iNumStreams);
				// a real use case might parse every stream in the input file. Here, we just take the first one (pStream3DPDFData[0]).
				if (pStream3DPDFData[0].m_bIsPrc) // test whether the data is PRC or U3D
				{
					// even an input PRC file must be read in memory and mapped into modelfile data structures
					A3DRWParamsPrcReadHelper* pPrcReadHelper;
					A3DAsmModelFileLoadFromPrcStream(pStream3DPDFData[0].m_pcStream, pStream3DPDFData[0].m_iLength, &pPrcReadHelper, &pModelFile);
				}

				else
				{
					
					printf("Cannot read PDF file containing U3D\n");

					return A3D_NOT_IMPLEMENTED;
				}

				A3DStatus iRet;
				// Comment in this code in if you physical properties one day
				//A3DPhysicalPropertiesData sPhysPropsData;
				//A3D_INITIALIZE_DATA(A3DPhysicalPropertiesData, sPhysPropsData);
				//iRet = A3DComputeModelFilePhysicalProperties(pModelFile, &sPhysPropsData);

				// Retrieve files path from model files
				A3DUns32 nbFiles = 0, nbAssemblyFiles = 0, nbMissingFiles = 0;
				A3DUTF8Char** ppPaths = NULL, ** ppAssemblyPaths = NULL, ** ppMissingPaths = NULL;
				CHECK_RET(A3DAsmGetFilesPathFromModelFile(sHoopsExchangeLoader->m_psModelFile, &nbFiles, &ppPaths, &nbAssemblyFiles,
					&ppAssemblyPaths, &nbMissingFiles, &ppMissingPaths));

				//faceList = /*(Face^)*/ gcnew List<Face^>();
				return TraverseModel(pModelFile, elements);
			}

		internal:
			/// <summary>
			/// Load HOOPS Exchange DLL and check if the license is valid.
			/// </summary>
			/// <returns>
			/// true if DLL has been loaded and the license is valid.
			/// </returns>
			bool Init()
			{
				if (stbA3DLoaded)
					return true;

#	/*ifdef _WIN64
				String^ libPath = _T("..\\..\\..\\..\\HOOPS_Exchange\\bin\\win64\\");
				String^ HoopsDlls = _T("HoopsDlls\\");
#	else
				String^ libPath = _T("..\\..\\..\\..\\HOOPS_Exchange\\bin\\win32\\");
				
#	endif
				String^ codeBase = Assembly::GetExecutingAssembly()->CodeBase;
				UriBuilder^ uri = gcnew UriBuilder(codeBase);
				String^ path = Uri::UnescapeDataString(uri->Path);
				libPath = Path::Combine(Path::GetDirectoryName(path), HoopsDlls);*/
				String^ libPath = "C:\\ProgramData\\Autodesk\\Inventor 2017\\Addins\\Addin3DPdf\\HoopsDlls";
				/*String^ libPath = _T("..\\..\\..\\..\\HOOPS_Exchange\\bin\\win64\\");
				String^ codeBase = Assembly::GetExecutingAssembly()->CodeBase;
				UriBuilder^ uri = gcnew UriBuilder(codeBase);
				String^ path = Uri::UnescapeDataString(uri->Path);
				libPath = Path::Combine(Path::GetDirectoryName(path), libPath);*/

				if (!A3DSDKLoadLibrary(marshal_as<std::string>(libPath).c_str()))
				{
					printf("Cannot load the HOOPS Exchange library\n");
					return false;
				}

				A3DLicPutLicense(A3DLicPutLicenseFile, pcCustomerKey, pcVariableKey);
				A3DInt32 iMajorVersion = 0, iMinorVersion = 0;
				A3DDllGetVersion(&iMajorVersion, &iMinorVersion);
				if (A3DDllInitialize(A3D_DLL_MAJORVERSION, A3D_DLL_MINORVERSION) != A3D_SUCCESS)
				{
					printf("Cannot initialize the HOOPS Exchange library\n");
					A3DSDKUnloadLibrary();
					return false;
				}

				sHoopsExchangeLoader = new A3DSDKHOOPSExchangeLoader(marshal_as<std::wstring>(libPath).c_str());
				if (sHoopsExchangeLoader->m_eSDKStatus != A3D_SUCCESS)
				{
					printf("Cannot initialize the A3DSDKHOOPSExchangeLoader\n");
					A3DSDKUnloadLibrary();
					return false;
				}

				stbA3DLoaded = true;

				return true;
			}

			void Terminate()
			{
				if (stbA3DLoaded)
				{
					A3DDllTerminate();
					A3DSDKUnloadLibrary();
					if (sHoopsExchangeLoader != NULL)
						delete sHoopsExchangeLoader;
					stbA3DLoaded = false;
				}
			}

			static A3DStatus TraverseModel(const A3DAsmModelFile* pModelFile, List<Element3D^>^% elements)
			{
				A3DStatus iRet = A3D_SUCCESS;
				A3DAsmModelFileData sData;

				A3D_INITIALIZE_DATA(A3DAsmModelFileData, sData);


				allElements = gcnew List<Element3D^>();
				iRet = A3DAsmModelFileGet(pModelFile, &sData);
				if (iRet == A3D_SUCCESS)
				{
					A3DUns32 ui;
					//Checks if there is just one assembly or part in the file
					if (sData.m_uiPOccurrencesSize)
					{
						Occurence^ newOccurence ;
						//Retrieve assembly or part;
						Initialized = false;
						TraversePOccurrenceFromSub(sData.m_ppPOccurrences[0], newOccurence);

					}
					else if (sData.m_uiPOccurrencesSize > 1)
					{
						//Must be implemented

					}
					elements = allElements;

					CHECK_RET(A3DAsmModelFileGet(NULL, &sData));
				}
				else
				{
					printf("Cannot retrieve the model file\n");
				}

				return iRet;
			}
			static A3DAsmPartDefinition* GetPart(A3DAsmProductOccurrence* po)
			{
				A3DAsmProductOccurrenceData poData;
				A3D_INITIALIZE_DATA(A3DAsmProductOccurrenceData, poData);
				A3DAsmProductOccurrenceGet(po, &poData);

				if (poData.m_pPart)
					return poData.m_pPart;
				if (poData.m_pPrototype)
					return GetPart(poData.m_pPrototype);
				if (poData.m_pExternalData)
					return GetPart(poData.m_pExternalData);
			}
			static	Part^ extractPart(AssemblyDefinition^ ass)
			{
				for each (Occurence^ occ in ass->Occurences)
				{
					if (!occ->PartIsReferenced) extractPart(occ->referencedAssemblyDefinition);
					else return occ->referencedPart;
				}
			}
			static A3DStatus getColorOfOccurence(const A3DAsmProductOccurrence* pOccurrence, ColorDefinition^ color)
			{
				// Class for gathering of all root base graphic data 
				A3DRootBaseWithGraphicsData rbwgData;

				A3DUns32 styleIndex;
				ColorDefinition^ pOccurenceColorDefinition = color;
				A3D_INITIALIZE_DATA(A3DRootBaseWithGraphicsData, rbwgData);
				A3DRootBaseWithGraphicsGet(pOccurrence, &rbwgData);

				//checks if there are graphics
				if (rbwgData.m_pGraphics != NULL)
				{
					// this entity has an attached graphics object
					A3DGraphicsData graphicsData;
					//Initializing of Data
					A3D_INITIALIZE_DATA(A3DGraphicsData, graphicsData);
					//getting graphics data
					A3DGraphicsGet(rbwgData.m_pGraphics, &graphicsData);
					//checks if the graphic is styled (has color/texture or not) 
					if (!(graphicsData.m_uiStyleIndex == A3D_DEFAULT_STYLE_INDEX))

						//getting the index of its style
						styleIndex = graphicsData.m_uiStyleIndex;
					else return A3D_NOT_IMPLEMENTED;
					try
					{
						//set its color
						color = allStoredColors[styleIndex];
					}
					catch (const std::exception&)

					{
						return A3D_NOT_IMPLEMENTED;
					}
				}
			}
			static A3DStatus RetrieveColorsFromOccurence()
			{
				allStoredColors = gcnew List<ColorDefinition^>();
				A3DGlobal* pGlobal;

				// get the pointer to global data
				A3DGlobalGetPointer(&pGlobal);


				A3DGlobalData globalData;

				//initialize the both instances of globalData
				A3D_INITIALIZE_DATA(A3DGlobalData, globalData);

				//get global Data
				A3DGlobalGet(pGlobal, &globalData);


				int numStyles = globalData.m_uiStylesSize;
				A3DGraphStyleData styleData;
				A3D_INITIALIZE_DATA(A3DGraphStyleData, styleData);


				//Getting all color styles
				for (size_t n = 0; n < numStyles; ++n)
				{
					// get the nth style from the global data
					A3DGlobalGetGraphStyleData(n, &styleData);

					if (styleData.m_bMaterial == true)
					{
						//The Matial Properties are actually not needded, therefore not implemented
						return A3D_NOT_IMPLEMENTED;
					}
					else
					{
						A3DStatus iRet = A3D_SUCCESS;
						A3DGraphRgbColorData sColorData;
						A3D_INITIALIZE_DATA(A3DGraphRgbColorData, sColorData);

						iRet = A3DGlobalGetGraphRgbColorData(styleData.m_uiRgbColorIndex, &sColorData);

						if (iRet == A3D_SUCCESS)
							allStoredColors->Add(gcnew ColorDefinition((byte)sColorData.m_dRed, (byte)sColorData.m_dGreen, (byte)sColorData.m_dBlue));
						else
							return iRet;


					}
				}
			}





			static A3DStatus TraversePOccurrenceFromSub(const A3DAsmProductOccurrence* pOccurrence, Occurence^% Occ)
			{
				A3DStatus iRet = A3D_SUCCESS;


				A3DAsmProductOccurrenceData sData;
				A3D_INITIALIZE_DATA(A3DAsmProductOccurrenceData, sData);

				iRet = A3DAsmProductOccurrenceGet(pOccurrence, &sData);
				if (iRet == A3D_SUCCESS)
				{
					A3DUns32 ui;

					if (!Initialized)
					{
						Initialized = true;
						if (sData.m_uiPOccurrencesSize > 1)
						{
					
							List<Occurence^>^ occurences = gcnew List<Occurence^>();
							List<Occurence^>^ occurencesClone = gcnew List<Occurence^>();
							//Retrieve all Occurences of the subassembly
							for (ui = 0; ui < sData.m_uiPOccurrencesSize; ++ui)
							{

								Occurence^ newOccurence ;
								TraversePOccurrenceFromSub(sData.m_ppPOccurrences[ui], newOccurence);
								occurences->Add(newOccurence);
							}
							//Create a new subassembly
							for each (Occurence^ var in occurences)
							{
								if (var != nullptr) occurencesClone->Add(var);
							}
							if (occurencesClone->Count > 1)
							{
								AssemblyDefinition^ MainAssembly = gcnew AssemblyDefinition(occurences);

								allElements->Add(MainAssembly);
							}
							else if(occurencesClone->Count==1&&occurencesClone[0]->PartIsReferenced)
							{
								allElements->Add(occurencesClone[0]->referencedPart);
							}
							else if (occurencesClone->Count == 1 && !occurencesClone[0]->PartIsReferenced)
							{
								allElements->Add(occurencesClone[0]->referencedAssemblyDefinition);
							}
							else return A3D_NOT_IMPLEMENTED;
							

						}
						else if (sData.m_uiPOccurrencesSize&&sData.m_pPart == nullptr)
						{

							List<Face^>^ faceList = gcnew List<Face^>();
							Occurence^ newOccurence;

							//Retrieves all faces from part
							/*		 TraversePartDef(GetPart(pOccurrence), faceList);*/
							//iRet = TraversePOccurrenceFromSub(pOccurrence, newOccurence);
							iRet = TraversePartOccurrence(sData.m_ppPOccurrences[0], faceList);
							
							if (iRet == A3D_SUCCESS)
							{

								try
								{
									///*if (newOccurence->PartIsReferenced)allElements->Add(newOccurence->referencedPart);
									//else*/ allElements->Add(extractPart(newOccurence->referencedAssemblyDefinition));
									ColorDefinition^ color;
									getColorOfOccurence(pOccurrence, color);
									Part^ newPart = gcnew Part(faceList);
									allElements->Add(newPart);
									return	A3D_SUCCESS;
								}
								catch (const std::exception&)
								{
										
								}
							

							}

						}
						else if (sData.m_pPart != nullptr)
						{
							List<Face^>^ faceList = gcnew List<Face^>();
							//Retrieves all faces from part
							iRet = TraversePartDef(sData.m_pPart, faceList);
							if (iRet == A3D_SUCCESS)
							{
								ColorDefinition^ color;
								getColorOfOccurence(pOccurrence, color);
								Part^ newPart = gcnew Part(faceList);
								allElements->Add(newPart);
								return A3D_SUCCESS;

							}
						}
						else
						{


							List<Occurence^>^ occurences = gcnew List<Occurence^>();
							//Retrieve all Occurences of the subassembly
							for (ui = 0; ui < sData.m_uiPOccurrencesSize; ++ui)
							{

								Occurence^ newOccurence;
								TraversePOccurrenceFromSub(sData.m_ppPOccurrences[ui], newOccurence);
								occurences->Add(newOccurence);
							}


							List<Face^>^ faceList = gcnew List<Face^>();
							//Retrieves all faces from part
							iRet = TraversePartDef(sData.m_pPart, faceList);
							if (iRet == A3D_SUCCESS)
							{
								Part^ newPart = gcnew Part(faceList);
								Occurence^ occ = gcnew Occurence(newPart);
								occurences->Add(occ);
								

							}
							//Create a new subassembly
							AssemblyDefinition^	MainAssembly = gcnew AssemblyDefinition(occurences);
							allElements->Add(MainAssembly);
							return A3D_SUCCESS;
						}
					}
					else
					{
						if (sData.m_uiPOccurrencesSize >= 1)
						{
							//Create a new subassembly
						

							List<Occurence^>^ occurences = gcnew List<Occurence^>();
							//Retrieve all Occurences of the subassembly
							for (ui = 0; ui < sData.m_uiPOccurrencesSize; ++ui)
							{

								Occurence^ newOccurence ;
								LocationData^ location;
						
								TraversePOccurrenceFromSub(sData.m_ppPOccurrences[ui], newOccurence);
						
								occurences->Add(newOccurence);
							}
							AssemblyDefinition^ subAssembly = gcnew AssemblyDefinition(occurences);
							//Set the subassembly as referenced element by occurence
							Occ = gcnew Occurence(subAssembly);
							
						}
					}

					//If the current occurence references to prototype then we have to 
					// explore more low
					if (sData.m_pPrototype)
					{
						printf("Prototype Data enterred");
						LocationData^ locationData;
						ColorDefinition^ color;
						getColorOfOccurence(sData.m_pLocation, color);
						TraverseLocationData(sData.m_pLocation, locationData);
						TraversePOccurrenceFromSub(sData.m_pPrototype, Occ);
						Occ->occurenceLocation = locationData;
					}

					//If the current occurence references to an external data then we have to 
					// explore more low
					if (sData.m_pExternalData)
					{
						printf("External Data enterred");
						LocationData^ locationData;
						ColorDefinition^ color;
						getColorOfOccurence(sData.m_pLocation, color);
						TraverseLocationData(sData.m_pLocation, locationData);
						TraversePOccurrenceFromSub(sData.m_pExternalData, Occ);
						Occ->occurenceLocation = locationData;

					}
					if (sData.m_pPart)
					{
						//Create new list of faces for the part
						List<Face^>^ faceList = gcnew List<Face^>();

						//Retrieves all faces from part
						TraversePartDef(sData.m_pPart, faceList);
						

						
						//if(faceList->Count>1)

						Part^ newPart = gcnew Part(faceList);
						ColorDefinition^ color;
						getColorOfOccurence(pOccurrence, color);
						//Set the Part as referenced element by occurence
						//if(faceList->Count>1)
						if (!IsPart)
							Occ = gcnew Occurence(newPart);
						else allElements->Add(newPart);
					}

					CHECK_RET(A3DAsmProductOccurrenceGet(NULL, &sData));
				}
				else
				{
					printf("Cannot retrieve the product occurence data\n");
				}

				return iRet;
			}
			static A3DStatus TraversePartOccurrence(const A3DAsmProductOccurrence* pOccurrence, List<Face^>^% faceList)
			{
				A3DStatus iRet = A3D_SUCCESS;
				A3DAsmProductOccurrenceData sData;
				A3D_INITIALIZE_DATA(A3DAsmProductOccurrenceData, sData);

				iRet = A3DAsmProductOccurrenceGet(pOccurrence, &sData);
				if (iRet == A3D_SUCCESS)
				{
					A3DUns32 ui;

					  if (sData.m_pPrototype)
					{
						  TraversePartOccurrence(sData.m_pPrototype, faceList);
					}

					if (sData.m_pExternalData)
					{
						TraversePartOccurrence(sData.m_pExternalData, faceList);
					}

					for (ui = 0; ui < sData.m_uiPOccurrencesSize; ++ui)
						TraversePartOccurrence(sData.m_ppPOccurrences[ui], faceList);

					if (sData.m_pPart)
					{
	                   TraversePartDef(sData.m_pPart, faceList);
					}

					CHECK_RET(A3DAsmProductOccurrenceGet(NULL, &sData));
				}
				else
				{
					printf("Cannot retrieve the product occurence data\n");
				}

				return iRet;
			}
	
			static A3DStatus TraverseLocationData(A3DMiscTransformation* TransformationNode, LocationData^% locationData)
			{
				A3DMiscTransformation* transformationNode = TransformationNode;

				A3DEEntityType peEntityType;
				A3DStatus iRet = A3DEntityGetType(transformationNode, &peEntityType);


				if(iRet == A3D_SUCCESS)
				if (peEntityType == kA3DTypeMiscCartesianTransformation)
				{


					A3DMiscCartesianTransformationData transformationData;
					A3D_INITIALIZE_DATA(A3DMiscCartesianTransformationData, transformationData);
					A3DMiscCartesianTransformationGet(transformationNode, &transformationData);

					// get transformation origin
					double x = transformationData.m_sOrigin.m_dX;
					double y = transformationData.m_sOrigin.m_dY;
					double z = transformationData.m_sOrigin.m_dZ;


					// transform type (bitwise flag)
					A3DUns8 flagBehaviour = transformationData.m_ucBehaviour;
					unsigned short Options = kA3DTransformationIdentity | kA3DTransformationTranslate |
						kA3DTransformationRotate | kA3DTransformationMirror | kA3DTransformationScale | kA3DTransformationNonUniformScale;
					string transformationType = GetTransformation(flagBehaviour);

					//TODO : To continue (Brandon)
					return A3D_SUCCESS;

				}
				else if (peEntityType == kA3DTypeMiscGeneralTransformation)
				{
					A3DMiscGeneralTransformationData transformationData;
					A3D_INITIALIZE_DATA(A3DMiscGeneralTransformationData, transformationData);
					A3DMiscGeneralTransformationGet(transformationNode, &transformationData);

					
					// get matrix
					A3DDouble *matrixAdress = transformationData.m_adCoeff;
					A3DDouble EndMatrix[4][4] ;
					EndMatrix[4][4] = *matrixAdress;
			/*		double Matrix[4][4];
					List<List<double>^>^ allElements = gcnew List<List<double>^>();*/
					cli::array<double^, 2>^ elements = gcnew cli::array<double^, 2>(4, 4);
					
			
					for (size_t i = 0; i < 4; i++)
					{
					/*	List<double>^ element = gcnew List<double>();*/
						for (size_t k = 0; k < 4; k++)
						{

					/*		double zahl = EndMatrix[i][k];
							element->Add(zahl);
							Matrix[i][k] = EndMatrix[i][k];*/
							elements[i,k] = EndMatrix[i][k];

						}
						/*allElements->Add(element);*/
					}
					locationData = gcnew LocationData(elements);
					return A3D_SUCCESS;
				}
				else return A3D_NOT_IMPLEMENTED;

	
				// TODO: to de-initialize transformationData structure when you're done

			}

			static string GetTransformation(A3DUns8 behaviour)
			{
				string result = "";

				bool IsmultipleTransformation = CheckMultipleTransformation(behaviour, &result);

				if (IsmultipleTransformation)
				{
					throw new exception("Multiple transformation actually not implemented");
					//Actually Not Implemented
				}
				else return result;


			}

			/// <summary>
			/// Checks if there are multiple transformation behaviours
			/// </summary>
			/// <param name="behaviour">Behaviur parameter</param>
			/// <param name="result">String result that specifies if the behaviour of the trnasformation</param>
			/// <returns>return true if there are many transformation behaviors and false if not</returns>
			static	bool CheckMultipleTransformation(A3DUns8 behaviour, string* Result)
			{
				string result = *Result;
				//Indentity Transformation
				if (behaviour&kA3DTransformationIdentity)
				{
					result = "0";
					return false;
				}

				//Translation Transformation
				else 	if (behaviour&kA3DTransformationTranslate)
				{
					result = "1";
					return false;
				}

				//Rotation Transformation
				else 	if (behaviour&kA3DTransformationRotate)
				{
					result = "2";
					return false;
				}

				//Mirror Transformation
				else 	if (behaviour&kA3DTransformationMirror)
				{
					result = "4";
					return false;
				}

				//Scaling Transformation
				else 	if (behaviour&kA3DTransformationScale)
				{
					result = "8";
					return false;
				}
				//Non Uniform Scaling  Transformation
				else 	if (behaviour&kA3DTransformationNonUniformScale)
				{
					result = "9";
					return false;
				}
				else return true;

			}

			static A3DStatus TraversePartDef(const A3DAsmPartDefinition* pPart, List<Face^>^% faceList)
			{
				A3DStatus iRet = A3D_SUCCESS;
				A3DAsmPartDefinitionData sData;
				A3D_INITIALIZE_DATA(A3DAsmPartDefinitionData, sData);

				iRet = A3DAsmPartDefinitionGet(pPart, &sData);
				if (iRet == A3D_SUCCESS)
				{
					A3DUns32 ui;


					for (ui = 0; ui < sData.m_uiRepItemsSize; ++ui)
					{
						TraverseRepItem(sData.m_ppRepItems[ui], faceList);
					}


					A3DAsmPartDefinitionGet(NULL, &sData);
				}
				else
				{
					printf("Cannot retrieve the part definition data\n");
				}

				return iRet;
			}

			static A3DStatus TraverseRepItem(const A3DRiRepresentationItem* pRepItem, List<Face^>^% faceList)
			{
				A3DStatus iRet = A3D_SUCCESS;
				A3DEEntityType eType;

				CHECK_RET(A3DEntityGetType(pRepItem, &eType));

				switch (eType)
				{
				case kA3DTypeRiBrepModel:
					iRet = TraverseRepItemContent(pRepItem, faceList);
					break;
				default:
					iRet = A3D_NOT_IMPLEMENTED;
					break;
				}
				return iRet;
			}

			static A3DStatus TraverseRepItemContent(const A3DRiRepresentationItem* pRi, List<Face^>^% faceList)
			{
				A3DStatus iRet = A3D_SUCCESS;
				A3DRiRepresentationItemData sData;
				A3D_INITIALIZE_DATA(A3DRiRepresentationItemData, sData);

				iRet = A3DRiRepresentationItemGet(pRi, &sData);
				if (iRet == A3D_SUCCESS)
				{

					TraverseTessBase(sData.m_pTessBase, faceList);

					A3DRiRepresentationItemGet(NULL, &sData);
				}
				else
				{
					printf("Cannot retrieve the RepItem content\n");
				}

				return A3D_SUCCESS;
			}

			static A3DStatus TraverseTessBase(const A3DTessBase* pTess, List<Face^>^% faceList)
			{
				A3DEEntityType eType;
				A3DStatus iRet = A3DEntityGetType(pTess, &eType);
				if (iRet == A3D_SUCCESS)
				{

					switch (eType)
					{
					case kA3DTypeTess3D:
						Traverse3DTess(pTess, faceList);
						break;
					default:
						break;
					}
				}

				return iRet;
			}

			static A3DStatus Traverse3DTess(const A3DTess3D* pTess, List<Face^>^% faceList)
			{
				A3DTessBaseData sBaseData;
				A3D_INITIALIZE_DATA(A3DTessBaseData, sBaseData);
				A3DStatus iBaseRet = A3DTessBaseGet(pTess, &sBaseData);

				A3DTess3DData sData;
				A3D_INITIALIZE_DATA(A3DTess3DData, sData);
				A3DStatus iRet = A3DTess3DGet(pTess, &sData);

				ColorDefinition^ color;
				if (iBaseRet == A3D_SUCCESS && iRet == A3D_SUCCESS && sData.m_bHasFaces)
				{


					// Face data: From here we can retrieve the necessary data form the 3 arrays.
					for (A3DUns32 ui = 0; ui < sData.m_uiFaceTessSize; ++ui)
					{
						A3DTessFaceData& sFace = sData.m_psFaceTessData[ui];



						// Type(s) of the entities of the face. 
						A3DUns16 flags = sFace.m_usUsedEntitiesFlags;
						// TODO: For now we handle triangles only: No triangle fans and strips
						unsigned short handledOptions = kA3DTessFaceDataTriangle | kA3DTessFaceDataTriangleTextured |
							kA3DTessFaceDataTriangleOneNormal | kA3DTessFaceDataTriangleOneNormalTextured;
						if (flags & handledOptions)
						{
							// For triangle types, the m_puiSizesTriangulated member specifies the number of triangles for this face. 
							A3DUns32 numberOfTriangles = sFace.m_puiSizesTriangulated[0];

							// The description of the triangles of the face begins at this position
							// in the m_puiTriangulatedIndexes array filled above.
							A3DUns32 startTriangulated = sFace.m_uiStartTriangulated;

							// Number of indexes per entry
							int indexesPerEntry = GetIndexesPerEntry(flags);

							// Number of normal per triangle 
							int normalsPerTriangle = GetNormalsPerTriangle(flags);
						
							if (sFace.m_uiRGBAVerticesSize > 0)
							{
								A3DUns8* storedColors = sFace.m_pucRGBAVertices;
								
								int nbreIndices = normalsPerTriangle * 3;
								A3DGraphRgbColorData* colors = (A3DGraphRgbColorData*)malloc(nbreIndices *sizeof(int*));
								colors = reinterpret_cast<A3DGraphRgbColorData*>(storedColors);

								for (size_t i = 0; i < numberOfTriangles*3; i++)
								{
									A3DGraphRgbColorData sColorData ;
									A3D_INITIALIZE_DATA(A3DGraphRgbColorData, sColorData);
									sColorData = colors[i];
									
									byte r = sColorData.m_dRed;
									byte g = sColorData.m_dGreen;
									byte b = sColorData.m_dBlue;
									byte alpha;

							


									if (sFace.m_bIsRGBA)
									{
										//alpha = GetA
										alpha = sFace.m_pucRGBAVertices[3];
										color = gcnew ColorDefinition(r, g, b, alpha);
									}
									else
									{

										color = gcnew ColorDefinition(r, g, b);
									}
								}
							}
								

						
							else
							{
								
							}
							// Add face data for this face to the list
							faceList->Add(CreateFaceData(indexesPerEntry, normalsPerTriangle, numberOfTriangles, startTriangulated, sBaseData, sData));

						}
						else
						{
							printf("Can only handle faces with the kA3DTessFaceDataTriangleTextured or the kA3DTessFaceDataTriangle flag\n");
							return A3D_NOT_IMPLEMENTED;
						}
					}

					A3DTessBaseGet(NULL, &sBaseData);
					A3DTess3DGet(NULL, &sData);
				}
				else
				{
					printf("Cannot retrieve the 3D face tesselation\n");
				}

				return iRet;
			}

			// ToDo: Handle normalsPerTriangle
			static Face^ CreateFaceData(int indexesPerEntry, int normalsPerTriangle, A3DUns32 numberOfTriangles, A3DUns32 startTriangulated,
				const A3DTessBaseData& sBaseData, const A3DTess3DData& sData)
			{
				// Dimension for normals and vertex coords
				int dimension = 3;
		
					// The lists of vertex coordonates
				List<Point3D^>^  vertexCoords = gcnew List<Point3D^>(numberOfTriangles * dimension * 3);
				//list of normals
				List<Point3D^>^ normals = gcnew List<Point3D^>(numberOfTriangles * dimension);
				//list o indices
				List<int>^ vertexIndices = gcnew List<int>(numberOfTriangles * 3);

				// Normal and vertex indexes: Array of indexes of points in A3DTessBaseData::m_pdCoords.
				// Case indexesPerEntry = 2 and normalsPerTriangle = 3
				// All the vertex and normal indexes are stored in the A3DTess3D.
				// The A3DTessFaceData::m_usUsedEntitiesFlags describe how to interpret these indexes.
				// They are stored as follows for triangle data:
				// N = normal index
				// P = point coordinates index
				// N, P		N, P		N, P		N, P		N, P		N,  P		Face 0
				//	\	first triangle /			\		second triangle /
				//
				// Case indexesPerEntry = 3 and normalsPerTriangle = 3
				// All the vertex, normal and texture indexes are stored in the A3DTess3D.
				// The A3DTessFaceData::m_usUsedEntitiesFlags describe how to interpret these indexes.
				// They are stored as follows for triangle data:
				// N = normal index
				// U = UV index for texture
				// P = point coordinates index
				// N, U, P		N, U, P		N, U, P		N, U, P		N, U, P		N, U, P		Face 0
				//		\		first triangle /			\		second triangle /

				// Extract normals:
				for (A3DUns32 ui = startTriangulated; ui < startTriangulated + (indexesPerEntry * numberOfTriangles * 3); ui += indexesPerEntry * 3)
				{
					// Extract normal index
					A3DUns32 normalIndex = sData.m_puiTriangulatedIndexes[ui];

					// Extract normal values
					Point3D^ newPoint = gcnew Point3D(sData.m_pdNormals[normalIndex], sData.m_pdNormals[normalIndex + 1], sData.m_pdNormals[normalIndex + 2]);
					normals->Add(newPoint);

				}

				// Extract vertex coordinates:
				long currentVertex = 0;
				long currentVertexIndex = 0;
				for (A3DUns32 ui = startTriangulated; ui < startTriangulated + (indexesPerEntry * numberOfTriangles * 3); ui += indexesPerEntry)
				{
					// Extract vertex index
					A3DUns32 vertexIndex = sData.m_puiTriangulatedIndexes[ui + indexesPerEntry - 1];

					// Extract vertex coordinates
					Point3D^ newVektor = gcnew Point3D(sBaseData.m_pdCoords[vertexIndex], sBaseData.m_pdCoords[vertexIndex + 1], sBaseData.m_pdCoords[vertexIndex + 2]);

					vertexCoords->Add(newVektor);
					// Set vertex index
					vertexIndices->Add(currentVertexIndex);

					++currentVertexIndex;
				}
				Face^ faceData = gcnew Face(vertexCoords, normals, vertexIndices);

				return faceData;
			}

			static int GetIndexesPerEntry(A3DUns16 flags)
			{
				if (flags & (kA3DTessFaceDataTriangle | kA3DTessFaceDataTriangleOneNormalTextured))
					return 2;
				else if (flags & kA3DTessFaceDataTriangleTextured)
					return 3;
				else if (flags & kA3DTessFaceDataTriangleOneNormal)
					return 1;

				return 0;
			}

			static int GetNormalsPerTriangle(A3DUns16 flags)
			{
				if (flags & (kA3DTessFaceDataTriangleOneNormal | kA3DTessFaceDataTriangleOneNormalTextured))
					return 1;
				else if (flags & (kA3DTessFaceDataTriangle | kA3DTessFaceDataTriangleTextured))
					return 3;

				return 0;
			}

		private:
			// Fields:


			static List<Element3D^>^ allElements;
			bool stbA3DLoaded = false;
			FILE* stOut = stdout;
			int stiSize = 0;
			static bool Initialized = false;
			static bool IsPart = false;
			static List<ColorDefinition^>^ allStoredColors;
			A3DSDKHOOPSExchangeLoader* sHoopsExchangeLoader;
		};
	}
}
