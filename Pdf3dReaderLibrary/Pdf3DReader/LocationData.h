#pragma once
#include "Point3D.h"
#include <msclr\marshal_cppstd.h>
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
		public ref class  LocationData
		{
#pragma region Backend fields
		private :
			Point3D^ rotation;
			Point3D^ positioning;
			cli::array<double^, 2>^ locationMatrix;

#pragma endregion 
		public:
#pragma region Constructor
			/// <summary>
			/// Constructor for creation of a new Location Data
			/// </summary>
			/// <param name="rotation">Rotaion value of the object relating to its Container (eg: Assembly)</param>
			/// <param name="positioning">Position value of the object relating to its Container (eg: Assembly)r</param>
			LocationData(/*Point3D^ rotation, Point3D^ positioning,*/ cli::array<double^, 2>^ location)
			{
	/*			Rotation = rotation;
				Positioning = positioning;*/
				LocationMatrix = location;
			}
#pragma endregion

#pragma region Destructor
			/// <summary>
			/// Destructor :
			/// destroys all created elements
			/// </summary>
			~LocationData()
			{
				try
				{
					delete Rotation;
					delete Positioning;
					delete positioning, rotation;
				}
				catch (const std::exception&)
				{
					printf("One or more properties could be deleted, because it has not been initialized");
				}

			}
	
#pragma endregion

#pragma region Semi-public properties


			/// <summary>
			/// Matrix Location of the occurence
			/// </summary>
			property	cli::array<double^, 2>^ LocationMatrix
			{
				cli::array<double^, 2>^ get() { return locationMatrix; }
			private:
				void  set(cli::array<double^, 2>^ value) { locationMatrix = value; }
			}

			/// <summary>
			/// Rotation value of the element relating to its container
			/// </summary>
		property	Point3D^ Rotation
		{
			Point3D^ get() { return rotation; }
		private:
			void  set(Point3D^ value) { rotation = value; }
		}

		/// <summary>
		/// Position value of the element relating to its container
		/// </summary>
		property    Point3D^ Positioning
		{
			Point3D^ get() { return positioning; }
		private:
			void  set(Point3D^ value) { positioning = value; }
		}
#pragma endregion

		};
	}
}