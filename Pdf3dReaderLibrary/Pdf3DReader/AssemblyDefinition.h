#pragma once
#include "Element3D.h"
#include "ColorDefinition.h"
//#include "Occurence.h"
//#include <msclr\marshal_cppstd.h>

//#include <msclr/marshal.h>
using namespace std;
using namespace msclr::interop;
using namespace System;
using namespace System::Collections::Generic;

namespace SimilaritySearch
{

	namespace Pdf3DReader
	{
		ref class Occurence;
		/// <summary>
		/// Reepresents an Assembly with its sub-elements 
		/// </summary>
		public ref class AssemblyDefinition :
			public Element3D
		{

#pragma region private backend field
		private:

			ColorDefinition^ color;
			bool colorUsed;

			/// <summary>
			/// Backend field of the prpoerty \ref Occurences
			/// </summary>
			List<Occurence^>^ occurences;
#pragma endregion

#pragma region Constructor 
		public:


			/// <summary>
			/// Constructor for Initilizations 
			/// </summary>
			/// <param name="Occ"> Occurences list of the assembly which is being created</param>
			AssemblyDefinition(List<Occurence^>^ Occ)
			{
				Occurences = Occ;
				Type = Element3DType::_Assembly;
			}

			///// <summary>
			///// Default Constructor
			///// </summary>
			//AssemblyDefinition()
			//{
			//	Occurences = gcnew List<Occurence^>();
			//	Type = Element3DType::_Assembly;
			//
			//}
#pragma endregion

#pragma region Destructor

			/// <summary>
			/// Destructor : 
			/// destroys all created properties
			/// </summary>
			~AssemblyDefinition()
			{
				try
				{
					delete occurences;

					delete Occurences;
				}
				catch (const std::exception&)
				{
					printf("One or more porperties you want to delete are not initialized");


				}

			}
#pragma endregion

#pragma region Semi-public properties

			property	ColorDefinition^ Color
			{
				ColorDefinition^ get()
				{
					if (colorUsed)return color;
					else throw new exception("Color is not used global");
				}
			internal:
				void  set(ColorDefinition^ value)
				{
					color = value;
					colorUsed = true;
				
					//TODO: to discuss with Herr Neumann
					//for each (Occurence^ occ in Occurences)
					//{
					//	occ->Color = color;
					//}
				}
			}

			/// <summary>
			/// List of all occurences oft the current Assembly object
			/// </summary>
			property List<Occurence^>^ Occurences
			{
				/// <summary>
				/// gets the value of the saved value(of the list above) from the backend field 
				/// </summary>
				/// <returns>List of Occurences</returns>
				List<Occurence^>^ get()
				{
					return occurences;
				}
			internal:

				/// <summary>
				///   Sets the value of the Occurences List to save
				/// </summary>
				/// <param name="Occ">List of Occurences to save</param>
				void set(List<Occurence^>^ Occ)
				{
					occurences = Occ;


				}
			}
#pragma endregion
		};
	}
}

