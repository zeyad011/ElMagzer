using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Shared.Dtos
{
    public class AddCowSeedRequestDto
    {
        [Required]
        public List<AddCowSeedDto> Cows { get; set; }
    }

}
