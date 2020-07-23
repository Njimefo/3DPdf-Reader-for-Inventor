#pragma once
#include "Element3D.h"
#include "Face.h"
#include<vector>
#include <msclr\marshal_cppstd.h>
#include "LocationData.h";

using namespace std;
using namespace msclr::interop;
using namespace System;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Text;
using namespace System::Runtime::InteropServices;



namespace SimilaritySearch
{
	namespace Pdf3DReader
	{
		public ref class Part : Element3D
		{
#pragma region Backend fields
		private :
			List<Face^>^ faces;
			ColorDefinition^ color;
			bool hasColor= false;
#pragma endregion

#pragma region Contructor
		public:
			/// <summary>
			/// Constructor for creation of a new part with its faces
			/// </summary>
			/// <param name="faces">List of all Faces of the part</param>
			Part(List<Face^>^ faces)
			{
				Faces = faces;
				Type = Element3DType::_Part;;
			}
			Part(List<Face^>^ faces,ColorDefinition^ partColor)
			{
				Faces = faces;
				Type = Element3DType::_Part;
				Color = partColor;
				hasColor = true;
			}
			//Part()
			//{
			//	Faces = gcnew List<Face^>();
			//	Type = Element3DType::_Part;;
			//}
#pragma endregion

#pragma region Destructor
			/// <summary>
			/// Destructor
			/// destroys all created properties and fields from memory
			/// </summary>
			~Part()
			{
				try
				{
					delete Faces;
					delete faces;
					delete color;
					delete Color;
				}
				catch (const std::exception&)
				{
					printf("One or more properties or fields could not be deleted, because it has not been initialized");
				}

			}
#pragma endregion

#pragma region Semi-public properties

			/// <summary>
			/// List of faces of the part
			/// </summary>
			property List<Face^>^ Faces
			{
				List<Face^>^ get() { return faces; }
			private:
				void set(List<Face^>^ value) { faces = value; }
			}
			/// <summary>
			/// Color of the whole part
			/// </summary>
			property ColorDefinition^ Color
			{
				ColorDefinition^ get()
				{
					if(hasColor)return color;
					else
					{
 				throw new exception("This part bears any color.");
					}
				}
			internal:
				void set(ColorDefinition^ value)
				{
					color = value;
					for each (Face^ face in Faces)
					{
						face->Color = value;

					}

				}
			}
#pragma endregion

		};

	}
}