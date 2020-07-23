#pragma once
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
#pragma region Point3D declaration
		/// <summary>
		/// Point object
		/// </summary>
		public ref class Point3D
		{
#pragma region Backend Fields
		private:
			double^ x;
			double^ y;
			double^ z;
#pragma endregion

#pragma region Contructors
		public:
			/// <summary>
			///  Constructor for creation of a copied Point3D
			/// </summary>
			/// <param name="copyElement">Point3D to copy</param>
			Point3D(Point3D^ copyElement)
			{
				X = copyElement->X;
				Y = copyElement->Y;
				Z = copyElement->Z;
			}
			/// <summary>
			/// Constructor for creation of a new Point3D with its Coordinates
			/// </summary>
			/// <param name="x">X value of the point</param>
			/// <param name="y"> Y Value of the point </param>
			/// <param name="z"> Z Value of the point</param>
			/// <returns></returns>
		Point3D(double^ x, double^ y, double^ z)
			{
				X = x;
				Y = y;
				Z = z;
			}
#pragma endregion

#pragma region Destructor
		/// <summary>
		/// Destructor :
		/// destroys all created properties and fields
		/// </summary>
		~Point3D()
		{
			try
			{
				delete X;
				delete Y;
				delete Z;
				delete x, y, z;
			}
			catch (const std::exception&)
			{
				printf("One or more properties or fields could not be deleted, because they were never initialized");
			}

		}

#pragma endregion

#pragma region Semi-public properties

		/// <summary>
		/// X value of the point
		/// </summary>
			property  double^ X
			{
				double^ get() { return x; }
			private:
				void set(double^ value) { x = value; }
			}

			/// <summary>
			/// Y value of the point
			/// </summary>
			property  double^ Y
			{
				double^ get() { return y; }
			private:
				void set(double^ value) { y = value; }
			}

			/// <summary>
			/// Z value of the point
			/// </summary>
			property  double^ Z
			{
				double^ get() { return z; }
			private:
				void set(double^ value) { z = value; }
			}
#pragma endregion
	};

#pragma endregion 
	}
}