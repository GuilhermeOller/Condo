using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Acesso_Moradores_Visitantes.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[Route("[controller]/[action]")]
public class VisitanteController : Controller
{
    private readonly IVisitanteService _visitanteService;
    private readonly IVisitanteRepository _visitanteRepository;
    private readonly AppDbContext _context;

    public VisitanteController(AppDbContext context, IVisitanteService visitanteService, IVisitanteRepository visitanteRepository)
    {
        _context = context;
        _visitanteService = visitanteService;
        _visitanteRepository = visitanteRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> ObterVisitantes()
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;
        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        var visitantes = await _visitanteRepository.ObterVisitantesAsync(idUsuario, tipoLogin);

        return Json(visitantes);
    }

    [HttpGet("{idVisitante}")]
    public async Task<IActionResult> ObterVisitantePorId(int idVisitante)
    {
        try
        {
            var visitante = await _context.tblVisitante
                .FirstOrDefaultAsync(v => v.idVisitante == idVisitante);

            if (visitante == null)
                return NotFound("Visitante não encontrado");

            return Ok(visitante);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar visitante: {ex.Message}");
        }
    }

    [HttpGet("{idFesta}")]
    public async Task<IActionResult> ObterFestaPorId(int idFesta)
    {
        try
        {
            var festa = await _context.tblFestas
                .FirstOrDefaultAsync(v => v.id == idFesta);

            if (festa == null)
                return NotFound("Festa não encontrada");

            return Ok(festa);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar festa: {ex.Message}");
        }
    }

    [HttpGet("{idFesta}")]
    public async Task<IActionResult> ObterVisitantesPorFesta(int idFesta)
    {
        try
        {
            var visitantes = await _context.tblFestaVisitante
                .Where(v => v.idFesta == idFesta)
                .ToListAsync();

            if (visitantes == null)
                return NotFound("Festa não encontrada");

            return Ok(visitantes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar festa: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AdicionarVisitante([FromBody] Visitantes visitante)
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;

        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.AdcionarVisitanteAsync(visitante, idUsuario, tipoLogin);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new {mensagem = resultado.Mensagem, id = resultado.idVisitante });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UploadFotos(IFormCollection form)
    {
        try
        {
            var resultado = await _visitanteService.UploadFotoAsync(form);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirVisitante(long id)
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;

        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.ExcluirVisitanteAsync(id, idUsuario, tipoLogin);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new {mensagem = resultado.Mensagem});
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> LiberarVisitante([FromBody] Acessos acesso, string tipo, string nomeFesta, short metodo, string convite, string tipoVisitante)
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;
        var nomeLogin = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.LiberarVisitanteAsync(acesso, tipo, nomeFesta, metodo, convite, tipoVisitante, idUsuario, tipoLogin, nomeLogin);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterAcessosEmAberto()
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;


        if (!long.TryParse(cookie, out long idUsuario))
            return Unauthorized("Usuário não autenticado ou ID inválido.");

        try
        {
            var acessos = await _context.tblAcessos
                        .Where(v =>
                            v.idMorador == idUsuario &&
                            v.TipoVisitado == tipoLogin &&
                            v.DtSaida == null)
                        .Select(v => new {
                            v.IdAcesso,
                            v.VisitanteInfo.nomeVisitante,
                            v.DtEntrada,
                            v.DtSaida,
                            v.de,
                            v.ate
                        })
                        .ToListAsync();

            return Json(acessos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar acessos: {ex.Message}\n{ex.StackTrace}");
        }
    }

    [HttpPut("{idAcesso}")]
    public async Task<IActionResult> BaixarAcesso(long idAcesso)
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";

        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.BaixarAcessoAsync(idAcesso);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    public async Task<IActionResult> ObterFestasPorUsuario()
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;


        if (!long.TryParse(cookie, out long idUsuario))
            return Unauthorized("Usuário não autenticado ou ID inválido.");

        var festas = await _visitanteRepository.ObterFestasPorUsuarioAsync(idUsuario, tipoLogin);

        return Json(festas);
    }

    [HttpGet]
    public IActionResult ObterNomeMorador()
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var nomeLogin = User.FindFirst(ClaimTypes.Name)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;
        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return BadRequest("Id não encontrado.");
        string nomeUsuario = $"{tipoLogin}: {nomeLogin}";


        return Json(new { usuario = nomeUsuario });
    }

    [HttpPost]
    public async Task<IActionResult> CadastrarFesta([FromBody] Festas festa)
    {

        var visitantes = festa.Visitantes;
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var nomeLogin = User.FindFirst(ClaimTypes.Name)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;
        string convite = "";

        if (string.IsNullOrEmpty(cookie) || !int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.CadastrarFestaAsync(festa, idUsuario, tipoLogin, nomeLogin);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirFesta(int id)
    {
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        if (!int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.ExcluirFestaAsync(id, idUsuario);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarFesta([FromBody] Festas festa)
    {
        var cookie = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
        var nomeLogin = User.FindFirst(ClaimTypes.Name)?.Value ?? "0";
        var tipoLogin = User.FindFirst("app:TipoLogin")?.Value;
        if (!int.TryParse(cookie, out int idUsuario))
            return Unauthorized();

        try
        {
            var resultado = await _visitanteService.AtualizarFestaAsync(festa, idUsuario, tipoLogin, nomeLogin);

            if (!resultado.Sucesso)
                return Unauthorized(new { erro = resultado.Mensagem });

            return Ok(new { mensagem = resultado.Mensagem });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = "Erro interno: " + ex.Message });
        }
    }

}
