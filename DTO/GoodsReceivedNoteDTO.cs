﻿using AccountManagermnet.Domain;
using System.ComponentModel.DataAnnotations;

namespace AccountManagermnet.DTO
{
    public class GoodsReceivedNoteDTO
    {
        public string GRNId { get; set; } 
        public string DocumentDay { get; set; } 

        public string DocumentNumber { get; set; } 

        public string Detail { get; set; } 
        //liên kết 1 to 1 với bảng Person
        public string PersonID { get; set; }

        //liên kết 1 to many với bảng GoodsReceivedNoteDetail
        public List<string> GoodReceivedNoteDetails { get; set; }
    }
}