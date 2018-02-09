using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using modeloaulaefjwt.Models;
using modelobasicoefjwt.Models;
using modelobasicoefjwt.Repositorio;

namespace modeloaulaefjwt.Controllers
{
    public class LoginController : Controller
    {

        readonly AutenticacaoContext _contexto;

        public LoginController(AutenticacaoContext contexto){
            _contexto = contexto;
        }

        [HttpPost]
        public IActionResult Validar([FromBody]Usuario usuario,
                                    [FromServices]SigningConfigurations signingConfigurations,
                                    [FromServices]TokenConfigurations tokenConfigurations){
            Usuario user = _contexto.Usuarios.FirstOrDefault(c=> c.Email == usuario.Email &&
                                                                c.Senha == usuario.Senha);
                 if(user !=null){
                     ClaimsIdentity identity = new ClaimsIdentity(
                         new GenericIdentity(user.IdUsuario.ToString(),"Login"),
                         new[]{
                             new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString("N")),
                             new Claim(JwtRegisteredClaimNames.UniqueName,user.IdUsuario.ToString()),
                             new Claim("Nome",user.Nome),
                             new Claim(ClaimTypes.Email,user.Email)}
                     );

                var handler = new JwtSecurityTokenHandler();
                var securityToken = handler.CreateToken(new SecurityTokenDescriptor{
                    Issuer = tokenConfigurations.Issuer,
                    Audience = tokenConfigurations.Audience,
                    SigningCredentials = signingConfigurations.SigningCredentials,
                    Subject = identity
                });
                
                var token = handler.WriteToken(securityToken);

                var retorno = new{
                    autenticacao = true,
                    accessToken = tokenConfigurations,
                    message = "Ok"
                };
                                         
              return Ok(retorno);  
            }

           var retornoerro = new{
               autenticacao = false,
               message = "Falha na autenticação"
           }; 
           
           return BadRequest(retornoerro);
            
        }
    }
}