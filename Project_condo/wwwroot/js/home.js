document.addEventListener("DOMContentLoaded", () => {
    carregarVisitantes();
    exibirNomeMorador();

    // Formatação de telefone
    document.getElementById("foneCelular").addEventListener("input", function (e) {
        formatarTelefone(e.target);
    });
    document.getElementById("editFoneCelular").addEventListener("input", function (e) {
        formatarTelefone(e.target);
    });
    // Formulário de cadastro
    document.getElementById("formVisitante").addEventListener("submit", async (e) => {
        e.preventDefault();
        const submitBtn = e.target.querySelector('button[type="submit"]');

        try {
            setButtonLoading(submitBtn, true);
            const telefone = document.getElementById("foneCelular").value.replace(/\D/g, '');

            if (telefone.length < 11) {
                showNotification('Por favor, insira um número de celular válido (DDD + número)', 'error');
                return;
            }
            const cadastro = {
                nomeVisitante: document.getElementById("visitante").value,
                celularVisitante: telefone,
                email: document.getElementById("email").value,
            };

            const response = await fetch('/Visitante/AdicionarVisitante', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(cadastro)
            });

            if (!response.ok) {
                throw new Error('Erro ao cadastrar visitante');
            }

            const result = await response.json();
            const Mensagem = result.mensagem;

            showNotification(Mensagem, 'success');
            const modal = bootstrap.Modal.getInstance(document.getElementById('formularioVisitante'));
            if (modal) modal.hide();

            carregarVisitantes();
            carregarVisitantesNaTabela();
        } catch (error) {
            console.error('Erro:', error);
            showNotification('Erro ao cadastrar visitante', 'error');
        } finally {
            setButtonLoading(submitBtn, false);
        }
    });
});

// Funções auxiliares
function showNotification(message, type) {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.innerHTML = `
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'} me-2"></i>
                ${message}
            `;
    document.body.appendChild(notification);

    setTimeout(() => {
        notification.classList.add('show');
    }, 10);

    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => {
            notification.remove();
        }, 300);
    }, 3000);
}

function setButtonLoading(button, isLoading) {
    if (!button) return;

    if (isLoading) {
        button.classList.add('btn-loading');
        button.disabled = true;
        const originalText = button.innerHTML;
        button.setAttribute('data-original-text', originalText);
        button.innerHTML = `<div class="loading-spinner"></div><span style="visibility: hidden;">${originalText}</span>`;
    } else {
        button.classList.remove('btn-loading');
        button.disabled = false;
        const originalText = button.getAttribute('data-original-text') || button.textContent;
        button.innerHTML = originalText;
    }
}

function formatarTelefone(input) {
    let numero = input.value.replace(/\D/g, '');
    if (numero.length > 0) {
        numero = `(${numero.substring(0, 2)}) ${numero.substring(2, 7)}-${numero.substring(7, 11)}`;
    }
    input.value = numero;
}

