using ElMagzer.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElMagzer.Service
{
    public class CowPiecesCleanupService
    {
        private readonly ElMagzerContext _context;

        public CowPiecesCleanupService(ElMagzerContext context)
        {
            _context = context;
        }
        public async Task CleanupCowPiecesAsync()
        {
            var cowPiecesToDelete = _context.CowsPieces
                .Where(cp => cp.Status == "Out" || cp.Status == "Cutting");
            _context.CowsPieces.RemoveRange(cowPiecesToDelete);


            var cowPieces2ToDelete = _context.Cow_Pieces_2
                .Where(cp => cp.status == "Out");
            _context.Cow_Pieces_2.RemoveRange(cowPieces2ToDelete);

            await _context.SaveChangesAsync();
        }
    }
}
