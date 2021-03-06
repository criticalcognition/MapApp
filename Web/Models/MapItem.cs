﻿using System.Data.Entity.Spatial;

namespace MapApp.Web.Models
{
    public class MapItem
    {
        public int Id { get; set; }
        public string EntityType { get; set; }
        public DbGeography Geolocation { get; set; }
    }
}