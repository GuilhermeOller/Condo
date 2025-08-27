using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Services.Interfaces;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Acesso_Moradores_Visitantes.Services.Implementations
{
    public class VisitanteService : IVisitanteService
    {
        private readonly AppDbContext _context;
        private readonly Geral _geral;
        private readonly IVisitanteRepository _visitanteR;
        private readonly ILogger<Geral> _logger;

        public VisitanteService(AppDbContext context, ILogger<Geral> logger, Geral geral, IVisitanteRepository visitanteR)
        {
            _context = context;
            _logger = logger;
            _geral = geral;
            _visitanteR = visitanteR;
        }

        public async Task<(bool Sucesso, string Mensagem, long idVisitante)> AdcionarVisitanteAsync(Visitantes visitante, long idUsuario, string tipoLogin)
        {
            if (visitante != null)
            {
                var stringProperties = visitante.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

                foreach (var property in stringProperties)
                {
                    var value = (string)property.GetValue(visitante);
                    if (!string.IsNullOrWhiteSpace(value))
                        property.SetValue(visitante, value.ToUpper());
                }
            }

            visitante.FlgStatus = "A";
            visitante.FlgTipo = string.IsNullOrWhiteSpace(visitante.Empresa) ? "V" : "P";

            // Verifica se já existe um visitante com o mesmo nome
            Visitantes visitanteExistente = await _visitanteR.BuscarVisitantePorNomeAsync(visitante.Visitante);

            if (visitanteExistente != null)
            {
                visitanteExistente.Visitante = visitante.Visitante;
                visitanteExistente.Empresa = visitante.Empresa;
                visitanteExistente.FoneCelular = visitante.FoneCelular;
                visitanteExistente.email = visitante.email;
                visitanteExistente.Endereco = visitante.Endereco;
                visitanteExistente.Numero = visitante.Numero;
                visitanteExistente.Bairro = visitante.Bairro;
                visitanteExistente.Cidade = visitante.Cidade;
                visitanteExistente.Estado = visitante.Estado;
                visitanteExistente.CEP = visitante.CEP;
                visitanteExistente.TpDocto = visitante.TpDocto;
                visitanteExistente.NumDocto = visitante.NumDocto;
                visitanteExistente.DtNascimento = visitante.DtNascimento;
                visitanteExistente.Placa = visitante.Placa;
                visitanteExistente.Veiculo = visitante.Veiculo;
                visitanteExistente.Marca = visitante.Marca;
                visitanteExistente.Modelo = visitante.Modelo;
                visitanteExistente.Cor = visitante.Cor;
                visitanteExistente.temCarro = visitante.temCarro;

                await _visitanteR.AtualizarVisitanteAsync(visitanteExistente);

                var relacaoExiste = await _visitanteR.VerificaRealacaoUsuarioVisitante(idUsuario, visitanteExistente.idVisitante, tipoLogin);

                if (relacaoExiste == null)
                {
                    var novaRelacao = new UsuarioVisitante
                    {
                        IdUsuario = idUsuario,
                        Tipo = tipoLogin,
                        IdVisitante = visitanteExistente.idVisitante,
                        DataCadastro = DateTime.Now
                    };
                    await _visitanteR.AdicionarRelacaoUsuarioVisitanteAsync(novaRelacao);
                }

                return (true, "Visitante Cadastrado com sucesso!", visitanteExistente.idVisitante);
            }
            else
            {
                await _visitanteR.AdicionarVisitanteAsync(visitante);

                var usuarioVisitante = new UsuarioVisitante
                {
                    IdUsuario = idUsuario,
                    Tipo = tipoLogin,
                    IdVisitante = visitante.idVisitante,
                    DataCadastro = DateTime.Now
                };

                await _visitanteR.AdicionarRelacaoUsuarioVisitanteAsync(usuarioVisitante);

                return (true, "Visitante cadastrado com sucesso!", visitante.idVisitante);
            }
        }
        public async Task<(bool Sucesso, string Mensagem)> UploadFotoAsync(IFormCollection form)
        {
            if (!int.TryParse(form["idVisitante"], out int idVisitante))
                return (false, "Erro ao salvar imagem!");

            string pasta = await _visitanteR.BuscarPathImagesAsync();

            if (pasta == null)
            {
                return (false, "Caminho não registrado, entrar em contato com condomínio");
            }

            var fotoVisitante = form.Files["fotoVisitante"];
            var fotoFrente = form.Files["fotoFrente"];
            var fotoVerso = form.Files["fotoVerso"];

            try
            {
                if (fotoVisitante != null)
                    await SalvarArquivoAsync(fotoVisitante, Path.Combine(pasta, $"V{idVisitante:D10}.jpg"));
                if (fotoFrente != null)
                    await SalvarArquivoAsync(fotoFrente, Path.Combine(pasta, $"VF{idVisitante:D10}.jpg"));
                if (fotoVerso != null)
                    await SalvarArquivoAsync(fotoVerso, Path.Combine(pasta, $"VV{idVisitante:D10}.jpg"));

                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao cadastrar foto: {ex.Message}");
            }
        }

        private async Task SalvarArquivoAsync(IFormFile arquivo, string caminho)
        {
            using var stream = new FileStream(caminho, FileMode.Create);
            await arquivo.CopyToAsync(stream);
        }

        public async Task<(bool Sucesso, string Mensagem)> ExcluirVisitanteAsync(long idVisitante, long idUsuario, string tipoLogin)
        {
            var visitante = await _visitanteR.VerificaRealacaoUsuarioVisitante(idUsuario, idVisitante, tipoLogin);

            if (visitante == null)
                return (false, "Visitante não encontrado");

            await _visitanteR.RemoverRelacaoUsuarioVisitanteAsync(visitante);

            return (true, "Visitante excluido com sucesso!");
        }

        public async Task<(bool Sucesso, string Mensagem)> LiberarVisitanteAsync([FromBody] Acessos acesso, string tipo, string nomeFesta,
            short metodo, string convite, string tipoVisitante, long idUsuario, string tipoLogin, string nomeLogin)
        {
            DateTime deData = acesso.de;
            DateTime ateData = acesso.ate;
            long idOrigem = 0;
            long idVisitante = acesso.IdOrigem;
            string nomeMorador = null;
            using var transaction = await _context.Database.BeginTransactionAsync();

            if (string.IsNullOrEmpty(acesso.ip))
            {
                //return (false, "");
            }
            try
            {
                var visitante = await _visitanteR.BuscarVisitantePorIdAsync(idVisitante);

                if (visitante == null) return (false, "Visitante não encontrado");

                var acessoEmAberto = await _visitanteR.VerificaAcessoEmAbertoAsync(idVisitante, idUsuario, tipoLogin);

                if (acessoEmAberto)
                {
                    return (false, "Visitante já possui um acesso em aberto. Finalize o acesso anterior antes de liberar um novo.");
                }

                string numCrachaString = DateTime.Now.ToString("HHmmssdd");
                int numCracha = int.Parse(numCrachaString);
                var novoAcesso = new Acessos
                {
                    NumCracha = numCracha,
                    CrachaLiberado = 1,
                    FlgOrigem = "V",
                    IdOrigem = idVisitante,
                    IdVisitado = idUsuario,
                    FlgPortao = "E",
                    FlgTpAcesso = "W",
                    DtEntrada = null,
                    DtSaida = null,
                    DtAlteracao = DateTime.Now,
                    FlgExcluido = 0,
                    PortaoAcesso = 1,
                    entrada = 1,
                    saida = 0,
                    caminhao = 0,
                    de = deData,
                    ate = ateData,
                    TipoVisitado = tipoLogin
                };

                await _visitanteR.AdicionarNovoAcessoAsync(novoAcesso);

                await _visitanteR.AdicionarNovoVisAcessoAsync(new VisAcesso
                {
                    IdAcesso = novoAcesso.IdAcesso,
                    FlgOrigem = "V",
                    IdOrigem = idVisitante,
                    FlgCondutor = 1,
                    FlgStatusAcesso = "A",
                    Visitante = visitante.Visitante,
                    numcracha = numCracha,
                    DtSaida = null
                });

                if (tipoLogin == "Morador")
                {
                    idOrigem = idUsuario;
                    nomeMorador = nomeLogin;
                }
                if (tipoLogin == "Autorizado")
                {
                    var autorizado = await _visitanteR.BuscarAutorizadoPorIdAsync(idUsuario);

                    if (autorizado != null)
                    {
                        long idMorador = autorizado.IdMorador;
                        string NomeMorador = autorizado.Nome;
                        idOrigem = idMorador;
                        nomeMorador = NomeMorador;
                    }

                }
                await _visitanteR.AdicionarNovoMorAcessoAsync(new MorAcesso
                {
                    IdAcesso = novoAcesso.IdAcesso,
                    IdMorador = idOrigem,
                    Morador = nomeMorador
                });

                //await _context.SaveChangesAsync();
                try
                {
                    await _visitanteR.AdicionarLogAcessoAsync(new LogAcessos
                    {
                        Ip = acesso.ip,
                        NomeUsuario = nomeLogin,
                        NomeVisitante = visitante.Visitante,
                        Data = DateTime.Now
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Falha ao liberar visitante");
                    return (false, $"Erro interno no servidor:{ex}");
                }

                await transaction.CommitAsync();
                string de = deData.ToString("dd/MM/yy HH:mm");
                string ate = ateData.ToString("dd/MM/yy HH:mm");

                try
                {
                    if (metodo == 0)
                    {
                        await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, tipo, nomeFesta, nomeLogin, null);
                    }
                    else if (metodo == 1)
                    {
                        if (!string.IsNullOrEmpty(convite))
                        {
                            if (tipoVisitante == "n")
                            {
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "festaComConviteUp", nomeFesta, nomeLogin, convite);
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "apenasQr", nomeFesta, nomeLogin, null);
                            }
                            else if (tipoVisitante == "a")
                            {
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "festaComConvite", nomeFesta, nomeLogin, convite);
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "apenasQr", nomeFesta, nomeLogin, null);
                            }

                        }
                        else
                        {
                            await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, tipo, nomeFesta, nomeLogin, null);
                        }
                    }
                    else if (metodo == 2)
                    {
                        await _geral.EnviarEmailComImagemAsync(visitante.email, nomeFesta, nomeLogin, convite, visitante.Visitante, numCrachaString, de, ate, "Convite", null);
                    }
                    else if (metodo == 3)
                    {
                        if (!string.IsNullOrEmpty(convite))
                        {
                            if (tipoVisitante == "n")
                            {
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "festaComConviteUp", nomeFesta, nomeLogin, convite);
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "apenasQr", nomeFesta, nomeLogin, null);
                            }
                            else if (tipoVisitante == "a")
                            {
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "festaComConvite", nomeFesta, nomeLogin, convite);
                                await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, "apenasQr", nomeFesta, nomeLogin, null);
                            }

                        }
                        else
                        {
                            await _geral.EnviarZapViaZAPIAsync(visitante.FoneCelular, visitante.Visitante, numCrachaString, de, ate, tipo, nomeFesta, nomeLogin, null);
                        }
                        await _geral.EnviarEmailComImagemAsync(visitante.email, nomeFesta, nomeLogin, convite, visitante.Visitante, numCrachaString, de, ate, "Convite", null);
                    }
                }
                catch (Exception ex)
                {
                    return (false, "Erro interno no servidor" + ex);
                }

                return (true, "Visitante liberado!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Falha ao liberar visitante");
                return (false, $"Erro interno no servidor:{ex}");
            }
        }

        public async Task<(bool Sucesso, string Mensagem)> BaixarAcessoAsync(long idAcesso)
        {
            var acesso = await _visitanteR.BuscarAcessoPorIdAsync(idAcesso);
            if (acesso != null)
            {
                acesso.DtSaida = DateTime.Now;
                await _visitanteR.AtualizarAcessoAsync(acesso);
            }
            var visacesso = await _visitanteR.BuscarVisAcessoPorIdAsync(idAcesso);
            if (visacesso != null)
            {
                visacesso.DtSaida = DateTime.Now;
                await _visitanteR.AtualizarVisAcessoAsync(visacesso);
            }
            return (true, "Acesso baixado com sucesso!");
        }

        public async Task<(bool Sucesso, string Mensagem)> CadastrarFestaAsync(Festas festa, long idUsuario, string tipoLogin, string nomeLogin)
        {
            string convite = "";

            if (!string.IsNullOrEmpty(festa.fotoConvite))
            {
                convite = festa.fotoConvite.Replace("data:image/jpeg;base64,", "");
            }

            if (festa != null)
            {
                var stringProperties = festa.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

                foreach (var property in stringProperties)
                {
                    var value = (string)property.GetValue(festa);
                    if (!string.IsNullOrWhiteSpace(value))
                        property.SetValue(festa, value.ToUpper());
                }
            }

            try
            {

                var novaFesta = new Festas
                {
                    nomeFesta = festa.nomeFesta,
                    idUsuario = idUsuario,
                    tipoUsuario = tipoLogin,
                    de = festa.de,
                    ate = festa.ate,
                    fotoConvite = convite,
                    metodo = festa.metodo,
                    diaTodoDe = festa.diaTodoDe,
                    diaTodoAte = festa.diaTodoAte,
                    horaDe = festa.horaDe,
                    horaAte = festa.horaAte
                };

                await _visitanteR.AdicionarNovaFestaAsync(novaFesta);

                foreach (var visitante in festa.Visitantes)
                {
                    var visitanteFesta = new FestaVisitante
                    {
                        idFesta = novaFesta.id,
                        idVisitante = visitante,
                        tipo = "n"
                    };

                    await _visitanteR.AdicionarRelacaoFestaVisitanteAsync(visitanteFesta);

                    var visitanteDados = await _visitanteR.BuscarVisitantePorIdAsync(visitante);

                    if (visitanteDados != null)
                    {

                        var acesso = new Acessos
                        {
                            IdOrigem = visitante,
                            de = novaFesta.de,
                            ate = novaFesta.ate
                        };
                        await LiberarVisitanteAsync(acesso, "festaSemConvite", festa.nomeFesta, festa.metodo, convite, visitanteFesta.tipo, idUsuario, tipoLogin, nomeLogin);
                    }
                }
                return (true, "Festa cadastrada!");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Sucesso, string Mensagem)> ExcluirFestaAsync(long idFesta, long idUsuario)
        {
            try
            {
                // Busca e remove todos os acessos e ligações com visitantes
                var festaVisitantes = await _visitanteR.ObterVisitantesPorFestaAsync(idFesta);

                foreach (var festaVisitante in festaVisitantes)
                {
                    var idAcesso = await _visitanteR.BuscarIdAcessoPorIdAsync(festaVisitante.idVisitante, idUsuario);

                    await BaixarAcessoAsync(idAcesso); 

                   await _visitanteR.RemoverRelacaoFestaVisitanteAsync(festaVisitante);
                }

                var festa = await _visitanteR.BuscarFestaPorIdAsync(idFesta);
                if (festa != null)
                {
                   await _visitanteR.DeletarFestaAsync(festa);
                }

                return (true, "Festa excluida!");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool Sucesso, string Mensagem)> AtualizarFestaAsync(Festas festa, long idUsuario, string tipoLogin, string nomeLogin)
        {
            bool mesmaData = false;
            bool mesmosVisitantes = false;
            bool mesmoMetodo = false;
            string convite = "";

            if (!string.IsNullOrEmpty(festa.fotoConvite))
            {
                convite = festa.fotoConvite.Replace("data:image/jpeg;base64,", "");
            }

            try
            {
                if (festa == null || festa.id == 0)
                    return (false, "Dados inválidos");

                var festaExist = await _visitanteR.BuscarFestaPorIdAsync(festa.id);

                if (festaExist == null)
                    return (false, "Festa não encontrada ou não pertence ao usuário");

                if (festa.de == festaExist.de && festa.ate == festaExist.ate)
                {
                    mesmaData = true;
                }
                if (festa.Visitantes == festaExist.Visitantes)
                {
                    mesmosVisitantes = true;
                }
                if (festa.metodo == festaExist.metodo)
                {
                    mesmoMetodo = true;
                }

                festaExist.nomeFesta = festa.nomeFesta;
                festaExist.de = festa.de;
                festaExist.ate = festa.ate;
                festaExist.fotoConvite = festa.fotoConvite;
                festaExist.metodo = festa.metodo;
                festaExist.diaTodoDe = festa.diaTodoDe;
                festaExist.diaTodoAte = festa.diaTodoAte;
                festaExist.horaDe = festa.horaDe;
                festaExist.horaAte = festa.horaAte;

                await _visitanteR.AtualizarFestaAsync(festaExist);

                if (mesmosVisitantes != true)
                {
                    foreach (var visitante in festa.Visitantes)
                    {
                        var visitanteExist = await _visitanteR.BuscarRelacaoFestaVisitanteAsync(visitante, festa.id);

                        if (visitanteExist == null || visitanteExist != null && mesmoMetodo == false)
                        {
                            var visitanteFesta = new FestaVisitante
                            {
                                idFesta = festa.id,
                                idVisitante = visitante,
                                tipo = "a"
                            };

                            await _visitanteR.AdicionarRelacaoFestaVisitanteAsync(visitanteFesta);
                            var acesso = new Acessos
                            {
                                IdOrigem = visitante,
                                de = festa.de,
                                ate = festa.ate,
                            };
                            await LiberarVisitanteAsync(acesso, "festaSemConvite", festa.nomeFesta, festa.metodo, convite, visitanteFesta.tipo, idUsuario, tipoLogin, nomeLogin);
                        }
                        else if (visitanteExist != null && mesmaData == false)
                        {
                            var acesso = new Acessos
                            {
                                IdOrigem = visitante,
                                de = festa.de,
                                ate = festa.ate
                            };
                            var idAcesso = await _visitanteR.BuscarIdAcessoPorIdAsync(visitante, idUsuario);

                            await BaixarAcessoAsync(idAcesso);

                            await LiberarVisitanteAsync(acesso, "festaSemConviteUp", festa.nomeFesta, festa.metodo, convite, visitanteExist.tipo, idUsuario, tipoLogin, nomeLogin);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                return (true, "Festa atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
