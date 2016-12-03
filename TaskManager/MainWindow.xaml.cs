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
        public mediametrieEntities bdd;
        public requete req;

        public MainWindow()
        {
            InitializeComponent();
            m_containers = new List<Container>();

            bdd = new mediametrieEntities();
            req = new requete();

            LoadDB();

            /*ComboBoxItem cbBoxItem = new ComboBoxItem();
            cbBoxItem.Content = "Boite de réception";
            comboBox.Items.Add(cbBoxItem);
            ComboBoxItem cbBoxItem1 = new ComboBoxItem();
            cbBoxItem1.Content = "Prioritaire";
            comboBox.Items.Add(cbBoxItem1);
            ComboBoxItem cbBoxItem2 = new ComboBoxItem();
            cbBoxItem2.Content = "Journée";
            comboBox.Items.Add(cbBoxItem2);*/

            comboBox.SelectedIndex = 0;
            remContainer.IsEnabled = false;
            currentIndex = 0;
            /*
            Container container = new Container();
            container.Name = "Boite de réception";// cbBoxItem.Content.ToString();
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
            */
            /*Container container1 = new Container();
            container1.Name = cbBoxItem1.Content.ToString();
            m_containers.Add(container1);
            Container container2 = new Container();
            container2.Name = cbBoxItem2.Content.ToString();
            m_containers.Add(container2);*/

            treeView.ItemsSource = m_containers[0].m_Tasks;
            //comboBox.ItemsSource = m_containers;
        }

        private void LoadDB()
        {
            List<container> list_containers = new List<container>();
            int index_container = 0;
            list_containers = req.getContainer(bdd);

            foreach(container element in list_containers)
            {
                int nbTache = req.getNbTacheContainer(bdd, element.label);
                comboBox.Items.Add(new ComboBoxItem() { Content = element.label });
                m_containers.Add(new Container() { Name = element.label });

                List<taches> list_tasks = new List<taches>();
                int index_task = 0;
                list_tasks = req.getTachesContainer(bdd, element.label);
                foreach(taches task in list_tasks)
                {
                    m_containers[index_container].addItem(null, task.label_tache, task.date_debut.ToString(), task.date_fin.ToString(), task.commentaire, (bool)task.effectuer);
                    m_containers[index_container].m_Tasks[index_task].chkBox.IsEnabled = true;
                    if (task.effectuer == true)
                    {
                        m_containers[index_container].m_Tasks[index_task].chkBox.IsChecked = true;
                    }

                    List<taches> list_subtasks = new List<taches>();
                    list_subtasks = req.getSousTaches(bdd, task.label_tache);
                    foreach(taches subtask in list_subtasks)
                    {
                        m_containers[index_container].addItem(m_containers[index_container].m_Tasks[index_task], subtask.label_tache, subtask.date_debut.ToString(), subtask.date_fin.ToString(), subtask.commentaire, (bool)subtask.effectuer);
                        if (task.effectuer == true)
                        {
                            m_containers[index_container].m_Tasks[index_task].chkBox.IsChecked = true;
                            m_containers[index_container].m_Tasks[index_task].chkBox.IsEnabled = false;
                        }
                    }
                    ++index_task;
                }
                ++index_container;
            }
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
            foreach (Container c in m_containers)
            {
                if (req.getSContainer(bdd, c.Name) == null)
                    req.ajoutContainer(bdd, new container() { label = c.Name });
                foreach (CustomTreeViewItem i in c.m_Tasks)
                {
                    if (i != null)
                    {
                        //MessageBox.Show(c.Name);
                        string parent_task;
                        if (i.parent == null)
                            parent_task = null;
                        else
                            parent_task = i.parent.Name;
                        taches t = new taches() { label_container = c.Name, label_tache = i.Title.Text, label_tache_parent = parent_task, commentaire = i.comment, date_debut = DateTime.Parse(i.dateBegin), date_fin = DateTime.Parse(i.dateEnd), effectuer = i.chkBox.IsChecked };
                        if (req.getSTaches(bdd, i.Title.Text) != null)
                            req.modifTaches(bdd, t);
                        else
                            req.ajoutTaches(bdd, t);
                    }
                }
            }
        }

        private void newTask_OnSaveNewTask(object sender, RoutedEventArgs e)
        {
            string Name = "Tache";
            int i = 0;
            while (m_containers[currentIndex].m_Tasks.IndexOf(m_containers[currentIndex].m_Tasks.Where(p => p.Title.Text == (Name+i)).FirstOrDefault()) != -1)
                ++i;
            Name += i;
            string content = new TextRange(newTask.comment.Document.ContentStart, newTask.comment.Document.ContentEnd).Text;
            m_containers[currentIndex].addItem(null, Name, newTask.beginDatePicker.Text, newTask.endDatePicker.Text, content, false);
            newTask.Close();
        }

        private void addTask_click(object sender, RoutedEventArgs e)
        {
            newTask = new NewTask();
            newTask.Show();
            newTask.OnSaveEvent += new RoutedEventHandler(newTask_OnSaveNewTask);
        }

        private void newTask_OnSaveNewSubTask(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                if (selected.parent != null)
                    selected = selected.parent;
                string content = new TextRange(newTask.comment.Document.ContentStart, newTask.comment.Document.ContentEnd).Text;
                string Name = "Sous-Tache";
                int i = 0;
                while (selected.Contains(Name+i) != -1)
                    ++i;
                Name += i;
                m_containers[currentIndex].addItem(selected, Name, newTask.beginDatePicker.Text, newTask.endDatePicker.Text, content, false);
                newTask.Close();
            }
        }

        private void addSubTask_click(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            if (selected != null)
            {
                if (selected.chkBox.IsEnabled == false && (selected.parent == null || (selected.parent != null && selected.parent.chkBox.IsEnabled == false)))
                {
                    MessageBox.Show("Impossible d'ajouter une Sous-Tache!");
                    return;
                }
                newTask = new NewTask();
                newTask.Show();
                newTask.OnSaveEvent += new RoutedEventHandler(newTask_OnSaveNewSubTask);
            }
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
                statusText.Text = "Taches: " + m_containers[currentIndex].m_Tasks.Count;
                int sTaskCount = 0;
                for (int i = 0; i < m_containers[currentIndex].m_Tasks.Count; ++i)
                    sTaskCount += m_containers[currentIndex].m_Tasks[i].Items.Count;
                statusText.Text += " Sous-Taches: " + sTaskCount;
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
            //Load tree for container
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
            if (item == null)
                return;
            for(int i = 0; i < m_containers.Count; ++i)
            {
                if (m_containers[i].Name == item.Content.ToString())
                {
                    if (i < 3)
                        remContainer.IsEnabled = false;
                    else
                        remContainer.IsEnabled = true;
                    treeView.ItemsSource = m_containers[i].m_Tasks;
                    statusText.Text = "Taches: " + m_containers[i].m_Tasks.Count;
                    int sTaskCount = 0;
                    for (int j = 0; j < m_containers[i].m_Tasks.Count; ++j)
                        sTaskCount += m_containers[i].m_Tasks[j].Items.Count;
                    statusText.Text += " Sous-Taches: " + sTaskCount;
                    if (m_containers[i].m_Tasks.Count > 0)
                        m_containers[i].m_Tasks[0].IsSelected = true;
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

        private void remContainer_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex > 2)
                m_containers.RemoveAt(currentIndex);
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
            currentIndex = 0;
            comboBox.SelectedIndex = 0;
            //MessageBox.Show(item.Content.ToString());
            container c = req.getSContainer(bdd, item.Content.ToString());
            req.supContainer(bdd, c);
            comboBox.Items.Remove(item);
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            CustomTreeViewItem selected = treeView.SelectedItem as CustomTreeViewItem;
            foreach (CustomTreeViewItem item in m_containers[currentIndex].m_Tasks)
            {
                if (searchText.Text == item.Title.Text)
                {
                    selected.IsSelected = false;
                    item.IsSelected = true;
                }
                foreach (CustomTreeViewItem subitem in item.Items)
                {
                    if (searchText.Text == subitem.Title.Text)
                    {
                        selected.IsSelected = false;
                        subitem.IsSelected = true;
                    }
                }
            }
        }
    }

    public class Container
    {
        public string Name;
        public ObservableCollection<CustomTreeViewItem> m_Tasks { get; set; }
        private mediametrieEntities bdd;
        private requete req;

        public Container()
        {
            bdd = new mediametrieEntities();
            req = new requete();
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
            taches t = req.getSTaches(bdd, item.Name);
            req.supTaches(bdd, t);
        }

        public void addItem(CustomTreeViewItem parent, string title, string beginDate, string endDate, string comment, bool done)
        {
            CustomTreeViewItem item = new CustomTreeViewItem(title);
            item.parent = parent;
            item.comment = comment;
            item.dateBegin = beginDate;
            item.dateEnd = endDate;
            item.chkBox.IsChecked = done;
            item.IsExpanded = true;
            if (parent != null)
            {
                //MessageBox.Show(parent.Name);
                item.chkBox.IsEnabled = !done;
                int index = (m_Tasks.IndexOf(parent) >= 0) ? m_Tasks.IndexOf(parent) : 0;
                m_Tasks[index].Items.Add(item);
            }
            else
            {
                item.chkBox.IsEnabled = true;
                m_Tasks.Add(item);
            }
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

        public int Contains(string name)
        {
            for(int i = 0; i < this.Items.Count; ++i)
            {
                CustomTreeViewItem item = this.Items[i] as CustomTreeViewItem;
                if (item.Title.Text == name)
                    return 0;
            }
            return -1;
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
                    {
                        Title.Text = textBox.Text;
                        chkBox.Content = Title;
                        ev.Handled = true;
                    }
                };
                textBox.LostFocus += (o, ev) =>
                {
                    Title.Text = textBox.Text;
                    chkBox.Content = Title;
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
            foreach(CustomTreeViewItem item in this.Items)
            {
                item.chkBox.IsChecked = true;
            }
        }

        public CustomTreeViewItem parent;
        public CheckBox chkBox = new CheckBox();
        public TextBlock Title = new TextBlock();
        public string comment;
        public string dateBegin;
        public string dateEnd;
    }
}