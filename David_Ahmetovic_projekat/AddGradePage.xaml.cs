using David_Ahmetovic_projekat.Models;
using David_Ahmetovic_projekat.Services;

namespace David_Ahmetovic_projekat;

public partial class AddGradePage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public AddGradePage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSubjects();
    }

    private async Task LoadSubjects()
    {
        var subjects = await _databaseService.GetSubjectsAsync();
        SubjectPicker.ItemsSource = subjects;
    }

    private async void OnSubjectSelected(object sender, EventArgs e)
    {
        await RefreshGradesAndAverage();
    }

    private async Task RefreshGradesAndAverage()
    {
        var selectedSubject = SubjectPicker.SelectedItem as Subject;
        if (selectedSubject == null) return;

        var grades = await _databaseService.GetGradesForSubjectAsync(selectedSubject.Id);
        GradesCollectionView.ItemsSource = grades;

        if (grades.Any())
        {
            double average = grades.Average(g => g.Value);
            AverageLabel.Text = $"Prosjek ocjena: {average:F2}";
        }
        else
        {
            AverageLabel.Text = "Prosjek ocjena: Nema unesenih ocjena";
        }
    }

    private async void OnSaveGradeClicked(object sender, EventArgs e)
    {
        var selectedSubject = SubjectPicker.SelectedItem as Subject;
        if (selectedSubject == null)
        {
            await DisplayAlert("Greška", "Molimo izaberite predmet.", "OK");
            return;
        }

        if (!int.TryParse(GradeValueEntry.Text, out int gradeValue) || gradeValue < 1 || gradeValue > 5)
        {
            await DisplayAlert("Greška", "Unesite validnu ocjenu od 1 do 5.", "OK");
            return;
        }

        var newGrade = new Grade
        {
            SubjectId = selectedSubject.Id,
            Value = gradeValue,
            Description = DescriptionEntry.Text ?? "Ocjena",
            Date = DateTime.Now
        };

        await _databaseService.SaveGradeAsync(newGrade);

        GradeValueEntry.Text = string.Empty;
        DescriptionEntry.Text = string.Empty;

        await RefreshGradesAndAverage();
        await DisplayAlert("Uspjeh", "Ocjena je sačuvana!", "OK");
    }
}