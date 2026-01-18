using Microsoft.AspNetCore.Identity;

namespace Al_Rayyan.Models
{
        public class Recitation_logs
        {
            public int ID { get; set; }
            public int StudentId { get; set; }
            public int TeacherId { get; set; }
            public DateTime Date { get; set; }
            public string HifzContent { get; set; }
            public string HifzReminders { get; set; }
            public int HifzMistakes { get; set; }
            public string HifzMinorMistakes { get; set; }
            public int HifzFinalScore { get; set; }
            public string RevContent { get; set; }
            public string RevReminders { get; set; }
            public int RevMistakes { get; set; }
            public string RevMinorMistakes { get; set; }
            public int RevFinalScore { get; set; }
            Recitation_logs() { }
    }
}
