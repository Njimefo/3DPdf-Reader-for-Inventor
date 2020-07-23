using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using  System.Xaml;
using Microsoft.Win32;
using  SimilaritySearch.Pdf3DReader;
using  System.IO;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;


namespace SimilaritySearch.Visualization
{
    /// <summary>
    /// Interaction logic for Visualizer3D.xaml
    /// </summary>
    public partial class Visualizer3D : UserControl
    {
        private VisualizerViewModell viewModell;
        private string ObjectName = null;
        public Visualizer3D()
        {
            InitializeComponent();
            this.DataContext = viewModell;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem clicked = sender as System.Windows.Controls.MenuItem;
            if (clicked.Header.ToString().Contains("Oeffnen"))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "3D PDF Dateien (*.pdf)|*.pdf*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    if (openFileDialog.FileNames.Length > 1)
                        MessageBox.Show("Es darf nur eine Datei geladen werden", "Ladefehler", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else if (openFileDialog.FileNames.Length == 1)
                    {
                        string path = openFileDialog.FileNames[0];
                        var service = new Pdf3DReaderService();
                        List<Element3D> elements = null;
                        service.ReadPdf3D(path, out elements);
                        if (elements.Count == 1)
                        {
                            if(trvMenu.Items.Count>0)
                            trvMenu.Items.Clear();
                            if ((Element3DType) elements[0].Type == Element3DType._Part)
                            {
                                Part extractePart = (Part) elements[0];
                             FileInfo info = new FileInfo(path);
                                string name = info.Name.Split('.')[0];
                                ObjectName = name;
                               StlConverter newConverter = new StlConverter(extractePart, name);
                               string savePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), name + ".stl");
                                viewModell = new VisualizerViewModell(savePath,true);
                                visualizer.Content = viewModell.Model;
                                TreeViewItem item = new TreeViewItem();
                                item.Header = name;
                                trvMenu.Items.Clear();
                                trvMenu.Items.Add(item);
                        trvMenu.SelectItem(item);

                            }
                          else  if ((Element3DType) elements[0].Type == Element3DType._Assembly)
                            {
                                AssemblyDefinition extracteAssembly = (AssemblyDefinition)elements[0];
                                FileInfo info = new FileInfo(path);
                                string name = info.Name.Split('.')[0];
                                ObjectName = name;
                                StlConverter newConverter = new StlConverter(extracteAssembly, name);
                                bool FromMain = true;
                                List<string> clone = null;
                                GenerateTreeView(newConverter.occurenceRef, name, ref clone,ref FromMain);

                            }
                            else
                            {
                                    
                            }
                        }
                        else if (elements.Count > 1)
                        {
                            
                        }
                    }
                }

            }
            else if (clicked.Header.ToString().Contains("Speichern"))
            {
                
            }

        }

        void GenerateTreeView(Dictionary<int, object> Ref, string Name,ref List<string> element,ref bool FromMainTree)
        { 
         
            foreach (object elemnt in Ref.Values)
            {
                try
                {
                    int position = (int)elemnt;
                    if (FromMainTree)
                    {
                       
                        TreeViewItem newItem = new TreeViewItem();
                        newItem.Header = "Part_" + position;
                        trvMenu.Items.Add(newItem);
                    }
                    else
                    {
                            element.Add("Part_" + position);
                    }
    
                }
                catch (Exception e)
                {
                    Dictionary<int, object> subElements =(Dictionary<int, object>)elemnt;
                    TreeViewItem newView = new TreeViewItem();
                    newView.Header = "Subassembly";
                    List<string> subElement = new List<string>();
                    FromMainTree = false;
                    GenerateTreeView(subElements,Name,ref subElement,ref FromMainTree);
                    newView.ItemsSource = subElement.ToArray();
                    trvMenu.Items.Add(newView);
                    FromMainTree = true;

                }
            
            }
        }

        private void trvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;

          
            if (tree.SelectedItem is TreeViewItem)
            {
                if (((TreeViewItem) tree.SelectedItem).Header.ToString().Contains(("Part")))
                {
                    string position = ((TreeViewItem)tree.SelectedItem).Header.ToString().Split('_')[1];
                    string savePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), ObjectName+"\\"+ ObjectName + "_" + position + ".stl");
                    viewModell = new VisualizerViewModell(savePath, true);
                    visualizer.Content = viewModell.Model;

                }
               
            }
            else if (tree.SelectedItem is string)
            {
                string position = tree.SelectedItem.ToString().Split('_')[1];
                string savePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), ObjectName + "\\" + ObjectName + "_" + position + ".stl");
                viewModell = new VisualizerViewModell(savePath, true);
                visualizer.Content = viewModell.Model;

            }

        }
    }
}
