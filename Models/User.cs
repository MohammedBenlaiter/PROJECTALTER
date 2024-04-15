using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PROJECTALTERAPI.Models;

public partial class User
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[] Picture { get; set; } = null!;

    public string Username { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    [JsonIgnore]
    public virtual ICollection<Exchange> ExchangeRecivers { get; set; } = new List<Exchange>();
    [JsonIgnore]
    public virtual ICollection<Exchange> ExchangeSenders { get; set; } = new List<Exchange>();
    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    [JsonIgnore]
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
    [JsonIgnore]
    public virtual ICollection<RatingStar> RatingStars { get; set; } = new List<RatingStar>();
    [JsonIgnore]
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    [JsonIgnore]
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    [JsonIgnore]
    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
