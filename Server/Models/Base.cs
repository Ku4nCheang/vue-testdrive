
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Triggers;

namespace netcore.Models
{
    public abstract class Trackable
    {
        public virtual DateTime Inserted { get; set; }

        public virtual DateTime LastUpdated { get; set; }

        static Trackable() {
            Triggers<Trackable>.Inserting += entry => entry.Entity.Inserted = entry.Entity.LastUpdated = DateTime.UtcNow;
            Triggers<Trackable>.Updating += entry => entry.Entity.LastUpdated = DateTime.UtcNow;
        }
    }
}