namespace UniTrack.Infrastructure.Services.Sheets;
public class GoogleSheetsConfig
{
    public string ServiceAccountJsonPath { get; set; }
    public const string SheetsScope = "https://www.googleapis.com/auth/spreadsheets";
    public const string DriveScope = "https://www.googleapis.com/auth/drive";
    public string RootFolderId { get; set; } 
}