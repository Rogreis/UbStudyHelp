using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UbStudyHelp.Classes
{
    public class TreeSyncronizer
    {
        private TreeView tree1, tree2;

        public TreeSyncronizer(TreeView tree1, TreeView tree2)
        {
            this.tree1 = tree1;
            this.tree2 = tree2;
        }



        private ItemsControl GetTreeview(TreeViewItem node)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(node);
            while (parent != null && parent.GetType() == typeof(TreeViewItem))
            {
                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }
            return parent;
        }


        //private void ExpandColapse(TreeViewItem node, bool IsExpanded)
        //{
        //    if (node != null && node.Tag != null)
        //    {
        //        ItemsControl itemsControl = GetTreeview(node);
        //        TOC_Entry entry = node.Tag as TOC_Entry;
        //        entry.IsExpanded = IsExpanded;
        //        TreeView tree = node.TreeView == tree1 ? tree2 : tree1;
        //        ExpandColapseTreeNode(tree.Nodes, entry);
        //    }

        //    //TreeViewItem newChild = new TreeViewItem();
        //    //newChild.Header = "zxczczxczx";
        //    //Parent.Items.Add(newChild);
        //}

        //private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)

        //{

        //    MessageBox.Show(indexOfSelectedNode.ToString());

        //}

        //public void SelectTreeNode(ItemCollection nodes, TOC_Entry index)
        //{
        //    foreach (TreeViewItem node in nodes)
        //    {
        //        TOC_Entry indCurrent = node.Tag as TOC_Entry;
        //        if (indCurrent == index)
        //        {
        //            node. .TreeView.SelectedNode = node;
        //            return;
        //        }
        //        else if (node.Nodes != null) SelectTreeNode(node.Nodes, index);
        //    }
        //}

        ///// <summary>
        ///// Once a tree view node is expanded/colapsed, do the same for the other
        ///// </summary>
        ///// <param name="tree"></param>
        ///// <param name="entry"></param>
        //public void ExpandColapseTreeNode(ItemCollection nodes, TOC_Entry entry)
        //{
        //    foreach (TreeViewItem node in nodes)
        //    {
        //        if (node.Tag != null)
        //        {
        //            TOC_Entry entry2 = node.Tag as TOC_Entry;
        //            if (entry2 == entry)
        //            {
        //                entry2.IsExpanded = entry.IsExpanded;
        //                if (entry.IsExpanded)
        //                    node.Expand();
        //                else
        //                    node.Collapse();
        //                return;
        //            }
        //            if (node.Nodes != null && node.Nodes.Count > 0)
        //                ExpandColapseTreeNode(node.Nodes, entry);
        //        }
        //    }
        //}


        //public void AfterExpand(TreeViewItem node)
        //{
        //    ExpandColapse(node, true);
        //}

        //public void AfterCollapse(TreeViewItem node)
        //{
        //    ExpandColapse(node, false);
        //}

        //public void AfterSelect(TreeViewItem node)
        //{
        //    if (node != null && node.Tag != null)
        //    {
        //        TOC_Entry entry = node.Tag as TOC_Entry;
        //        if (entry == null) return;
        //        SelectTreeNode(tree1.Nodes, entry);
        //        SelectTreeNode(tree2.Nodes, entry);
        //        EventsControl.FireTOCClicked(entry);
        //        EventsControl.FireSendMessage($"{entry.ParagraphID} selected");
        //    }
        //}


    }
}
