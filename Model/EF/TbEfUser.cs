using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.EF
{   
    [Table( "tbefuser")]
    public class TbEfUser
    {   
        [Key]
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
             
    }
}
