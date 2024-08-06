using CmsResources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ServicesLibrary.UserServices;
using ServicesLibrary.MedicalCenterServices;
using CmsDataAccess.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsWeb.Hubs;

namespace CmsApi.API.Address
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IHubContext<MyHub> _hubContext;
        private readonly IMedicalCenterService _medicalCenterService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<AddressController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _cmsContext;

        public AddressController(
            IHubContext<MyHub> hubContext,
            IMedicalCenterService medicalCenterService,
            IStringLocalizer<Messages> localizer,
            IConfiguration config,
            IUserService userService,
            ApplicationDbContext cmsContext,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILogger<AddressController> logger)
        {
            _hubContext = hubContext;
            _medicalCenterService = medicalCenterService;
            _localizer = localizer;
            _config = config;
            _userService = userService;
            _cmsContext = cmsContext;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet("get-address")]
        public async Task<ActionResult<IEnumerable<CmsDataAccess.DbModels.Address>>> GetAddresses()
        {
            try
            {
                var addresses = await _cmsContext.Address
                                    .Include(a => a.AddressTranslation)
                                    .ToListAsync();
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching addresses");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: /Address/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CmsDataAccess.DbModels.Address>> GetAddress(Guid id)
        {
            var address = await _cmsContext.Address
                .Include(a => a.AddressTranslation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // POST: /Address
        [HttpPost("post-address")]
        public async Task<ActionResult<CmsDataAccess.DbModels.Address>> PostAddress(CmsDataAccess.DbModels.Address address)
        {
            if (address.AddressTranslation != null && address.AddressTranslation.Any())
            {
                foreach (var translation in address.AddressTranslation)
                {
                    translation.Id = Guid.NewGuid(); // Ensure each translation has a unique ID
                    _cmsContext.AddressTranslation.Add(translation);
                }
            }

            _cmsContext.Address.Add(address);
            await _cmsContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }

        // PUT: /Address/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(Guid id, CmsDataAccess.DbModels.Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            _cmsContext.Entry(address).State = EntityState.Modified;

            if (address.AddressTranslation != null && address.AddressTranslation.Any())
            {
                foreach (var translation in address.AddressTranslation)
                {
                    _cmsContext.Entry(translation).State = translation.Id == Guid.Empty
                        ? EntityState.Added
                        : EntityState.Modified;
                }
            }

            try
            {
                await _cmsContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: /Address/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var address = await _cmsContext.Address.Include(a => a.AddressTranslation).FirstOrDefaultAsync(a => a.Id == id);
            if (address == null)
            {
                return NotFound();
            }

            _cmsContext.Address.Remove(address);
            await _cmsContext.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(Guid id)
        {
            return _cmsContext.Address.Any(e => e.Id == id);
        }
    }
}
