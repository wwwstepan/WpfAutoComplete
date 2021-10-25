using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfAutoComplete {

    public partial class MainWindow : Window {

        const string TipTextBox = "Type for search";
        private Button firstBtnAutoComplete = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // Generating a selection list

            var group = new TreeViewItem { Header = "Food", IsManipulationEnabled = false, Focusable = false };
            SelectionTree.Items.Add(group);
            group.Items.Add(new TreeViewItem { Header = "Apple", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Banana", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Candy", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Salad", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Sauce", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Sandwich", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Soup", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Pie", IsManipulationEnabled = false, Focusable = true });

            group = new TreeViewItem { Header = "Drink", IsManipulationEnabled = false, Focusable = false };
            SelectionTree.Items.Add(group);
            group.Items.Add(new TreeViewItem { Header = "Coffee", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Cola", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Tea", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Water", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Tequila", IsManipulationEnabled = false, Focusable = true });

            group = new TreeViewItem { Header = "Dress", IsManipulationEnabled = false, Focusable = false };
            SelectionTree.Items.Add(group);
            group.Items.Add(new TreeViewItem { Header = "Coat", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Jacket", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Suit", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Sweater", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Pants", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Socks", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Gloves", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Scarf", IsManipulationEnabled = false, Focusable = true });
            group.Items.Add(new TreeViewItem { Header = "Tie", IsManipulationEnabled = false, Focusable = true });
        }

        // 3 Methods for collapse selection tree
        private void ForeachTreeViewItem(ItemCollection col, Action<TreeViewItem> action) {
            foreach (var it in col)
                if (it is TreeViewItem tvi)
                    action(tvi);
        }

        private void TreeViewItemCollapseAll(TreeViewItem item) {
            item.IsSelected = false;
            if (item.Items.Count > 0) {
                item.IsExpanded = false;
                ForeachTreeViewItem(item.Items, TreeViewItemCollapseAll);
            }
        }

        private void SelectionTreeCollapseAll() {
            ForeachTreeViewItem(SelectionTree.Items, TreeViewItemCollapseAll);
        }

        // Selection item of autocomplete list - the text in the textbox is filled in, the selected element is activated in the selection tree
        private void ActivateTreeItem(TreeViewElement tve) {
            SelectionTreeCollapseAll();
            tve.Parent.IsExpanded = true;
            tve.Parent.UpdateLayout();
            tve.Item.IsSelected = true;
            tve.Item.Focus();
        }

        // Add next item to autocomplete list
        private void AddToAutoCompleteList(string text, TreeViewElement treeElem) {
            var button = new Button {
                Content = text,
                Height = 26,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                Cursor = Cursors.Hand,
                FontSize = 14,
                Tag = treeElem,
                Style = Resources["TextAutoCompleteButton"] as Style
            };

            button.Click += (sender, e) => {
                if (sender is Button bt && bt.Tag is TreeViewElement tve) {
                    TextBoxAutoComplete.Text = bt.Content.ToString();
                    BorderAutoComplete.Visibility = Visibility.Collapsed;
                    ActivateTreeItem(tve);
                }
            };

            AutoCompleteList.Children.Add(button);

            if (firstBtnAutoComplete == null)
                firstBtnAutoComplete = button;
        }

        // Handler of keyboard event in textbox - if there is a match, a autocomplete list appears below the textbox
        private void TextBoxAutoComplete_KeyUp(object sender, KeyEventArgs e) {
            if ((e.Key == Key.Down || e.Key == Key.Enter) && firstBtnAutoComplete != null) {
                firstBtnAutoComplete.Focus();
                return;
            }

            BorderAutoComplete.Visibility = Visibility.Collapsed;
            AutoCompleteList.Children.Clear();
            firstBtnAutoComplete = null;

            if (e.Key == Key.Escape)
                return;

            if (TextBoxAutoComplete.Text.Length < 2)
                return;

            var qAdded = 0;
            foreach (var item1lvl in SelectionTree.Items)
                if (item1lvl is TreeViewItem group)
                    foreach (var item2lvl in group.Items)
                        if (item2lvl is TreeViewItem item && item.Focusable
                            && item.Header.ToString().ToLower().Contains(TextBoxAutoComplete.Text.ToLower())
                        ) {
                            AddToAutoCompleteList(item.Header.ToString(), new TreeViewElement() { Item = item, Parent = group });
                            qAdded++;
                        }

            if (qAdded > 0) {
                BorderAutoComplete.Visibility = Visibility.Visible;
                BorderAutoComplete.Height = Math.Min(10, qAdded) * 26;
            } else {
                BorderAutoComplete.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBoxAutoComplete_GotFocus(object sender, RoutedEventArgs e) {
            if (TextBoxAutoComplete.Text == TipTextBox)
                TextBoxAutoComplete.Text = "";
        }

        private void TextBoxAutoComplete_LostFocus(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(TextBoxAutoComplete.Text))
                TextBoxAutoComplete.Text = TipTextBox;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            BorderAutoComplete.Visibility = Visibility.Collapsed;
            try {
                DragMove(); // Moving window on mouse pressing in any point of window
            }
            catch { }
        }
    }

    // An auxiliary class for navigating the selection tree
    public class TreeViewElement {
        public TreeViewItem Item;
        public TreeViewItem Parent;
    }
}