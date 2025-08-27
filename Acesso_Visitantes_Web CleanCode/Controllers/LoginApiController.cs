using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Acesso_Moradores_Visitantes.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginApiController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginApiController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                var resultado = await _loginService.AutenticarAsync(login, HttpContext);

                if (!resultado.Sucesso)
                    return Unauthorized(new { erro = resultado.Mensagem });

                return Ok(new { mensagem = resultado.Mensagem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Login login)
        {
            try
            {
                var resultado = await _loginService.CadastrarAsync(login, HttpContext);
                if (!resultado.Sucesso)
                    return Unauthorized(new { erro = resultado.Mensagem });

                return Ok(new { mensagem = resultado.Mensagem});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> EnviarToken([FromBody] EnviarTokenDto dados)
        {
            try
            {
                var resultado = await _loginService.EnviarTokenAsync(dados.Email, HttpContext, dados.Ip);
                if (!resultado.Sucesso)
                    return Unauthorized(new { erro = resultado.Mensagem });

                return Ok(new { mensagem = resultado.Mensagem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
            }
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> VerificaToken(string token)
        {
            try
            {
                var resultado = await _loginService.VerificaTokenAsync(token, HttpContext);
                if (!resultado.Sucesso)
                    return Unauthorized(new { erro = resultado.Mensagem });

                return Ok(new { mensagem = resultado.Mensagem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno: " + ex.Message + ex.InnerException });
            }
        }

        [HttpPut("{novaSenha}")]
        public async Task<IActionResult> UpdateSenha(string novaSenha)
        {
            try
            {
                var resultado = await _loginService.UpdateSenhaAsync(novaSenha, HttpContext);
                if (!resultado.Sucesso)
                    return Unauthorized(new { erro = resultado.Mensagem });

                return Ok(new { mensagem = resultado.Mensagem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno: " + ex.Message + ex.InnerException });
            }
        }
    }
}