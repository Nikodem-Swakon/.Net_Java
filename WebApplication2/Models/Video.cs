using System;
using System.Collections.Generic;
using WebApplication2.Models; // żeby mógł widzieć klasę Profile
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    /// <summary>
    /// Model reprezentujący film na YouTube.
    /// Zawiera właściwości takie jak Id, VideoId, Tytuł, Id kanału, Data publikacji i czy jest ulubiony.
    /// </summary>
    public class Video
    {
    public int Id { get; set; }
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public bool IsFavorite { get; set; } = false;

    // Foreign key to Profile
    public int ProfileId { get; set; }
    public Profile? Profile { get; set; }
    }
}