using WebApiAuthors.Entities;

namespace WebApiAuthors.Services
{
    public class KeyService
    {
        private readonly ApplicationDbContext _context;

        public KeyService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to create a Key to manage the subscription
        /// </summary>
        /// <param name="userId">Id Of the user</param>
        /// <param name="keyType">Type of the key to generate</param>
        /// <returns></returns>
        public async Task CreateKey(string userId, KeyType keyType)
        {
            var key = GenerateKey();

            var apiKey = new APIKey()
            {
                Active = true,
                Key = key,
                KeyType = keyType,
                UserId = userId
            };

            _context.Add(apiKey);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Method to generate a random string for the Key
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
