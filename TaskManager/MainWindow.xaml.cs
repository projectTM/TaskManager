using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManager
{
    public partial class MainWindow : Window
    {
        public List<Container> m_containers;
        public int currentIndex;
        public NewTask newTask;
        public NewContainer newContainer;

        public MainWindow()
        {
            InitializeComponent();
            m_containers = new List<Container>();

            ComboBoxItem cbBoxItem = new ComboBoxItem();
            cbBoxItem.Content = "Boite de réception";
            comboBox.Items.Add(cbBoxItem);
            ComboBoxItem cbBoxItem1 = new ComboBoxItem();
            cbBoxItem1.Content = "Prioritaire";
            comboBox.Items.Add(cbBoxItem1);
            ComboBoxItem cbBoxItem2 = new ComboBoxItem();
            cbBoxItem2.Content = "Journée";
            comboBox.Items.Add(cbBoxItem2);

            comboBox.SelectedIndex = 0;
            currentIndex = 0;

            Container container = new Container();
            container.Name = cbBoxItem.Content.ToString();
            m_containers.Add(container);
            m_containers[0].addItem(null, "Task0", "01/01/2018", "01/02/2018", "testComment0.0");
            unselectItem();
            m_containers[0].m_Tasks[0].IsSelected = true;
            m_containers[0].addItem(m_containers[0].m_Tasks[0], "SubTask0.0", "01/01/2016", "01/02/2016", "testComment0.0");
            m_containers[0].addItem(null, "Task1", "01/01/2019", "01/02/2019", "testComment1.0");
            unselectItem();
            m_containers[0].m_Tasks[1].IsSelected = true;
            m_containers[0].addItem(m_containers[0].m_Tasks[1], "SubTask1.0", "02/01/2016", "02/02/2016", "testComment1.0");
            m_containers[0].addItem(m_containers[0].m_Tasks[1], "SubTask1.1", "03/01/2016", "03/02/2016", "testComment1.1");
            m_containers[0].addItem(m_containers[0].m_Tasks[1], "SubTask1.2", "04/01/2016", "04/02/2016", "testComment1.2");

            Container container1 = new Container();
            container1.Name = cbBoxItem1.Content.ToString();
            m_containers.Add(container1);
            Container container2 = new Container();
            container2.Name = cbBoxItem2.Content.ToString();
            m_containers.Add(container2);

            treeView.ItemsSource = m_containers[0].m_Tasks;
        }

        private void unselectItem()
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                selected.IsSelected = false;
            }
        }

        private void saveTask_Click(object sender, RoutedEventArgs e)
        {
            // Save BDD
        }

        private void newTask_OnSaveNewTask(object sender, RoutedEventArgs e)
        {
            string content = new TextRange(newTask.comment.Document.ContentStart, newTask.comment.Document.ContentEnd).Text;
            m_containers[currentIndex].addItem(null, "Tache", newTask.beginDatePicker.Text, newTask.endDatePicker.Text, content);
            newTask.Close();
        }

        private void addTask_click(object sender, RoutedEventArgs e)
        {
            newTask = new NewTask();
            newTask.Show();
            newTask.OnSaveEvent += new RoutedEventHandler(newTask_OnSaveNewTask);
        }

        private void addSubTask_click(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
            }
            newTask = new NewTask();
            newTask.Show();
            newTask.OnSaveEvent += new RoutedEventHandler(newTask_OnSaveNewTask);
        }


        private void remTask_click(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            m_containers[currentIndex].removeItem(selected);
        }

        private void OnSelectTreeViewItem(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                if (selected.dateBegin != null)
                    beginDatePicker.SelectedDate = DateTime.Parse(selected.dateBegin);
                if (selected.dateEnd != null)
                    endDatePicker.SelectedDate = DateTime.Parse(selected.dateEnd);
                if (selected.comment != null)
                {
                    comment.Document.Blocks.Clear();
                    comment.Document.Blocks.Add(new Paragraph(new Run(selected.comment)));
                }
            }
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                System.Windows.Controls.Primitives.DatePickerTextBox datePickerTextBox = FindVisualChild<System.Windows.Controls.Primitives.DatePickerTextBox>(datePicker);
                if (datePickerTextBox != null)
                {

                    ContentControl watermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
                    if (watermark != null)
                    {
                        watermark.Content = "                ";
                    }
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject depencencyObject) where T : DependencyObject
        {
            if (depencencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depencencyObject); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depencencyObject, i);
                    T result = (child as T) ?? FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private void comment_TextInput(object sender, TextCompositionEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                string content = new TextRange(comment.Document.ContentStart, comment.Document.ContentEnd).Text;
                selected.comment = content;
            }
        }

        private void comment_KeyUp(object sender, KeyEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                string content = new TextRange(comment.Document.ContentStart, comment.Document.ContentEnd).Text;
                selected.comment = content;
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("changed");
            //Load tree for container
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
            for(int i = 0; i < m_containers.Count; ++i)
            {
                //MessageBox.Show(item.Content.ToString());
                if (m_containers[i].Name == item.Content.ToString())
                {
                    treeView.ItemsSource = m_containers[i].m_Tasks;
                    this.currentIndex = i;
                    break;
                }

            }
        }

        private void endDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                selected.dateEnd = endDatePicker.Text;
            }
        }

        private void beginDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                selected.dateBegin = beginDatePicker.Text;
            }
        }

        private void newContainer_addContainer(object sender, RoutedEventArgs e)
        {
            Container container = new Container();
            container.Name = newContainer.collectionName.Text;
            ComboBoxItem cbBoxItem = new ComboBoxItem();
            cbBoxItem.Content = newContainer.collectionName.Text;
            comboBox.Items.Add(cbBoxItem);
            m_containers.Add(container);
            newContainer.Close();
        }
        
        private void addContainer_Click(object sender, RoutedEventArgs e)
        {
            newContainer = new NewContainer();
            newContainer.Show();
            newContainer.OnAddEvent += new RoutedEventHandler(newContainer_addContainer);
        }

    }

    public class Container
    {
        public string Name;
        public ObservableCollection<CustomTreeViewItem> m_Tasks { get; set; }

        public Container()
        {
            this.m_Tasks = new ObservableCollection<CustomTreeViewItem>();
        }

        public void removeItem(CustomTreeViewItem item)
        {
            int i = 0;
            int index = -1;
            while (i < m_Tasks.Count && index < 0)
            {
                index = m_Tasks[i].Items.IndexOf(item);
                ++i;
            }

                if (item.parent != null && index >= 0)
                {
                    int parent_index = m_Tasks.IndexOf(item.parent);
                    m_Tasks[parent_index].Items.Remove(item);
                }
                else if (item.parent == null)
                {
                    m_Tasks.Remove(item);
                }

        }

        public void addItem(CustomTreeViewItem parent, string title, string beginDate, string endDate, string comment)
        {
            CustomTreeViewItem item = new CustomTreeViewItem(title);
            item.parent = parent;
            item.comment = comment;
            item.dateBegin = beginDate;
            item.dateEnd = endDate;
            item.IsExpanded = true;
            if (parent != null)
            {
                int index = (m_Tasks.IndexOf(parent) >= 0) ? m_Tasks.IndexOf(parent) : 0;
                m_Tasks[index].Items.Add(item);
            }
            else
                m_Tasks.Add(item);
            /*if (selected != null)
                selected.Items.Add(childItem);
            else
                m_Tasks[index].Items.Add(childItem);*/
        }
    }

    public class CustomTreeViewItem : TreeViewItem
    {
        public CustomTreeViewItem(string title)
        {
            Title.Text = title;
            chkBox.Content = Title;
            Title.AddHandler(TextBlock.MouseDownEvent, new MouseButtonEventHandler(change_label));
            chkBox.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkBox_Checked));
            Header = chkBox;
        }

        private void change_label(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //MessageBox.Show("dbclick");
                TextBox textBox = new TextBox();
                textBox.Text = Title.Text;

                chkBox.Content = textBox;
                textBox.KeyDown += (o, ev) =>
                {
                    if (ev.Key == Key.Enter || ev.Key == Key.Escape)
                        Title.Text = textBox.Text;
                    ev.Handled = true;
                };
                textBox.LostFocus += (o, ev) =>
                {
                    Title.Text = textBox.Text;
                    ev.Handled = true;
                };
            }
            else
                this.IsSelected = true;
            e.Handled = true;
        }

        private void chkBox_Checked(object sender, RoutedEventArgs e)
        {
            this.IsSelected = true;
        }

        public CustomTreeViewItem parent;
        public CheckBox chkBox = new CheckBox();
        public TextBlock Title = new TextBlock();
        public string comment;
        public string dateBegin;
        public string dateEnd;
    }
}