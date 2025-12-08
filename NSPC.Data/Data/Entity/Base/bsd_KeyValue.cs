using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NSPC.Data
{
    [Table("bsd_Key_Value")]
    public class bsd_KeyValue
    {
        [Key]
        [StringLength(128)]
        public string Key { get; set; }

        public string Value { get; set; }

    }
}
