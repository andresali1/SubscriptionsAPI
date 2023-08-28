using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;
using WebApiAuthors.Services;
using WebApiAuthors.Utilities;

namespace WebApiAuthors.Controllers.V1
{
    [ApiController]
    [Route("api/apikey")]
    [IsPresentHeader("x-version", "1")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class APIKeyController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly KeyService _keyService;

        public APIKeyController(ApplicationDbContext context,
            IMapper mapper, KeyService keyService)
        {
            _context = context;
            _mapper = mapper;
            _keyService = keyService;
        }

        /// <summary>
        /// Method to get the user's api keys
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "getKeys")]
        public async Task<List<KeyDTO>> MyKeys()
        {
            var userId = GetUserId();
            var keys = await _context.APIKey
                .Include(x => x.DomainRestrictions)
                .Include(x => x.IPRestrictions)
                .Where(x => x.UserId == userId).ToListAsync();
            return _mapper.Map<List<KeyDTO>>(keys);
        }

        /// <summary>
        /// Method to create a new Api Key
        /// </summary>
        /// <param name="keyCreationDTO">Object with key creation data</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateKey")]
        public async Task<ActionResult> CreateKey(KeyCreationDTO keyCreationDTO)
        {
            var userId = GetUserId();

            if(keyCreationDTO.KeyType == KeyType.Free)
            {
                var checkKeyType = await _context.APIKey.AnyAsync(k => k.UserId == userId && k.KeyType == KeyType.Free);

                if(checkKeyType)
                {
                    return BadRequest("El usuario ya tiene una llave gratuita");
                }
            }

            await _keyService.CreateKey(userId, keyCreationDTO.KeyType);

            return NoContent();
        }

        /// <summary>
        /// Method to Update an user´s key
        /// </summary>
        /// <param name="keyUpdateDTO">Object with the information to update</param>
        /// <returns></returns>
        [HttpPut(Name = "updateKey")]
        public async Task<ActionResult> UpdateKey(KeyUpdateDTO keyUpdateDTO)
        {
            var userId = GetUserId();

            var keyDB = await _context.APIKey.FirstOrDefaultAsync(k => k.Id == keyUpdateDTO.KeyId);

            if(keyDB == null)
            {
                return NotFound();
            }

            if(userId != keyDB.UserId)
            {
                return Forbid();
            }

            if (keyUpdateDTO.UpdateKey)
            {
                keyDB.Key = _keyService.GenerateKey();
            }

            keyDB.Active = keyUpdateDTO.Active;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
