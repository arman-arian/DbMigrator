using System.ComponentModel.DataAnnotations;

namespace DbMigrator.Test.Data
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string FullName { get; set; }
    }
}