// Funções principais
async function carregarVisitantes() {
    const container = document.getElementById("visitantesCards");
    container.innerHTML = `
                <div class="col-12 text-center py-4">
                    <div class="loading-spinner" style="width: 3rem; height: 3rem; border-width: 0.3em;"></div>
                    <p class="mt-2">Carregando visitantes...</p>
                </div>
            `;

    try {
        const res = await fetch('/Visitante/ObterVisitantes', {
            method: 'GET',
            credentials: 'include'
        });

        if (!res.ok) {
            throw new Error('Erro ao carregar visitantes');
        }

        const visitantes = await res.json();
        container.innerHTML = "";

        if (visitantes.length === 0) {
            container.innerHTML = `
                        <div class="col-12">
                            <div class="alert alert-info text-center">
                                <i class="fas fa-info-circle me-2"></i>
                                Nenhum visitante cadastrado ainda.
                            </div>
                        </div>
                    `;
            return;
        }

        visitantes.forEach(v => {
            container.innerHTML += `
                        <div class="col">
                            <div class="card h-100 shadow-sm">
                                <div class="card-body">
                                    <h5 class="card-title">
                                        <i class="fas fa-user me-2"></i>${v.nomeVisitante}
                                    </h5>
                                    <div class="d-flex justify-content-between mt-3">
                                        <button class="btn btn-sm btn-danger btn-wave" onclick="excluirVisitante(${v.idVisitante})">
                                            <i class="fas fa-trash me-1"></i> Excluir
                                        </button>
                                        <button class="btn btn-sm btn-primary btn-wave" onclick="editarVisitante(${v.idVisitante})">
                                            <i class="fas fa-save me-1"></i> Editar
                                        </button>
                                        <button class="btn btn-sm btn-success btn-wave" onclick="liberarVisitante(${v.idVisitante}, '${v.nomeVisitante}')">
                                            <i class="fas fa-check me-1"></i> Liberar
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
        });

        exibirNomeMorador();
    } catch (error) {
        console.error("Erro ao carregar visitantes:", error);
        showNotification('Erro ao carregar visitantes', 'error');
    }
}

async function carregarAcessosEmAberto() {
    const container = document.getElementById("acessosCards");
    container.innerHTML = `
                <div class="col-12 text-center py-4">
                    <div class="loading-spinner" style="width: 3rem; height: 3rem; border-width: 0.3em;"></div>
                    <p class="mt-2">Carregando acessos...</p>
                </div>
            `;

    try {
        const res = await fetch('/Visitante/ObterAcessosEmAberto', {
            method: 'GET',
            credentials: 'include'
        });

        if (!res.ok) {
            throw new Error('Erro ao carregar acessos');
        }

        const acessos = await res.json();
        container.innerHTML = "";

        if (acessos.length === 0) {
            container.innerHTML = `
                        <div class="col-12">
                            <div class="alert alert-info text-center">
                                <i class="fas fa-info-circle me-2"></i>
                                Nenhum acesso em aberto.
                            </div>
                        </div>
                    `;
        } else {
            acessos.forEach(a => {
                const dataDe = new Date(a.de).toLocaleDateString('pt-BR');
                const dataAte = new Date(a.ate).toLocaleDateString('pt-BR');
                container.innerHTML += `
                            <div class="col-md-6 mb-3">
                                <div class="card h-100 shadow-sm">
                                    <div class="card-body">
                                        <h5 class="card-title">
                                            <i class="fas fa-user me-2"></i>${a.visitante}
                                        </h5>
                                        <p class="card-text"><strong>De:</strong> ${dataDe}</p>
                                        <p class="card-text"><strong>Até:</strong> ${dataAte}</p>

                                        <div class="d-flex justify-content-end mt-3">
                                            <button class="btn btn-sm btn-danger" onclick="baixarAcesso(${a.idAcesso})">
                                                <i class="fas fa-trash me-1"></i> Baixar
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
            });
        }

        const modal = new bootstrap.Modal(document.getElementById('modalAcessos'));
        modal.show();
    } catch (error) {
        console.error("Erro ao carregar acessos:", error);
        showNotification('Erro ao carregar acessos em aberto', 'error');
    }
}

async function carregarFestas() {
    const container = document.getElementById("festasCards");
    container.innerHTML = `
                <div class="col-12 text-center py-4">
                    <div class="loading-spinner" style="width: 3rem; height: 3rem; border-width: 0.3em;"></div>
                    <p class="mt-2">Carregando festas...</p>
                </div>
            `;

    try {
        const res = await fetch('/Visitante/ObterFestasPorUsuario', {
            method: 'GET',
            credentials: 'include'
        });

        if (!res.ok) {
            throw new Error('Erro ao carregar festas');
        }

        const festas = await res.json();
        container.innerHTML = "";

        if (festas.length === 0) {
            container.innerHTML = `
                        <div class="col-12">
                            <div class="alert alert-info text-center">
                                <i class="fas fa-info-circle me-2"></i>
                                Nenhuma festa cadastrada.
                            </div>
                        </div>
                    `;
        } else {
            festas.forEach(f => {
                const dataDe = new Date(f.de).toLocaleDateString('pt-BR');
                const dataAte = new Date(f.ate).toLocaleDateString('pt-BR');
                container.innerHTML += `
                            <div class="col-md-6 mb-3">
                                <div class="card h-100 shadow-sm">
                                    <div class="card-body">
                                        <h5 class="card-title">
                                            <i class="fas fa-users me-2"></i>${f.nomeFesta}
                                        </h5>
                                        <p class="card-text"><strong>De:</strong> ${dataDe}</p>
                                        <p class="card-text"><strong>Até:</strong> ${dataAte}</p>

                                        <div class="d-flex justify-content-end mt-3">
                                            <button class="btn btn-sm btn-danger" onclick="baixarFesta(${f.id})">
                                                <i class="fas fa-trash me-1"></i> Excluir
                                            </button>
                                            <button class="btn btn-sm btn-primary" onclick="editarFesta(${f.id})">
                                                <i class="fas fa-save me-1"></i> Editar Festa
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `;
            });
        }

        const modal = new bootstrap.Modal(document.getElementById('modalFestas'));
        modal.show();
    } catch (error) {
        console.error("Erro ao carregar festas:", error);
        showNotification('Erro ao carregar festas', 'error');
    }
}

