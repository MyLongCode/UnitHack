﻿using UnitApi.Models;

namespace UnitApi.dto.Review
{
    public class CreateReviewRequest
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}