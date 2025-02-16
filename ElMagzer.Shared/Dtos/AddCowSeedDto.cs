using System.ComponentModel.DataAnnotations;


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
