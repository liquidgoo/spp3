using AssemblyBrowserProject;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AssemblyBrowserWPF
{
    public class CommandImp : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public CommandImp(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }

    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public MainWindow hWnd;
        private TreeComponent currentTree;
        public TreeComponent CurrentTree
        {
            get { return currentTree; }
            set
            {
                currentTree = value;
                OnPropertyChanged("CurrentTree");
            }
        }

        private CommandImp openFileCommand;
        public CommandImp OpenFileCommand
        {
            get
            {
                return openFileCommand ??
                  (openFileCommand = new CommandImp(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog();
                      openFileDialog.Filter = "Dynamic Libraries (*.dll)|*.dll";
                      openFileDialog.InitialDirectory = "D:\\БГУИР\\Третий курс\\newRepos\\AssemblyBrowserProject\\TestAssembly\\bin\\Debug\\";
                      if (openFileDialog.ShowDialog() == true)
                      {
                          AssemblyTreeBuilder asm = 
                                new AssemblyTreeBuilder(openFileDialog.FileName);
                          asm.Build();
                          CurrentTree = asm.GetRoot();
                          hWnd.GetTreeFilled(CurrentTree);
                      }
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }


    public partial class App : Application { }
}
