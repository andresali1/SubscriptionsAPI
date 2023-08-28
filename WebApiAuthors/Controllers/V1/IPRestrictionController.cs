using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Migrations;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/iprestriction")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class IPRestrictionController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public IPRestrictionController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to create a new IP Restriction in DB
        /// </summary>
        /// <param name="ipRestrictionCreationDTO">Object with creation Data</param>
        /// <returns></returns>
        [HttpPost(Name = "createIpRestriction")]
        public async Task<ActionResult> Post(IPRestrictionCreationDTO ipRestrictionCreationDTO)
        {
            var keyDB = await _context.APIKey.FirstOrDefaultAsync(k => k.Id == ipRestrictionCreationDTO.KeyId);

            if (keyDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();

            if (keyDB.UserId != userId)
            {
                return Forbid();
            }

            var ipRestriction = new IPRestriction()
            {
                KeyId = keyDB.Id,
                IP = ipRestrictionCreationDTO.IP
            };

            _context.Add(ipRestriction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Method to Put a IPRestriction to DB
        /// </summary>
        /// <param name="id">Id of the restriction</param>
        /// <param name="ipRestrictionUpdateDTO">Object with the data to update</param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "putIpRestriction")]
        public async Task<ActionResult> Put(int id, IPRestrictionUpdateDTO ipRestrictionUpdateDTO)
        {
            var restrictionDB = await _context.IPRestriction
                .Include(k => k.Key)
                .FirstOrDefaultAsync(k => k.Id == id);

            if(restrictionDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();

            if (restrictionDB.Key.UserId != userId)
            {
                return Forbid();
            }

            restrictionDB.IP = ipRestrictionUpdateDTO.IP;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Method to delete a IPRestriction from DB
        /// </summary>
        /// <param name="id">Id of the restriction</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteIpRestriction")]
        public async Task<ActionResult> Delete(int id)
        {
            var restrictionDb = await _context.IPRestriction
                .Include(r => r.Key)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (restrictionDb == null)
            {
                return NotFound();
            }

            var userId = GetUserId();

            if (restrictionDb.Key.UserId != userId)
            {
                return Forbid();
            }

            _context.Remove(restrictionDb);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
