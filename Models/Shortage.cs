using System;
using System.ComponentModel.DataAnnotations;

namespace VismaShortageManagement.Models
{
    public class Shortage
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public Room Room { get; set; }
        public Category Category { get; set; }
        [Range(1, 10)]
        public int Priority { get; set; }
        public DateTime CreatedOn { get; set; }

        public Shortage()
        {
            CreatedOn = DateTime.Now;
        }

        public Shortage(string title, string name, Room room, Category category, int priority)
        {
            Title = title;
            Name = name;
            Room = room;
            Category = category;
            Priority = priority;
            CreatedOn = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Title: {Title}, Name: {Name}, Room: {Room}, Category: {Category}, Priority: {Priority}, Created: {CreatedOn:yyyy-MM-dd HH:mm}";
        }
    }

    public enum Room
    {
        MeetingRoom = 1,
        Kitchen = 2,
        Bathroom = 3
    }

    public enum Category
    {
        Electronics = 1,
        Food = 2,
        Other = 3
    }
}