using System.ComponentModel;

namespace ZStore.Core
{
    public class Category
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
