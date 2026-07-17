namespace OnlineGym.Application.Domain;

public class Rating
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long TrainerId { get; set; }
    public int RatingValue { get; set; }
    public string? Comment { get; set; }
    public DateTime RatingDate { get; set; }

    public Rating(long id, long clientId, long trainerId, int ratingValue, string? comment, DateTime ratingDate)
    {
        Id = id;
        ClientId = clientId;
        TrainerId = trainerId;
        RatingValue = ratingValue;
        Comment = comment;
        RatingDate = ratingDate;
    }
}