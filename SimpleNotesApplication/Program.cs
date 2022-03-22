using System;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

namespace SimpleNotesApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            

            ReadCommand();
            Console.ReadLine();
        }

        private static string NoteDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Notes\";

        private static void ReadCommand()
        {
            // Read user input
            Console.Write(Directory.GetDirectoryRoot(NoteDirectory));
            string Command = Console.ReadLine();

            // Execute the correct method
            switch (Command.ToLower()) {
                case "new":
                    NewNote();
                    Main(null);
                    break;
                case "edit":
                    EditNote();
                    Main(null);
                    break;
                case "read":
                    ReadNote();
                    Main(null);
                    break;
                case "delete":
                    DeleteNote();
                    Main(null);
                    break;
                case "shownotes":
                    ShowNotes();
                    Main(null);
                    break;
                case "dir":
                    NotesDirectory();
                    Main(null);
                    break;
                case "cls":
                    Console.Clear();
                    Main(null);
                    break;
                case "exit":
                    Exit();
                    break;
                default:
                    CommandsAvailable();
                    Main(null);
                    break;
            }
        }

        private static void NewNote()
        {
            // Read file name
            Console.WriteLine("Please Enter The File Name:\n");
            string FileName = Console.ReadLine() + ".xml";

            // Read note input
            Console.WriteLine("\nPlease Enter Notes:");
            Console.WriteLine("  --To finish writing, enter '**save'--\n");
            int i = 0;
            List<string> Inputs = new List<string>();
            string Note;
            do {
                Note = Console.ReadLine();
                Inputs.Add(Note);
            } while (Note != "**save");

            Inputs.Remove("**save");

            // Add XML settings
            XmlWriterSettings NoteSettings = new XmlWriterSettings();

            NoteSettings.CheckCharacters = false;
            NoteSettings.ConformanceLevel = ConformanceLevel.Auto;
            NoteSettings.Indent = true;

            // Write the file
            using (XmlWriter NewNote = XmlWriter.Create(NoteDirectory + FileName, NoteSettings)) {
                NewNote.WriteStartDocument();
                NewNote.WriteStartElement("Note");
                foreach (var item in Inputs) {
                    NewNote.WriteElementString("body", item);
                }
                NewNote.WriteEndElement();

                NewNote.Flush();
                NewNote.Close();
            }
        }

        private static void EditNote()
        {
            // Read user input
            Console.WriteLine("Please enter file name.\n");
            string FileName = Console.ReadLine().ToLower();
            
            if (File.Exists(NoteDirectory + FileName)) {
                XmlDocument doc = new XmlDocument();

                // Load the document
                try {
                    doc.Load(NoteDirectory + FileName);
                    Console.Write(doc.SelectSingleNode("//body").InnerText);
                    string ReadInput = Console.ReadLine();

                    if(ReadInput.ToLower() == "cancel") {
                        Main(null);
                    } else {
                        string newText = doc.SelectSingleNode("//body").InnerText = ReadInput;

                        doc.Save(NoteDirectory + FileName);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Could not edit note follosing error occurred: " + ex.Message);
                }
            } else {
                Console.WriteLine("File not found\n");
            }
        }

        private static void ReadNote()
        {
            Console.WriteLine("Please enter file name.\n");
            string FileName = Console.ReadLine().ToLower();

            if (File.Exists(NoteDirectory + FileName)) {
                XmlDocument Doc = new XmlDocument();
                Doc.Load(NoteDirectory + FileName);

                Console.WriteLine(Doc.SelectSingleNode("//body").InnerText);
            } else {
                Console.WriteLine("File not found");
            }
        }

        private static void DeleteNote()
        {
            // Read user input
            Console.WriteLine("Please enter file name\n");
            string FileName = Console.ReadLine();

            if (File.Exists(NoteDirectory + FileName)) {
                // Confirm deletion
                Console.WriteLine(Environment.NewLine + "Are you sure you wish to delete this file? Y/N\n");
                string Confirmation = Console.ReadLine().ToLower();

                // Delete file or return to main
                if (Confirmation == "y") {
                    try {
                        File.Delete(NoteDirectory + FileName);

                        Console.WriteLine("File has been deleted\n");
                    }
                    catch(Exception ex) {
                        Console.WriteLine("File not deleted following erro occured: " + ex.Message);
                    }
                } else if (Confirmation == "n") {
                    Main(null);
                } else {
                    Console.WriteLine("Invalid command\n");
                    DeleteNote();
                }
            } else {
                Console.WriteLine("File does not exist\n");
                DeleteNote();
            }
        }

        private static void ShowNotes()
        {
            // Get directory
            string NoteLocation = NoteDirectory;

            DirectoryInfo Dir = new DirectoryInfo(NoteLocation);

            if (Directory.Exists(NoteLocation)) {
                // Get all cml files
                FileInfo[] NoteFiles = Dir.GetFiles("*.xml");

                if(NoteFiles.Length != 0) {
                    Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop + 2);
                    Console.WriteLine("+-----------------+");

                    // Show the file names
                    foreach (var item in NoteFiles) {
                        Console.WriteLine("  " + item.Name);
                    }

                    Console.WriteLine(Environment.NewLine);
                } else {
                    Console.WriteLine("No notes found.\n");
                }
            } else {
                // Create directory if not existing
                Console.WriteLine(" Directory does not exist....creating directory\n");

                Directory.CreateDirectory(NoteLocation);

                Console.WriteLine(" Directory: " + NoteLocation + " created successfully.\n");
            }
        }

        private static void CommandsAvailable()
        {
            // Show the commands available
            Console.WriteLine(" New - Create a new note\n Edit - Edit a note\n Read -  Read a note\n ShowNotes - List all notes\n Exit - Exit the application\n Dir - Opens note directory\n Help - Shows this help message\n");
        }

        private static void Exit()
        {
            // Close application
            Environment.Exit(0);
        }

        private static void NotesDirectory()
        {
            Process.Start("explorer.exe", NoteDirectory);
        }

    }
}
