using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToDoApplication.Models
{
    public class TodoItem : INotifyPropertyChanged
    {
        private int _item_id;
        private string _item_name = string.Empty;
        private string _item_description = string.Empty;
        private string _status = "Pending";
        private int _user_id;

        public int item_id
        {
            get => _item_id;
            set
            {
                _item_id = value;
                OnPropertyChanged();
            }
        }

        public string item_name
        {
            get => _item_name;
            set
            {
                _item_name = value;
                OnPropertyChanged();
            }
        }

        public string item_description
        {
            get => _item_description;
            set
            {
                _item_description = value;
                OnPropertyChanged();
            }
        }

        public string status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public int user_id
        {
            get => _user_id;
            set
            {
                _user_id = value;
                OnPropertyChanged();
            }
        }

        public bool IsCompleted
        {
            get => status == "Completed";
            set
            {
                status = value ? "Completed" : "Pending";
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
