﻿using System;

namespace Warehouse.Model.Models
{
    public class Movements
    {
        public int ID { get; set; }
        public Product Product { get; set; }
        public State State { get; set; }
        public DateTime DateStamp { get; set; }
    }
}
