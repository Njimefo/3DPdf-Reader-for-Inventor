using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimilaritySearch.Visualization
{
    public static class Extensions
    {

        /// <summary>
        /// Select specified item in a TreeView
        /// </summary>
        public static void SelectItem(this TreeView treeView, object item)
        {
            var tvItem = treeView.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
            if (tvItem != null)
            {
                tvItem.IsSelected = true;
            }
        }
    }
}
