namespace HalaqatBackend.Models
{
        public class RecitationLogs
        {
            public string Id { get; set; }
            public string StudentId { get; set; }
            public string TeacherId { get; set; }
            public string GroupId { get; set; }
            public DateTime Date { get; set; } // The date of the recitation session
            public bool Completed { get; set; } = false;


            public string HifzContent { get; set; }
            public float HifzPageCount { get; set; } = 0;
            public int HifzReminders { get; set; } = 0;
            public int HifzMistakes { get; set; } = 0;
            public int HifzMinorMistakes { get; set; } = 0;
            public int HifzFinalScore { get; set; } = 0;


            public string RevContent { get; set; }
            public float RevPageCount { get; set; } = 0;
            public int RevReminders { get; set; } = 0;
            public int RevMistakes { get; set; } = 0;
            public int RevMinorMistakes { get; set; } = 0;
            public int RevFinalScore { get; set; } = 0;

            public int ExstraPoint { get; set; }
            public string Notes { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
