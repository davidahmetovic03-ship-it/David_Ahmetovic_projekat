using David_Ahmetovic_projekat.Models;

namespace David_Ahmetovic_projekat.Services
{
    public class PdfService
    {
        public static async Task GenerateAndShareSchedulePdfAsync(List<ScheduleItem> scheduleItems, string studentName)
        {
            // 1. Pravljenje HTML sadržaja koji izglada kao uredan dokument
            var htmlContent = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; padding: 20px; }}
                    h1 {{ color: #512BD4; }}
                    table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
                    th, td {{ border: 1px solid #ddd; padding: 10px; text-align: left; }}
                    th {{ background-color: #512BD4; color: white; }}
                    tr:nth-child(even) {{ background-color: #f2f2f2; }}
                </style>
            </head>
            <body>
                <h1>Raspored Časova</h1>
                <p><strong>Učenik/Student:</strong> {studentName}</p>
                <p><strong>Datum generisanja:</strong> {DateTime.Now:dd.MM.yyyy. HH:mm}</p>
                
                <table>
                    <tr>
                        <th>Dan</th>
                        <th>Predmet</th>
                        <th>Vrijeme</th>
                        <th>Učionica</th>
                    </tr>";

            foreach (var item in scheduleItems)
            {
                htmlContent += $@"
                    <tr>
                        <td>{item.DayOfWeek}</td>
                        <td>{item.SubjectName}</td>
                        <td>{item.StartTime}</td>
                        <td>{item.Classroom}</td>
                    </tr>";
            }

            htmlContent += @"
                </table>
            </body>
            </html>";

            // 2. Snimanje HTML fajla u privremenu lokalnu memoriju
            string fileName = $"Raspored_{DateTime.Now:yyyyMMdd_HHmmss}.html";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            await File.WriteAllTextAsync(filePath, htmlContent);

            // 3. Otvaranje dijaloga za dijeljenje/preuzimanje ili štampu
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Preuzmi ili Odštampaj Raspored",
                File = new ShareFile(filePath)
            });
        }
    }
}