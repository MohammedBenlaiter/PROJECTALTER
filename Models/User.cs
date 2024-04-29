using System;
using System.Collections.Generic;

namespace PROJECTALTERAPI.Models;

public partial class User
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Picture { get; set; }

    public string Username { get; set; } = null!;

    public List<string>? VerifiedUser { get; set; }

    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    public virtual ICollection<Email> Emails { get; set; } = new List<Email>();

    public virtual ICollection<Exchange> ExchangeRecivers { get; set; } = new List<Exchange>();

    public virtual ICollection<Exchange> ExchangeSenders { get; set; } = new List<Exchange>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual ICollection<RatingStar> RatingStars { get; set; } = new List<RatingStar>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
