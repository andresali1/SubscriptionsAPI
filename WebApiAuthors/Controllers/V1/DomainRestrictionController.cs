using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/domainrestriction")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DomainRestrictionController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;

        public DomainRestrictionController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to create a new Domain Restriction in DB
        /// </summary>
        /// <param name="domainRestrictionCreateDTO">Object with data to save</param>
        /// <returns></returns>
        [HttpPost(Name = "createDomainRestriction")]
        public async Task<ActionResult> Post(DomainRestrictionCreateDTO domainRestrictionCreateDTO)
        {
            var keyDB = await _context.APIKey.FirstOrDefaultAsync(k => k.Id == domainRestrictionCreateDTO.KeyId);

            if(keyDB == null)
            {
                return NotFound();
            }

            var userId = GetUserId();

            if(keyDB.UserId != userId)
            {
                return Forbid();
            }

            var domainRestriction = new DomainRestriction()
            {
                KeyId = domainRestrictionCreateDTO.KeyId,
                Domain = domainRestrictionCreateDTO.Domain
            };

            _context.Add(domainRestriction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Method to Put a domain restriction to DB
        /// </summary>
        /// <param name="id">Id of the restriction</param>
        /// <param name="domainRestrictionUpdateDTO">Object with data to Update</param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "putDomainRestriction")]
        public async Task<ActionResult> Put(int id, DomainRestrictionUpdateDTO domainRestrictionUpdateDTO)
        {
            var restrictionDb = await _context.DomainRestriction
                .Include(r => r.Key)
                .FirstOrDefaultAsync(r => r.Id == id);

            if(restrictionDb == null)
            {
                return NotFound();
            }

            var userId = GetUserId();

            if (restrictionDb.Key.UserId != userId)
            {
                return Forbid();
            }

            restrictionDb.Domain = domainRestrictionUpdateDTO.Domain;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Method to delete a Domain Restriction From DB
        /// </summary>
        /// <param name="id">Id of the restriction</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "deleteDomainRestriction")]
        public async Task<ActionResult> Delete(int id)
        {
            var restrictionDb = await _context.DomainRestriction
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
