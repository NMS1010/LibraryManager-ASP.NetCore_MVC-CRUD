using BookManagerMVC.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Xml.Serialization;

namespace BookManagerMVC.Models
{
    public class Service
    {
        private readonly string dataPath = "Data\\data.xml";
        private readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(HashSet<Book>));
        public HashSet<Book> Books { get; set; }

        public Service()
        {
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }
            if (File.Exists(dataPath))
            {
                using (FileStream fs = new FileStream(dataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    Books = xmlSerializer.Deserialize(fs) as HashSet<Book>;
                }
            }
            else
            {
                Books = new HashSet<Book>()
                {
                    new Book {Id = 1, Name = "ASP.NET Core for dummy",Publisher = "Apress", Year = 2018, Authors = "Donald Trump"},
                    new Book {Id = 2,  Name = "Professional ASP.NET Core 3",Publisher = "Manning", Year = 2019, Authors = "Bill Clinton"},
                    new Book {Id = 3,  Name = "ASP.NET Core Self learning",Publisher = "Wiley", Year = 2017,Authors = "Barack Obama"},
                    new Book {Id = 4,  Name = "ASP.NET Core quick course",Publisher = "Linda",Authors = "George Bush"},
                    new Book {Id = 5,  Name = "ASP.NET Core Video Course",Publisher = "Linda", Authors = "Vladimir Putin"},
                };
            }
        }

        public Book GetBook(int id) => Books.FirstOrDefault(b => b.Id == id);

        public Book CreateBook()
        {
            int currID = Books.Max(b => b.Id);
            return new Book() { Id = currID + 1 };
        }
        public bool AddBook(Book b) => Books.Add(b);

        public bool DeleteBook(int id)
        {
            Book book = GetBook(id);
            return book != null && Books.Remove(book);
        }

        public bool UpdateBook(Book b)
        {
            Book book = GetBook(b.Id);
            return book != null && Books.Remove(book) && Books.Add(b);
        }

        public void SaveChange()
        {
            using (FileStream fs = new FileStream(dataPath,FileMode.Create ,FileAccess.ReadWrite))
            {
                xmlSerializer.Serialize(fs, Books);
            }
        }
        public string GetPathFile(string fileName)
        {
            return $"Data\\{fileName}";
        }
        public void Upload(Book b, IFormFile file)
        {
            if (file == null) return;
            string path = GetPathFile(file.FileName);
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                file.CopyTo(fs);
            }
            b.FileName = file.FileName;
        }
        public (Stream, string) Download(Book b)
        {
            MemoryStream ms = new MemoryStream();
            using (FileStream fs = new FileStream(b.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fs.CopyTo(ms);
                ms.Position = 0;
                var type = Path.GetExtension(b.FileName) switch
                {
                    "pdf" => "application/pdf",
                    "docx" => "application/vnd.ms-word",
                    "doc" => "application/vnd.ms-word",
                    "txt" => "text/plain",
                    _ => "application/pdf"
                };

                return (ms, type);
            }
        }

        public HashSet<Book> Search(string strSearch)
        {
            if (string.IsNullOrEmpty(strSearch))
                return new HashSet<Book>();
            strSearch = strSearch.ToLower();
            return new HashSet<Book>(Books.Where(book => 
            book.Name.ToLower().Contains(strSearch) ||
            book.Publisher.ToLower().Contains(strSearch) ||
            book.Authors.ToLower().Contains(strSearch) ||
            (book.Description != null && book.Description.ToLower().Contains(strSearch)) ||
            book.Year == int.Parse(strSearch)));
        }

        public (HashSet<Book> books, int totalPages, int currPage) Pagination(string orderBy, bool asc, int currPage)
        {
            int size = (int)ITEM_PER_PAGE.NUMBER_ITEM;
            int totalPages = (int)Math.Ceiling((double)Books.Count() / size);
            return (new HashSet<Book>(Books.Skip((currPage - 1) * size).Take(size).AsQueryable().OrderBy($"{orderBy} {(asc ? "" : "descending")}")), totalPages, currPage);
        }
    }
}
