using SimpleNoteLocker.App.Common;
using SimpleNoteLocker.Core.Models;
using SimpleNoteLocker.Core.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleNoteLocker.App.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly NoteService _noteService = new();

        public ObservableCollection<Note> Notes { get; set; }

        private Note _selectedNote;
        public Note SelectedNote
        {
            get => _selectedNote;
            set
            {
                _selectedNote = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NoteTitle = value.Title;
                    NoteContent = value.Content;
                }
            }
        }

        private string _noteTitle;
        public string NoteTitle
        {
            get => _noteTitle;
            set { _noteTitle = value; OnPropertyChanged(); }
        }

        private string _noteContent;
        public string NoteContent
        {
            get => _noteContent;
            set { _noteContent = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand => new RelayCommand(SaveNote);
        public ICommand DeleteCommand => new RelayCommand(DeleteNote);
        public ICommand ClearCommand => new RelayCommand(ClearForm);

        public MainViewModel()
        {
            Notes = _noteService.GetAllNotes();
        }

        private void SaveNote()
        {
            var note = SelectedNote ?? new Note();
            note.Title = NoteTitle;
            note.Content = NoteContent;
            _noteService.SaveNote(note);
            if (!Notes.Contains(note)) Notes.Add(note);
            ClearForm();
        }

        private void DeleteNote()
        {
            if (SelectedNote == null) return;
            _noteService.DeleteNote(SelectedNote.Id);
            Notes.Remove(SelectedNote);
            ClearForm();
        }

        private void ClearForm()
        {
            SelectedNote = null;
            NoteTitle = "";
            NoteContent = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
