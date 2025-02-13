using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class AddCowSeedDto
    {
        [Required]
        public double Weight { get; set; }

        [Required]
        public int TypeofCowsId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int SuppliersId { get; set; }

    }

}
