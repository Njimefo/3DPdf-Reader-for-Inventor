#pragma once
#include <msclr\marshal_cppstd.h>
#include "Element3DType.h"
//#include <msclr/marshal.h>
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
		/// <summary>
		/// Global abstract class of an 3D Object which can be either a Face, or an Assembly, or a Part, dependending on  what it is being used for
		/// </summary>
		public ref class Element3D abstract
		{

#pragma region Backend field
		protected :
			/// <summary>
			/// Backend field of the property \ref Type
			/// </summary>
			Element3DType^ typ;
#pragma endregion

#pragma region Public properties
		public:
			property Element3DType^ Type
			{
				/// <summary>
				/// gets the value of the property from the backend field \ref typ
				/// </summary>
				/// <returns>value of the property</returns>
				Element3DType^ get()
				{
					return typ;
				}
			public:
			
				/// <summary>
				/// Sets the value of the property which is going to be saved into the backend field \ref typ 
				/// </summary>
				/// <param name="value">Value of property</param>
				void set(Element3DType^ value)
				{
					typ = value;
				}

			}
#pragma endregion

		};

	}
}

