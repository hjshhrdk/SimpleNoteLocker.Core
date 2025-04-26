using SimpleNoteLocker.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNoteLocker.Core.Service
{
    public class NoteService
    {
        private readonly string _dbPath;
        public NoteService()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNoteLocker");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _dbPath = Path.Combine(folder, "notes.db");

            InitializeDatabase();
        }

        public NoteService(string dbPath)
        {
            _dbPath = dbPath;
        }

        private SQLiteConnection GetConnection() =>
            new SQLiteConnection($"Data Source={_dbPath};Version=3;");

        private void InitializeDatabase()
        {
            var conn = GetConnection();
            conn.Open();

            var cmd = new SQLiteCommand(@"
            CREATE TABLE IF NOT EXISTS Notes (
                Id TEXT PRIMARY KEY,
                Title TEXT NOT NULL,
                Content TEXT,
                CreatedAt TEXT
            )", conn);

            cmd.ExecuteNonQuery();
        }

        public ObservableCollection<Note> GetAllNotes()
        {
            var notes = new ObservableCollection<Note>();
            var conn = GetConnection();
            conn.Open();

            var cmd = new SQLiteCommand("SELECT * FROM Notes ORDER BY CreatedAt DESC", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                notes.Add(new Note
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    Title = reader["Title"].ToString(),
                    Content = reader["Content"].ToString(),
                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                });
            }

            return notes;
        }

        public void SaveNote(Note note)
        {
            var conn = GetConnection();
            conn.Open();

            bool exists = NoteExists(note.Id, conn);

            var cmd = new SQLiteCommand(conn);

            if (exists)
            {
                cmd.CommandText = @"UPDATE Notes SET Title = @title, Content = @content WHERE Id = @id";
            }
            else
            {
                cmd.CommandText = @"INSERT INTO Notes (Id, Title, Content, CreatedAt) 
                                VALUES (@id, @title, @content, @createdAt)";
            }

            cmd.Parameters.AddWithValue("@id", note.Id.ToString());
            cmd.Parameters.AddWithValue("@title", note.Title);
            cmd.Parameters.AddWithValue("@content", note.Content);
            cmd.Parameters.AddWithValue("@createdAt", note.CreatedAt.ToString("o"));

            cmd.ExecuteNonQuery();
        }

        private bool NoteExists(Guid id, SQLiteConnection conn)
        {
            var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Notes WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id.ToString());
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public void DeleteNote(Guid id)
        {
            var conn = GetConnection();
            conn.Open();

            var cmd = new SQLiteCommand("DELETE FROM Notes WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id.ToString());
            cmd.ExecuteNonQuery();
        }
    }
}
