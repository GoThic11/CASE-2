using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.VisualBasic;
using System.Windows.Media;

namespace Case2
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private TimeSpan currentTaskTime = TimeSpan.Zero;
        private string currentTask = null;
        private bool isRunning = false;

        private Dictionary<string, List<string>> projects = new Dictionary<string, List<string>>();

        private Dictionary<string, TimeSpan> taskDurations = new Dictionary<string, TimeSpan>();

        private HashSet<string> completedTasks = new HashSet<string>();

        private int completedProjects = 0;
        private int totalCompletedTasks = 0;
        private TimeSpan totalTime = TimeSpan.Zero;

        public MainWindow()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            projects = new Dictionary<string, List<string>>();
            UpdateProjectList();
            timerTextBlock.Text = "00:00:00";
        }

        // === TIMER ===
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentTask != null && !completedTasks.Contains(currentTask))
            {
                currentTaskTime = currentTaskTime.Add(TimeSpan.FromSeconds(1));
                timerTextBlock.Text = currentTaskTime.ToString(@"hh\:mm\:ss");
                taskDurations[currentTask] = currentTaskTime;
            }
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is ListBoxItem selectedItem &&
                selectedItem.Content is TextBlock textBlock)
            {
                currentTask = textBlock.Text;

                if (!taskDurations.ContainsKey(currentTask))
                {
                    taskDurations[currentTask] = TimeSpan.Zero;
                }

                currentTaskTime = taskDurations[currentTask];
                timer.Start();
                isRunning = true;
            }
            else
            {
                MessageBox.Show("Выберите задачу для запуска таймера.");
            }
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning && currentTask != null)
            {
                timer.Stop();
                taskDurations[currentTask] = currentTaskTime;
                isRunning = false;

                MessageBox.Show($"Таймер остановлен. Время сохранено для задачи '{currentTask}'.");
            }
        }

        // === PROJECTS ===
        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            string name = ProjectNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(name) && !projects.ContainsKey(name))
            {
                projects[name] = new List<string>();
                UpdateProjectList();
                ProjectNameTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Введите уникальное имя проекта.");
            }
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject)
            {
                string newName = Interaction.InputBox("Введите новое имя проекта:", "Редактирование", selectedProject);
                if (!string.IsNullOrEmpty(newName) && newName != selectedProject && !projects.ContainsKey(newName))
                {
                    var tasks = projects[selectedProject];
                    projects.Remove(selectedProject);
                    projects[newName] = tasks;
                    UpdateProjectList();
                    UpdateTaskList(newName);
                }
            }
            else
            {
                MessageBox.Show("Выберите проект для редактирования.");
            }
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject)
            {
                projects.Remove(selectedProject);
                UpdateProjectList();
                TaskListBox.Items.Clear();
                currentTask = null;
            }
            else
            {
                MessageBox.Show("Выберите проект для удаления.");
            }
        }

        private void ExecuteProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject && projects.ContainsKey(selectedProject))
            {
                foreach (var task in projects[selectedProject])
                {
                    if (!completedTasks.Contains(task))
                    {
                        completedTasks.Add(task);
                    }
                    if (!taskDurations.ContainsKey(task))
                    {
                        taskDurations[task] = TimeSpan.Zero;
                    }
                }

                completedProjects++;
                projects.Remove(selectedProject);
                UpdateProjectList();
                UpdateTaskList(null);

                MessageBox.Show($"Проект '{selectedProject}' выполнен. Все задачи отмечены как выполненные.");
            }
            else
            {
                MessageBox.Show("Выберите проект для выполнения.");
            }
        }

        // === TASKS ===
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string project)
            {
                string task = TaskNameTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(task) && !projects[project].Contains(task))
                {
                    projects[project].Add(task);
                    taskDurations[task] = TimeSpan.Zero;
                    UpdateTaskList(project);
                    TaskNameTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Введите уникальное имя задачи.");
                }
            }
            else
            {
                MessageBox.Show("Выберите проект перед добавлением задачи.");
            }
        }

        private void ExecuteTask_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject &&
                TaskListBox.SelectedItem is ListBoxItem selectedItem &&
                selectedItem.Content is TextBlock textBlock)
            {
                string selectedTask = textBlock.Text;

                if (!completedTasks.Contains(selectedTask))
                {
                    completedTasks.Add(selectedTask);
                    totalCompletedTasks++;
                }

                taskDurations[selectedTask] = currentTaskTime;
                totalTime += currentTaskTime;

                UpdateTaskList(selectedProject);

                currentTaskTime = TimeSpan.Zero;
                timerTextBlock.Text = "00:00:00";

                MessageBox.Show($"Задача '{selectedTask}' выполнена.");
            }
            else
            {
                MessageBox.Show("Выберите проект и задачу для выполнения.");
            }
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject &&
                TaskListBox.SelectedItem is ListBoxItem selectedItem &&
                selectedItem.Content is TextBlock textBlock)
            {
                string oldTask = textBlock.Text;
                string newName = Interaction.InputBox("Введите новое имя задачи:", "Редактирование", oldTask);

                if (!string.IsNullOrEmpty(newName) && newName != oldTask && !projects[selectedProject].Contains(newName))
                {
                    int index = projects[selectedProject].IndexOf(oldTask);
                    projects[selectedProject][index] = newName;

                    if (taskDurations.ContainsKey(oldTask))
                    {
                        taskDurations[newName] = taskDurations[oldTask];
                        taskDurations.Remove(oldTask);
                    }

                    if (completedTasks.Contains(oldTask))
                    {
                        completedTasks.Remove(oldTask);
                        completedTasks.Add(newName);
                    }

                    UpdateTaskList(selectedProject);
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу и проект для редактирования.");
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject &&
                TaskListBox.SelectedItem is ListBoxItem selectedItem &&
                selectedItem.Content is TextBlock textBlock)
            {
                string task = textBlock.Text;
                projects[selectedProject].Remove(task);
                UpdateTaskList(selectedProject);
                currentTask = null;
            }
            else
            {
                MessageBox.Show("Выберите задачу для удаления.");
            }
        }

        // === LISTBOX HANDLERS ===
        private void ProjectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string project)
            {
                UpdateTaskList(project);
            }
        }

        private void TaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskListBox.SelectedItem is ListBoxItem selectedItem &&
                selectedItem.Content is TextBlock textBlock)
            {
                string selectedTask = textBlock.Text;
                currentTask = selectedTask;

                if (taskDurations.ContainsKey(selectedTask))
                {
                    currentTaskTime = taskDurations[selectedTask];
                    timerTextBlock.Text = currentTaskTime.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    currentTaskTime = TimeSpan.Zero;
                    timerTextBlock.Text = "00:00:00";
                    taskDurations[selectedTask] = TimeSpan.Zero;
                }
            }
            else
            {
                currentTask = null;
                currentTaskTime = TimeSpan.Zero;
                timerTextBlock.Text = "00:00:00";
            }
        }

        // === UTILITIES ===
        private void UpdateProjectList()
        {
            ProjectListBox.Items.Clear();
            foreach (var project in projects.Keys)
            {
                ProjectListBox.Items.Add(project);
            }
        }

        private void UpdateTaskList(string projectName)
        {
            TaskListBox.Items.Clear();

            if (projectName != null && projects.ContainsKey(projectName))
            {
                foreach (var task in projects[projectName])
                {
                    var textBlock = new TextBlock { Text = task };
                    if (completedTasks.Contains(task))
                    {
                        textBlock.Foreground = Brushes.Green;
                    }

                    var item = new ListBoxItem();
                    item.Content = textBlock;
                    TaskListBox.Items.Add(item);
                }
            }
            else
            {
                TaskListBox.Items.Add("Выберите проект");
            }
        }

        // === EXPORT TO TXT ===
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is string selectedProject && projects.ContainsKey(selectedProject))
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string filePath = System.IO.Path.Combine(desktopPath, $"{selectedProject}_Отчет.txt");

                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    writer.WriteLine($"Проект: {selectedProject}\n");

                    foreach (var task in projects[selectedProject])
                    {
                        TimeSpan time = taskDurations.ContainsKey(task) ? taskDurations[task] : TimeSpan.Zero;
                        writer.WriteLine($" • Задача: {task}, Время: {time:hh\\:mm\\:ss}");
                    }
                }

                MessageBox.Show($"Файл успешно сохранён:\n{filePath}", "Экспорт завершён");
            }
            else
            {
                MessageBox.Show("Выберите проект для экспорта.");
            }
        }

        private void EndBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // === TEXTBOX HANDLERS ===
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == "Название проекта" || textBox.Text == "Название задачи")
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = (textBox.Name == "ProjectNameTextBox") ? "Название проекта" : "Название задачи";
                textBox.Foreground = Brushes.Gray;
            }
        }
    }
}