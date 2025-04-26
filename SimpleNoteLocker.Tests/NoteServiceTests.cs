using SimpleNoteLocker.Core.Models;
using SimpleNoteLocker.Core.Service;

namespace SimpleNoteLocker.Tests
{
    [TestFixture]
    public class NoteServiceTests
    {
        private string _testDbPath;
        private NoteService _service;

        [SetUp]
        public void Setup()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SimpleNoteLocker");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _testDbPath = Path.Combine(folder, $"test_{Guid.NewGuid()}.db");

            // Redirect db path by subclassing or modifying the service constructor
            _service = new NoteService(_testDbPath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testDbPath))
                File.Delete(_testDbPath);
        }

        [Test]
        public void SaveNote_ShouldAddNoteToDatabase()
        {
            var note = new Note { Id = Guid.NewGuid(), Title = "Test", Content = "Hello" };
            _service.SaveNote(note);

            var notes = _service.GetAllNotes();

            Assert.AreEqual(1, notes.Count);
            Assert.AreEqual("Test", notes[0].Title);
        }

        [Test]
        public void DeleteNote_ShouldRemoveNoteFromDatabase()
        {
            var note = new Note { Id = Guid.NewGuid(), Title = "To Delete", Content = "Bye" };
            _service.SaveNote(note);

            _service.DeleteNote(note.Id);
            var notes = _service.GetAllNotes();

            Assert.AreEqual(0, notes.Count);
        }

        [Test]
        public void SaveNote_ShouldUpdateExistingNote()
        {
            var note = new Note { Id = Guid.NewGuid(), Title = "Original", Content = "Original content" };
            _service.SaveNote(note);

            note.Title = "Updated";
            note.Content = "Updated content";
            _service.SaveNote(note);

            var notes = _service.GetAllNotes();
            Assert.AreEqual(1, notes.Count);
            Assert.AreEqual("Updated", notes[0].Title);
            Assert.AreEqual("Updated content", notes[0].Content);
        }
    }
}