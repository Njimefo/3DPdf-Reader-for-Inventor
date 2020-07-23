#pragma once
#include "LocationData.h"
#include "ColorDefinition.h"
#include "Part.h"
#include "AssemblyDefinition.h"
//#include "AssemblyDefinition.h"
namespace SimilaritySearch
{

	namespace Pdf3DReader
	{
		public ref class Occurence
		{
#pragma region Backend fields
		private:
			Part^ part;
			bool^ colorUsed;
			ColorDefinition^ color;
			AssemblyDefinition^ subAss;
			LocationData^ location;
			bool partreferenced;
#pragma endregion 

#pragma region Constructors
		public:
			/// <summary>
			/// Constructor for creation of a new SubAssembly 
			/// </summary>
			/// <param name="OccurenceAssembly">SubAssembly to which the occurence points</param>
			/// <param name="location">Location of pointed SubAssembly</param>
			Occurence(AssemblyDefinition ^ OccurenceAssembly/*, LocationData ^ location*/)
			{
				referencedAssemblyDefinition = OccurenceAssembly;
				/*occurenceLocation = location;*/
				PartIsReferenced = false;
			}
			/// <summary>
			/// Constructor for creation of a new Assembly 
			/// </summary>
			/// <param name="OccurenceAssembly">Part to which the occurence points</param>
			/// <param name="location">Location of pointed Part</param>
			Occurence(Part ^ part/*, LocationData ^ location*/)
			{
				referencedPart = part;
				/*occurenceLocation = location;*/
				PartIsReferenced = true;
			}

			///// <summary>
			///// Default Constructor of an occurence
			///// </summary>
			//Occurence()
			//{
			//
			//}
#pragma endregion

#pragma region Destructor
			/// <summary>
			/// Destructor :
			/// destroys all created elements
			/// </summary>
			~Occurence()
			{
				try
				{
					delete subAss;
					delete  referencedPart;
					delete referencedAssemblyDefinition;
					delete part;
					delete occurenceLocation;
					/*delete PartIsReferenced;*/
					delete /*partreferenced,*/ location;
				}
				catch (const std::exception&)
				{
					printf("One or more properties or fields could not be deleted, because they were not initialized");
				}

			}
#pragma endregion

#pragma region Semi-public fields

			/// <summary>
			/// Part which is pointed by the occurence
			/// </summary>
			property	Part^ referencedPart
			{
				Part^ get()
				{
					return PartIsReferenced ? part : throw new exception("The Assembly is referenced");
				}
			private:
				void set(Part^ value)
				{
					part = value;
					PartIsReferenced = true;
				}
			}

			/// <summary>
			/// SubAssembly which is pointed by the occurence
			/// </summary>
			property	AssemblyDefinition^ referencedAssemblyDefinition
			{
				AssemblyDefinition^ get()
				{
					return !PartIsReferenced ? subAss : throw new exception("The Part is referenced");
				}
			private:
				void set(AssemblyDefinition^ value)
				{
					subAss = value;
					PartIsReferenced = false;
				}
			}
			/// <summary>
			/// LocationData of the Occurence (Part or SubAssembly)
			/// </summary>
			property	LocationData^ occurenceLocation
			{
				LocationData^ get() { return location; }
			internal:
				void set(LocationData^ value) { location = value; }
			}

			property ColorDefinition^ Color
			{
				ColorDefinition^ get() { if (colorUsed) return color; else throw new exception("There is no global color at this occurence"); }
			internal:
				void set(ColorDefinition^ value)
				{
					color = value;
					if(PartIsReferenced)
					{
						referencedPart->Color = color;
					}
					else
					{
							
					}
				}
			}

			/// <summary>
			/// Determines if the Part is referenced or not
			/// </summary>
			property    bool PartIsReferenced
			{
				bool get() {
					return partreferenced;
				}
			private:
				void set(bool value) { partreferenced = value; }
			}
#pragma endregion
		};
	}
}

