using Library;

namespace Library
{
    [Table("users")]
    public class User
    {
        [PrimaryKey] 
        public int Id { get; set; }

        [Column] 
        public string Name { get; set; }
    }
    
}