#pragma once
#include "Point3D.h"
#include "ColorDefinition.h"
#include "Element3D.h"
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
	namespace Pdf3DReader {

		/// <summary>
		/// The equivalent of an A3DTessFaceData but also comprising normals, vertex coordinates and indexes.
		/// The current implementation is limited to triangle based tesselations with its vertices colors.
		/// </summary>
		public ref class Face : Element3D
		{

#pragma region backend private fields
		private:
			List<Point3D^>^  vertexCoords;
			List<Point3D^>^  normals;
			List<int>^ indices;
			ColorDefinition^ color;
#pragma endregion

#pragma region  Constructors

		public:
			/// <summary>
			/// Constructor for initialization of a part object without color
			/// </summary>
		Face(List<Point3D^>^  vertexCoords, List<Point3D^>^  normals, List<int>^ vertexIndices)
			{
				VertexCoords = vertexCoords;
				Normals = normals;
				VertexIndices = vertexIndices;
				Type = Element3DType::_Face;
			}

		/// <summary>
		/// Constructor for initialization of a part object with color
		/// </summary>
		Face(List<Point3D^>^  vertexCoords, List<Point3D^>^  normals, List<int>^ vertexIndices, ColorDefinition^ faceColorDefinition)
		{
			VertexCoords = vertexCoords;
			Normals = normals;
			VertexIndices = vertexIndices;
			Color = faceColorDefinition;
			Type = Element3DType::_Face;
		}
#pragma endregion

#pragma region Destructor
		/// <summary>
		/// Destructor :
		/// destroys all created properties
		/// </summary>
	~Face()
		{
		try
		{
			delete VertexCoords;
			delete Normals;
			delete VertexIndices;
			delete vertexCoords;
			delete indices;
			delete normals;
			delete Color;
			delete color;
		}
		catch (const std::exception&)
		{
			printf("One or more porperties you want to delete are not initialized");
		}
		
		}
#pragma endregion
	
#pragma region Semi-public properties 
	/// <summary>
	/// Color property of the face
	/// </summary>
			property ColorDefinition^ Color
			{
				ColorDefinition^ get() { return color; }
			
				void set(ColorDefinition^ value) { color = value; }
			}
			/// <summary>
			/// Get/set the 0-based vertex coordinates. 
			/// </summary>
			property  List<Point3D^>^  VertexCoords
			{
				List<Point3D^>^ get() { return vertexCoords; }
			private :
				void set(List<Point3D^>^ value) { vertexCoords = value; }
			}

			

			/// <summary>
			/// Get/set the 0-based normals. 
			/// </summary>
			property List<Point3D^>^  Normals
			{
				List<Point3D^>^ get() { return normals; }
			private:
				void set(List<Point3D^>^ value) { normals = value; }
			}

			/// <summary>
			/// Get/set the vertex index per triangle.
			/// </summary>
			property List<int>^ VertexIndices
			{
				List<int>^ get() { return indices; }
			private:
				void set(List<int>^ value) { indices = value; };
			}
#pragma endregion
		};
	}
}