async function baixarFesta(id) {
    if (!confirm("Tem certeza que deseja excluir esta festa?")) return;

    const buttons = document.querySelectorAll(`button[onclick="baixarFesta(${id})"]`);
    buttons.forEach(btn => setButtonLoading(btn, true));

    try {
        const res = await fetch(`/Visitante/ExcluirFesta/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });

        if (res.ok) {
            showNotification('Festa excluída com sucesso', 'success');
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalFestas'));
            if (modal) modal.hide();
            carregarFestas();
        } else {
            throw new Error('Erro ao excluir festa');
        }
    } catch (error) {
        console.error("Erro ao excluir festa", error);
        showNotification('Erro ao excluir festa', 'error');
    } finally {
        buttons.forEach(btn => setButtonLoading(btn, false));
    }
}

async function logout() {
    try {
        const res = await fetch('/Visitante/Logout', {
            method: 'POST',
            credentials: 'include'
        });

        if (res.ok) {
            showNotification('Logout realizado com sucesso', 'success');
            setTimeout(() => {
                window.location.href = "/Login";
            }, 1500);
        } else {
            throw new Error('Erro ao fazer logout');
        }
    } catch (error) {
        console.error("Erro ao fazer logout:", error);
        showNotification('Erro ao fazer logout', 'error');
    }
}

async function excluirVisitante(id) {
    if (!confirm("Tem certeza que deseja excluir este visitante?")) return;

    const buttons = document.querySelectorAll(`button[onclick="excluirVisitante(${id})"]`);
    buttons.forEach(btn => setButtonLoading(btn, true));

    try {
        const res = await fetch(`/Visitante/ExcluirVisitante/${id}`, {
            method: 'DELETE',
            credentials: 'include'
        });

        if (res.ok) {
            showNotification('Visitante excluído com sucesso', 'success');
            carregarVisitantes();
        } else {
            throw new Error('Erro ao excluir visitante');
        }
    } catch (error) {
        console.error("Erro ao excluir visitante:", error);
        showNotification('Erro ao excluir visitante', 'error');
    } finally {
        buttons.forEach(btn => setButtonLoading(btn, false));
    }
}

async function baixarAcesso(idAcesso) {
    if (!confirm("Tem certeza que deseja baixar este acesso?")) return;

    const buttons = document.querySelectorAll(`button[onclick="baixarAcesso(${idAcesso})"]`);
    buttons.forEach(btn => setButtonLoading(btn, true));

    try {
        const res = await fetch(`/Visitante/BaixarAcesso/${idAcesso}`, {
            method: 'PUT',
            credentials: 'include'
        });

        if (res.ok) {
            showNotification('Acesso baixado com sucesso', 'success');
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalAcessos'));
            if (modal) modal.hide();
            carregarAcessosEmAberto();
        } else {
            throw new Error('Erro ao baixar acesso');
        }
    } catch (error) {
        console.error("Erro ao baixar acesso", error);
        showNotification('Erro ao baixar acesso', 'error');
    } finally {
        buttons.forEach(btn => setButtonLoading(btn, false));
    }
}

function liberarVisitante(id, visitante) {
    const modal = new bootstrap.Modal(document.getElementById('periodoAcesso'));
    modal.show();
    document.getElementById("idPeriodoAcesso").textContent = id;
    document.getElementById("nomeVisitante").textContent = visitante;
}

function formatarDataLocal(data) {
    const ano = data.getFullYear();
    const mes = String(data.getMonth() + 1).padStart(2, '0');
    const dia = String(data.getDate()).padStart(2, '0');
    const hora = String(data.getHours()).padStart(2, '0');
    const minuto = String(data.getMinutes()).padStart(2, '0');

    return `${ano}-${mes}-${dia}T${hora}:${minuto}`;
}

async function liberarVisitanteComPeriodo() {
    const liberarBtn = document.querySelector('#periodoAcesso .btn-primary');
    setButtonLoading(liberarBtn, true);

    try {
        if (!confirm("Deseja liberar este visitante?")) return;

        const deInput = document.getElementById("de").value;
        const ateInput = document.getElementById("ate").value;
        const horasDeInput = document.getElementById("horasDe").value;
        const horasAteInput = document.getElementById("horasAte").value;

        if (deInput == null || ateInput == null || deInput == "" || ateInput == "") {
            showNotification('Digite uma data', 'error');
            return;
        }

        const de = new Date(deInput);
        const ate = new Date(ateInput);
        const hoje = new Date();

        de.setHours(0, 0, 0, 0);
        ate.setHours(0, 0, 0, 0);
        hoje.setHours(0, 0, 0, 0);

        if (deInput && ateInput) {
            const hojeReal = (hoje - 1) / (1000 * 60 * 60 * 24);

            if (de < hojeReal || ate < hojeReal || ate < de) {
                showNotification('Selecione uma data válida.', 'error');
                return;
            }

            const diffEmDias = (ate - de) / (1000 * 60 * 60 * 24);

            if (diffEmDias > 30) {
                showNotification('Limite de 30 dias', 'error')
                return;
            }

            if (de == null || ate == null || de == " " || ate == " ") {
                showNotification('Digite uma data', 'error');
                return;
            }
            if ((horasDeInput == "" && !document.getElementById("diaTodoDe").checked) || (horasAteInput == "" && !document.getElementById("diaTodoAte").checked)) {
                showNotification('Expecifique um horario', 'error');
                return;
            }
        }
        const deFinal = new Date(deInput + "T" + (document.getElementById("diaTodoDe").checked ? "00:00" : horasDeInput));
        const ateFinal = new Date(ateInput + "T" + (document.getElementById("diaTodoAte").checked ? "23:59" : horasAteInput));

        const acesso = {
            IdOrigem: document.getElementById("idPeriodoAcesso").textContent,
            de: formatarDataLocal(deFinal),
            ate: formatarDataLocal(ateFinal),
            ip: await obterIpPublico()
        };
        const tipo = "normal"
        const metodo = 0
        const convite = null
        const nomeFesta = null

        const response = await fetch('/Visitante/liberarVisitante', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(acesso, tipo, nomeFesta, metodo, convite)
        });

        if (response.ok) {
            showNotification('Visitante liberado com sucesso!', 'success');
            const modal = bootstrap.Modal.getInstance(document.getElementById('periodoAcesso'));
            if (modal) modal.hide();
            carregarVisitantes();
        } else {
            const errorText = await response.text();
            throw new Error(errorText || 'Erro ao liberar visitante');
        }
    } catch (error) {
        console.error("Erro ao liberar visitante:", error);
        showNotification(error.message, 'error');
    } finally {
        setButtonLoading(liberarBtn, false);
    }
}

function mostrarFormulario() {
    const modalEl = document.getElementById('formularioVisitante');

    const modal = new bootstrap.Modal(modalEl, {
        backdrop: false,
        keyboard: false,
        focus: true
    });

    modal.show();
}

function cancelarFormulario() {
    const form = document.getElementById("formVisitante");
    if (form && form.reset) {
        form.reset();
    }
    document.getElementById("formularioVisitante").style.display = "none";
}

async function exibirNomeMorador() {
    try {
        const res = await fetch('/Visitante/ObterNomeMorador', {
            method: 'GET',
            credentials: 'include'
        });

        if (!res.ok) throw new Error("Erro ao buscar nome.");

        const data = await res.json();
        document.getElementById("boasVindas").innerHTML = `
                    <i class="fas fa-user-circle me-2"></i>Bem-vindo, <strong>${data.usuario}</strong>!
                `;
    } catch (err) {
        console.error("Erro ao exibir nome do morador:", err);
    }
}

function mostrarImagemVisitante(idVisitante) {
    const idFormatado = idVisitante.toString().padStart(10, '0');
    const imagens = [
        { id: "previewFotoFrente", prefixo: "VF" },
        { id: "previewFotoVerso", prefixo: "VV" },
        { id: "previewFotoVisitante", prefixo: "V" }
    ];

    imagens.forEach(({ id, prefixo }) => {
        const url = `/imagens/${prefixo}${idFormatado}.jpg`;
        const img = document.getElementById(id);

        const testeImg = new Image();
        testeImg.onload = () => {
            img.src = url;
            img.style.display = "block";
        };
        testeImg.onerror = () => {
            img.style.display = "none";
        };
        testeImg.src = url;
    });
}


async function editarVisitante(idVisitante) {
    try {
        const response = await fetch(`/Visitante/ObterVisitantePorId/${idVisitante}`);
        if (!response.ok) {
            throw new Error('Erro ao carregar dados do visitante');
        }

        const dadosVisitante = await response.json();

        document.getElementById('editIdVisitante').value = dadosVisitante.idVisitante;
        document.getElementById('editVisitante').value = dadosVisitante.visitante || '';
        document.getElementById('editEmpresa').value = dadosVisitante.empresa || '';
        document.getElementById('editFoneCelular').value = dadosVisitante.foneCelular || '';
        document.getElementById('editEmail').value = dadosVisitante.email || '';
        document.getElementById("editEndereco").value = dadosVisitante.endereco || '';
        document.getElementById("editNumero").value = dadosVisitante.numero || '';
        document.getElementById("editBairro").value = dadosVisitante.bairro || '';
        document.getElementById("editCidade").value = dadosVisitante.cidade || '';
        document.getElementById("editEstado").value = dadosVisitante.estado || '';
        document.getElementById("editCep").value = dadosVisitante.cep || '';
        document.getElementById("editTpDocto").value = dadosVisitante.tpDocto || '';
        document.getElementById("editNumDocto").value = dadosVisitante.numDocto || '';
        document.getElementById('editTemVeiculo').checked = dadosVisitante.temCarro != 0;
        document.getElementById("editPlaca").value = dadosVisitante.placa || '';
        document.getElementById("editVeiculo").value = dadosVisitante.veiculo || '';
        document.getElementById("editMarca").value = dadosVisitante.marca || '';
        document.getElementById("editModelo").value = dadosVisitante.modelo || '';
        document.getElementById("editCor").value = dadosVisitante.cor || '';
        mostrarImagemVisitante(idVisitante);
        const grupoVeiculo = document.getElementById("editGrupoVeiculo");
        grupoVeiculo.style.display = dadosVisitante.temCarro != 0 ? "flex" : "none";

        if (dadosVisitante.dtNascimento) {
            const dataNascimento = new Date(dadosVisitante.dtNascimento);
            if (!isNaN(dataNascimento.getTime())) {
                const formattedDate = dataNascimento.toISOString().split('T')[0];
                document.getElementById('editDtNascimento').value = formattedDate;
            }
        }

        const modal = new bootstrap.Modal(document.getElementById('editarVisitanteModal'));
        modal.show();
    } catch (error) {
        console.error('Erro ao editar visitante:', error);
        showNotification('Erro ao carregar dados do visitante', 'error');
    }
}

async function salvarEdicao() {
    const salvarBtn = document.querySelector('#editarVisitanteModal .btn-primary');
    setButtonLoading(salvarBtn, true);

    try {
        const idVisitante = document.getElementById('editIdVisitante').value;

        const dtNascimentoInput = document.getElementById("editDtNascimento").value;
        if (dtNascimentoInput) {
            const dtNascimento = new Date(dtNascimentoInput);
            const hoje = new Date();

            dtNascimento.setHours(0, 0, 0, 0);
            hoje.setHours(0, 0, 0, 0);

            if (dtNascimento > hoje) {
                showNotification('A data de nascimento não pode ser maior que a data atual.', 'error');
                return;
            }
        }
        const dtNascimentoFormatada = dtNascimentoInput ? new Date(dtNascimentoInput).toISOString() : null;

        const dadosAtualizados = {
            idVisitante: parseInt(idVisitante),
            Visitante: document.getElementById("editVisitante").value,
            Empresa: document.getElementById("editEmpresa").value,
            FoneCelular: telefone,
            email: document.getElementById("editEmail").value,
            Endereco: document.getElementById("editEndereco").value,
            Numero: document.getElementById("editNumero").value,
            Bairro: document.getElementById("editBairro").value,
            Cidade: document.getElementById("editCidade").value,
            Estado: document.getElementById("editEstado").value,
            CEP: document.getElementById("editCep").value,
            TpDocto: document.getElementById("editTpDocto").value,
            NumDocto: document.getElementById("editNumDocto").value,
            DtNascimento: dtNascimentoFormatada,
            Placa: document.getElementById("editPlaca").value,
            Veiculo: document.getElementById("editVeiculo").value,
            Marca: document.getElementById("editMarca").value,
            Modelo: document.getElementById("editModelo").value,
            Cor: document.getElementById("editCor").value
        };

        if (telefone.length < 11) {
            showNotification('Por favor, insira um número de celular válido (DDD + número)', 'error');
            return;
        }

        const response = await fetch('/Visitante/AtualizarVisitante', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dadosAtualizados)
        });

        if (!response.ok) {
            const errorDetails = await response.text();
            console.error("Detalhes do erro:", errorDetails);
            throw new Error(`Erro ${response.status}: ${errorDetails}`);
        }

        showNotification('Visitante atualizado com sucesso!', 'success');

        const modal = bootstrap.Modal.getInstance(document.getElementById('editarVisitanteModal'));
        modal.hide();

        carregarVisitantes();
    } catch (error) {
        console.error('Erro completo:', error);
        showNotification(error.message || 'Erro ao atualizar visitante', 'error');
    } finally {
        setButtonLoading(salvarBtn, false);
    }
}

async function carregarVisitantesNaTabela() {
    const tabela = document.getElementById("tabelaVisitantesFesta");
    tabela.innerHTML = `
                <tr>
                    <td colspan="4" class="text-center py-3">
                        <div class="loading-spinner" style="width: 1.5rem; height: 1.5rem;"></div>
                        <p class="mt-2">Carregando visitantes...</p>
                    </td>
                </tr>
            `;

    try {
        const response = await fetch("/Visitante/ObterVisitantes");
        const visitantes = await response.json();

        tabela.innerHTML = "";
        visitantes.forEach(v => {
            const linha = `
                        <tr>
                            <td><input type="checkbox" class="form-check-input" value="${v.idVisitante}" name="visitanteSelecionado"></td>
                            <td>${v.nomeFesta}</td>
                            <td>${v.celularVisitante}</td>
                            <td>${v.emailVisitante}</td>
                        </tr>
                    `;
            tabela.innerHTML += linha;
        });

    } catch (erro) {
        console.error("Erro ao carregar visitantes:", erro);
        tabela.innerHTML = `<tr><td colspan="4" class="text-danger">Erro ao carregar visitantes.</td></tr>`;
    }
}

function carregarCadastroFesta() {
    const modalElement = document.getElementById("adicionarFesta");
    if (!modalElement) {
        console.error("Modal 'adicionarFesta' não encontrado no DOM.");
        return;
    }
    const modal = new bootstrap.Modal(modalElement);
    modal.show();
    carregarVisitantesNaTabela();
}

function toBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });
}

function getBase64FromImageSrc(src) {
    return fetch(src)
        .then(res => res.blob())
        .then(blob => new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => {
                const base64 = reader.result.split(',')[1];
                resolve(base64);
            };
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        }));
}

async function salvarFesta() {
    const salvarBtn = document.querySelector('#adicionarFesta .btn-success');
    setButtonLoading(salvarBtn, true);

    try {
        const nomeFesta = document.getElementById('nomeFesta').value.trim();
        let conviteBase64 = null;

        const dataInicio = document.getElementById('dataInicio').value;
        const horaInicio = document.getElementById('horaInicio').value;
        const diaTodoInicio = document.getElementById('diaTodoInicio').checked;

        const dataFim = document.getElementById('dataFim').value;
        const horaFim = document.getElementById('horaFim').value;
        const diaTodoFim = document.getElementById('diaTodoFim').checked;

        if (!nomeFesta || !dataInicio || (!diaTodoInicio && !horaInicio) || !dataFim || (!diaTodoFim && !horaFim) || (!document.getElementById('viaZap').checked && !document.getElementById('viaEmail').checked)) {
            showNotification('Preencha os campos necessários', 'error');
            return;
        }

        const de = new Date(dataInicio);
        const ate = new Date(dataFim);
        const hoje = new Date();

        de.setHours(0, 0, 0, 0);
        ate.setHours(0, 0, 0, 0);
        hoje.setHours(0, 0, 0, 0);

        const hojeReal = (hoje - 1) / (1000 * 60 * 60 * 24);

        if (de < hojeReal || ate < hojeReal || ate < de) {
            showNotification('Selecione uma data válida.', 'error');
            return;
        }

        const diffEmDias = (ate - de) / (1000 * 60 * 60 * 24);

        if (diffEmDias > 30) {
            showNotification('Limite de 30 dias', 'error')
            return;
        }

        const inicio = `${dataInicio}T${diaTodoInicio ? '00:00' : horaInicio}`;
        const fim = `${dataFim}T${diaTodoFim ? '23:59' : horaFim}`;

        const checkboxes = document.querySelectorAll('#tabelaVisitantesFesta input[type="checkbox"]:checked');
        const visitantes = Array.from(checkboxes).map(cb => parseInt(cb.value));

        const fileInput = document.getElementById('fotoConvite');
        const file = fileInput?.files?.[0];

        if (file) {
            conviteBase64 = await toBase64(file);
        }

        const viaZap = document.getElementById('viaZap').checked;
        const viaEmail = document.getElementById('viaEmail').checked;

        let metodo = 0;
        if (viaZap) metodo += 1;
        if (viaEmail) metodo += 2;

        const dados = {
            nomeFesta: nomeFesta,
            de: inicio,
            ate: fim,
            visitantes: visitantes,
            fotoConvite: conviteBase64,
            metodo: metodo,
            diaTodoDe: document.getElementById('diaTodoInicio').checked ? 1 : 0,
            diaTodoAte: document.getElementById('diaTodoFim').checked ? 1 : 0,
            horaDe: document.getElementById('horaInicio').value,
            horaAte: document.getElementById('horaFim').value
        };

        const response = await fetch('/Visitante/CadastrarFesta', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dados)
        });

        if (response.ok) {
            showNotification('Festa cadastrada com sucesso', 'success');
            bootstrap.Modal.getInstance(document.getElementById('adicionarFesta')).hide();
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalFestas'));
            if (modal) modal.hide();
            carregarFestas();
        } else {
            const error = await response.text();
            showNotification(error.message || 'Erro ao cadastrar festa', 'error');
        }
    } catch (error) {
        console.error("Erro ao enviar:", error);
        showNotification(error.message || 'Erro inesperado ao enviar dados', 'error');
    } finally {
        setButtonLoading(salvarBtn, false);
    }
}

async function carregarVisitantesEditNaTabela(idFesta) {
    const tabela = document.getElementById("editTabelaVisitantesFesta");
    tabela.innerHTML = `
                <tr>
                    <td colspan="4" class="text-center py-3">
                        <div class="loading-spinner" style="width: 1.5rem; height: 1.5rem;"></div>
                        <p class="mt-2">Carregando visitantes...</p>
                    </td>
                </tr>
            `;

    try {
        // Buscar visitantes já cadastrados na festa
        const responseMarcados = await fetch(`/Visitante/ObterVisitantesPorFesta/${idFesta}`);
        if (!responseMarcados.ok) throw new Error("Erro ao buscar visitantes da festa");
        const visitantesMarcados = await responseMarcados.json();
        const idsMarcados = visitantesMarcados.map(v => v.idVisitante);

        // Buscar todos os visitantes disponíveis
        const responseTodos = await fetch("/Visitante/ObterVisitantes");
        if (!responseTodos.ok) throw new Error("Erro ao buscar todos os visitantes");
        const todosVisitantes = await responseTodos.json();

        // Preencher a tabela
        tabela.innerHTML = "";
        todosVisitantes.forEach(v => {
            const checked = idsMarcados.includes(v.idVisitante) ? "checked" : "";
            const linha = `
                        <tr>
                            <td><input type="checkbox" class="form-check-input" value="${v.idVisitante}" name="visitanteSelecionado" ${checked}></td>
                            <td>${v.visitante}</td>
                            <td>${v.numDocto}</td>
                            <td>${v.foneCelular}</td>
                        </tr>
                    `;
            tabela.innerHTML += linha;
        });

    } catch (erro) {
        console.error("Erro ao carregar visitantes:", erro);
        tabela.innerHTML = `<tr><td colspan="4" class="text-danger">Erro ao carregar visitantes.</td></tr>`;
    }
}

async function editarFesta(idFesta) {
    try {
        const response = await fetch(`/Visitante/ObterFestaPorId/${idFesta}`);
        if (!response.ok) {
            throw new Error('Erro ao carregar dados do visitante');
        }

        const dadosFesta = await response.json();

        const de = dadosFesta.de.split("T")[0];
        const ate = dadosFesta.ate.split("T")[0];
        const fotoConviteBase64 = dadosFesta.fotoConvite;

        console.log("De:", de);
        console.log("Ate:", ate);

        document.getElementById('editIdFesta').value = idFesta;
        document.getElementById('editNomeFesta').value = dadosFesta.nomeFesta || '';
        document.getElementById('editViaZap').checked = dadosFesta.metodo != 2;
        document.getElementById('editViaEmail').checked = dadosFesta.metodo != 1;
        document.getElementById('editDataInicio').value = de || '';
        document.getElementById('editHoraInicio').value = dadosFesta.horaDe || '';
        document.getElementById('editDiaTodoInicio').checked = dadosFesta.diaTodoDe != 0;
        document.getElementById('editDataFim').value = ate || '';
        document.getElementById('editHoraFim').value = dadosFesta.horaAte || '';
        document.getElementById('editDiaTodoFim').checked = dadosFesta.diaTodoAte != 0;

        const imgPreview = document.getElementById('previewFotoConvite');

        if (fotoConviteBase64) {
            imgPreview.src = `data:image/jpeg;base64,${fotoConviteBase64}`;
            imgPreview.style.display = 'block';
        } else {
            imgPreview.src = '';
            imgPreview.style.display = 'none';
        }

        await carregarVisitantesEditNaTabela(dadosFesta.id);

        const modal = new bootstrap.Modal(document.getElementById('atualizarFesta'));
        modal.show();

    } catch (error) {
        console.error('Erro ao editar festa:', error);
        showNotification('Erro ao carregar dados da festa', 'error');
    }
}

async function salvarEdicaoFesta() {
    const salvarBtn = document.querySelector('#atualizarFesta .btn-primary');
    setButtonLoading(salvarBtn, true);

    try {
        const idFesta = document.getElementById("editIdFesta").value;
        const nomeFesta = document.getElementById('editNomeFesta').value.trim();
        let conviteBase64 = null;

        const dataInicio = document.getElementById('editDataInicio').value;
        const horaInicio = document.getElementById('editHoraInicio').value;
        const diaTodoInicio = document.getElementById('editDiaTodoInicio').checked;

        const viaZap = document.getElementById('editViaZap').checked;
        const viaEmail = document.getElementById('editViaEmail').checked;

        const dataFim = document.getElementById('editDataFim').value;
        const horaFim = document.getElementById('editHoraFim').value;
        const diaTodoFim = document.getElementById('editDiaTodoFim').checked;

        if (!nomeFesta || !dataInicio || (!diaTodoInicio && !horaInicio) || !dataFim || (!diaTodoFim && !horaFim) || (!viaZap && !viaEmail)) {
            showNotification('Preencha os campos necessários', 'error');
            return;
        }

        const inicio = `${dataInicio}T${diaTodoInicio ? '00:00' : horaInicio}`;
        const fim = `${dataFim}T${diaTodoFim ? '23:59' : horaFim}`;

        const checkboxes = document.querySelectorAll('#editTabelaVisitantesFesta input[type="checkbox"]:checked');
        const visitantes = Array.from(checkboxes).map(cb => parseInt(cb.value));

        const fileInput = document.getElementById('editFotoConvite');
        const file = fileInput?.files?.[0];

        if (file) {
            conviteBase64 = await toBase64(file);
        } else {
            const imgPreview = document.getElementById('previewFotoConvite');
            if (imgPreview && imgPreview.style.display !== 'none' && imgPreview.src.startsWith('data:image')) {
                conviteBase64 = imgPreview.src.split(',')[1]; // extrai só o base64
            }
        }

        let metodo = 0;
        if (viaZap) metodo += 1;
        if (viaEmail) metodo += 2;

        const dadosAtualizados = {
            id: idFesta,
            nomeFesta: nomeFesta,
            de: inicio,
            ate: fim,
            visitantes: visitantes,
            metodo: metodo,
            diaTodoDe: diaTodoInicio ? 1 : 0,
            diaTodoAte: diaTodoFim ? 1 : 0,
            horaDe: horaInicio,
            horaAte: horaFim
        };

        // Só adiciona a imagem se houver
        if (conviteBase64) {
            dadosAtualizados.fotoConvite = conviteBase64;
        }

        const response = await fetch('/Visitante/AtualizarFesta', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dadosAtualizados)
        });

        if (!response.ok) {
            const errorDetails = await response.text();
            console.error("Detalhes do erro:", errorDetails);
            throw new Error(`Erro ${response.status}: ${errorDetails}`);
        }

        showNotification('Festa atualizada com sucesso!', 'success');

        bootstrap.Modal.getInstance(document.getElementById('atualizarFesta')).hide();
        const modal = bootstrap.Modal.getInstance(document.getElementById('modalFestas'));
        if (modal) modal.hide();
        carregarFestas();

    } catch (error) {
        console.error('Erro completo:', error);
        showNotification(error.message || 'Erro ao atualizar festa', 'error');
    } finally {
        setButtonLoading(salvarBtn, false);
    }
}
async function obterIpPublico() {
    const response = await fetch("https://api.ipify.org?format=json");
    const data = await response.json();
    return data.ip;
}