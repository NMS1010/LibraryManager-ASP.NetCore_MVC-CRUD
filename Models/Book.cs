using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookManagerMVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required, DisplayName("Tiêu đề")]
        public string Name { get; set; } = "A New Book";

        [Required, DisplayName("Tác giả")]
        public string Authors { get; set; } = "Authors";

        [Required, DisplayName("Nhà xuất bản")]
        public string Publisher { get; set; } = "Publisher";

        [Required, DisplayName("Năm xuất bản")]
        public int Year { get; set; } = DateTime.Now.Year;

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("File")]
        public string FileName { get; set; }
    }
}
