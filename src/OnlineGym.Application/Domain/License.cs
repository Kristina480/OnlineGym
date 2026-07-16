namespace OnlineGym.Application.Domain;

public class License
{
    public long LicenseId { get; set; }
    public long TrainerId { get; set; }
    public string Name { get; set; }
    public string DocumentType { get; set; }
    public DateTime IssueDate { get; set; }

    public License(long licenseId, long trainerId, string name, string documentType, DateTime issueDate)
    {
        LicenseId = licenseId;
        TrainerId = trainerId;
        Name = name;
        DocumentType = documentType;
        IssueDate = issueDate;
    }
}