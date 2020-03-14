using System;
using System.Reactive;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class OperationsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> EditCommand { get; set; }

        public ReactiveCommand<Unit, Unit> CopyCommand { get; set; }

        public ReactiveCommand<Unit, Unit> MoveCommand { get;  set;}

        public ReactiveCommand<Unit, Unit> NewFolderCommand { get; set; }

        public ReactiveCommand<Unit, Unit> RemoveCommand { get; set; }

        public OperationsViewModel()
        {
            EditCommand = ReactiveCommand.Create(Edit);
            CopyCommand = ReactiveCommand.Create(Copy);
            MoveCommand = ReactiveCommand.Create(Move);
            NewFolderCommand = ReactiveCommand.Create(CreateNewFolder);
            RemoveCommand = ReactiveCommand.Create(Remove);
        }

        private void Edit()
        {
            throw new System.NotImplementedException();
        }

        private void Copy()
        {
            Console.WriteLine("COpy");
        }

        private void Move()
        {
            throw new System.NotImplementedException();
        }

        private void CreateNewFolder()
        {
            throw new System.NotImplementedException();
        }

        private void Remove()
        {
            throw new System.NotImplementedException();
        }
    }
}