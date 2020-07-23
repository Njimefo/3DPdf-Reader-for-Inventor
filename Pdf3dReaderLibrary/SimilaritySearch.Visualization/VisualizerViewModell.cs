using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SimilaritySearch.Visualization.Annotations;
using HelixToolkit.Wpf;

namespace SimilaritySearch.Visualization
{
   public class VisualizerViewModell : INotifyPropertyChanged
   {
       private Model3D model;
       private StLReader reader;
       private bool ISPart;
        private Model3DGroup modelGroup = new Model3DGroup();
       private List<MeshBuilder> meshes;
       Material standardMaterial = MaterialHelper.CreateMaterial(Colors.DarkGray);
        public StLReader stlReader
       {
           get { return reader; }
         private  set
           {
               reader = value;
              
           }
       }

       public VisualizerViewModell(string path,bool isPart = false)
       {
           if (isPart)
           {
               ISPart = isPart;
               stlReader = new StLReader();
               stlReader.Read(path);
               meshes = (List<MeshBuilder>)stlReader.Meshes;
               modelGroup.Children.Add(new GeometryModel3D
               {
                   Geometry = meshes[0].ToMesh(true),
                   Material = standardMaterial,
                   BackMaterial = standardMaterial
               });
               Model = modelGroup;
            }
         

       }
       public Model3D Model
       {
           get { return model; }
         private  set
           {
               model = value;
               OnPropertyChanged();
           }
       }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
