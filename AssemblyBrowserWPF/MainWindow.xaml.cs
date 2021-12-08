using AssemblyBrowserProject;
using System.Windows;
using System.Windows.Controls;

namespace AssemblyBrowserWPF
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
			ApplicationViewModel avm = new ApplicationViewModel();
			avm.hWnd = this;
			DataContext = avm;
        }

		public void GetTreeFilled(TreeComponent tree)
        {
			trvStructure.Items.Clear();
			trvStructure.Items.Add(CreateTreeItem(tree.GetTreeComposite()));
        }

		public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
		{
			TreeViewItem item = e.Source as TreeViewItem;
			if ((item.Items.Count == 1) && (item.Items[0] is string))
			{
				item.Items.Clear();

				TreeComposite expandedNode = null;
				if (item.Tag is TreeComposite)
				{
					expandedNode = (item.Tag as TreeComposite).GetTreeComposite();

					foreach (TreeComponent subDir in expandedNode.components)
						item.Items.Add(CreateTreeItem(subDir));
				}
			}
		}

		private TreeViewItem CreateTreeItem(object o)
		{
			TreeViewItem item = new TreeViewItem();
			item.Header = o.ToString();
			item.Tag = o;
			if (o is TreeComposite) item.Items.Add("Loading...");
			return item;
		}
	}
}
