using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DataAccess;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationContext _context;

        private const string SecretKey = "NaphatChartwanchaiNaphatChartwanchaiLdza01"; 
        private const string Issuer = "phat";
        private const string Audience = "saeroy";

        public ProductController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet("UserLogin")]
        public IActionResult UserLogin()
        {
            var token = GenerateToken("User");
            return Ok(new { token });
        }

        [HttpGet("AdminLogin")]
        public IActionResult AdminLogin()
        {
            var token = GenerateToken("Admin");
            return Ok(new { token });
        }

        private string GenerateToken(string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("Products")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult Get()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("Product/{id}")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult GetProduct([FromRoute] int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("Products")]
        [Authorize(Roles = "Admin")]
        public IActionResult Save([FromBody] Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut("Products")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody] Product product)
        {
            var result = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == product.Id);
            if (result == null)
            {
                return NotFound();
            }
            _context.Products.Update(product);
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("Products")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromQuery] int id)
        {
            var deleteProduct = _context.Products.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (deleteProduct == null)
            {
                return NotFound();
            }
            _context.Products.Remove(deleteProduct);
            _context.SaveChanges();
            return Ok();
        }

    }
}