#pragma once
#include "stdafx.h"
#include "ColorDefinition.h"
#include <msclr\marshal_cppstd.h>

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
		/// ColorDefinition Class which represents one color
		/// </summary>
	public	ref class ColorDefinition
		{
#pragma region Backend fields
		private :

			/// <summary>
			/// Backend field of the property \ref Alpha
			/// </summary>
			byte^ alpha;

			/// <summary>
			/// Backend field of the property \ref R
			/// </summary>
			byte^ r;

			/// <summary>
			/// Backend field of the property \ref G
			/// </summary>
			byte^ g;

			/// <summary>
			/// Backend field of the property \ref B
			/// </summary>
			byte^ b;

			/// <summary>
			/// Backend field of the property \ref IsRGB
			/// </summary>
			bool ^ isrgb;
#pragma endregion

#pragma region Constructors
		public:

			/// <summary>
			/// Constructor for creation of a new RGB Color
			/// </summary>
			/// <param name="Red">Red value of the color</param>
			/// <param name="Green">Green value of the color</param>
			/// <param name="Blue">Blue value of the color</param>
			ColorDefinition(byte^ Red, byte^ Green, byte^ Blue)
			{
				R = Red;
				G = Green;
				B = Blue;
				IsRGB = true;
			}

			/// <summary>
			/// Constructor for creation of a new RGBA Color
			/// </summary>
			/// <param name="Red">Red value of the color</param>
			/// <param name="Green">Green value of the color</param>
			/// <param name="Blue">Blue value of the color</param>
			/// <param name="alpha">Alpha value of the color</param>
		ColorDefinition(byte^ Red, byte^ Green, byte^ Blue, byte^ alpha)
			{
				R = Red;
				G = Green;
				B = Blue;
				Alpha = alpha;
				IsRGB = false;
			}
#pragma endregion

#pragma region
		/// <summary>
		/// Destructor :
		/// destroys all created elements from memory
		/// </summary>
		~ColorDefinition()
		{
			try
			{
			
			delete R;
			delete G;
			delete B;
			delete Alpha;
			delete alpha;
			delete r, g, b, isrgb,IsRGB;
			}
			catch (const std::exception&)
			{
				printf("One or more properties have not been initialized");
			}
					
			
		}
#pragma endregion

#pragma region Semi-public properties
		/// <summary>
		/// determines if the Color is of type RGB or RGBA
		/// returns true if it is a RGB Color
		/// returns false if it is a RGBA Color
		/// </summary>
			property bool^ IsRGB
			{
				/// <summary>
				/// gets the bool from the backend field \ref isrgb
				/// </summary>
				/// <returns>value of the property</returns>
				bool^ get()
				{
					return isrgb;
				}
			private:
				/// <summary>
				/// Sets the value of the property which is going to be saved into the backend field \ref isrgb 
				/// </summary>
				/// <param name="value">Value of property</param>
				void set(bool^ value)
				{
					isrgb = value;
				}
			}

			/// <summary>
			/// Red value of the color
			/// </summary>
			property byte^ R
			{
				/// <summary>
				/// gets the value of the property from the backend field \ref r
				/// </summary>
				/// <returns>value of the property</returns>
				byte^ get()
				{
					return r;
				}
			private:
				/// <summary>
				/// Sets the value of the property which is going to be saved into the backend field \ref r 
				/// </summary>
				/// <param name="value">Value of property</param>
				void set(byte^ value)
				{
					r = value;
				}
			}

			/// <summary>
			/// Green value of the color
			/// </summary>
			property byte^ G
			{
				/// <summary>
				/// gets the value of the property from the backend field \ref g
				/// </summary>
				/// <returns>value of the property</returns>
				byte^ get()
				{
					return g;
				}
			private:
				/// <summary>
				/// Sets the value of the property which is going to be saved into the backend field \ref g 
				/// </summary>
				/// <param name="value">Value of property</param>
				void set(byte^ value)
				{
					g = value;
				}

			}

			/// <summary>
			/// Blue value of the color
			/// </summary>
			property byte^ B
			{
				/// <summary>
				/// gets the value of the property from the backend field \ref b
				/// </summary>
				/// <returns>value of the property</returns>
				byte^ get()
				{
				 return b;
				}
			private:
				/// <summary>
				/// Sets the value of the property which is going to be saved into the backend field \ref b 
				/// </summary>
				/// <param name="value">Value of property</param>
				void set(byte^ value)
				{
					b = value;
				}

			}

			/// <summary>
			/// Alpha value of the color
			/// </summary>
			property byte^ Alpha
			{
				/// <summary>
				/// gets the value of the property from the backend field \ref alpha
				/// </summary>
				/// <returns>value of the property</returns>
				byte^ get()
				{
				if (IsRGB) throw new exception("The type of this Color is RGB");
					else return alpha;
				}
			private:
				/// <summary>
				/// Sets the value of the property which is going to be saved into the backend field \ref alpha 
				/// </summary>
				/// <param name="value">Value of property</param>
				void set(byte^ value)
				{
					alpha = value;
				}

			}
#pragma endregion
		};
	}
}

