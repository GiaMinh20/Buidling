using API.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Members")]
    public class Member : Person
    {
        public int Id { get; set; }
        public UserAddress PlaceOfOrigin { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public virtual Account Account { get; set; }
        public int AccountId { get; set; }

    }


}
