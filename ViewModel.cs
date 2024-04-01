using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Caro
{
    public partial class Test : ObservableObject
    {
        [ObservableProperty]
        private string _text = "Test";
    }

    public partial class ViewModel : ObservableObject
    {

        public ObservableCollection<Test> List2 { get; set; } = [
            new Test(),
            new Test()
        ];

        public ObservableCollection<Cell> List { get; set; } = [
            new Cell(Position.Top | Position.Left),
            new Cell(Position.Top | Position.Left),

        ];
    }
}
