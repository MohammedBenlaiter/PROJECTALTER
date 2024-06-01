using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using PROJECTALTERAPI.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.CodeDom.Compiler;
namespace PROJECTALTERAPI;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private IConfiguration _configuration;
    private readonly AlterDbContext _context;

    public WishlistController(AlterDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    [HttpPost("CreateWishlist/{id}")]
    public async Task<IActionResult> CreateWishlist(WishlistDto wishlistDto, long id)
    {
        //var userId = GetCurrentUser();
        var wishlist = new Wishlist
        {
            UserId = id,
            WishlistName = wishlistDto.WishlistName
        };
        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
        return Ok(wishlist);
    }
}
