// Controle das telas
function showScreen(screenId) {
    document.querySelectorAll('.screen').forEach(screen => {
        screen.classList.remove('active', 'left');
    });

    const screen = document.getElementById(screenId);
    screen.classList.add('active');

    if (event) {
        const currentScreen = event.currentTarget.closest('.screen');
        currentScreen.classList.add('left');
    }
}

async function fazerLogin() {
    const usuario = document.getElementById("usuario").value;
    const senha = document.getElementById("senha").value;
    const errorDiv = document.getElementById("error");
    const loadingDiv = document.getElementById("loading");
    const ip = await obterIpPublico();

    errorDiv.textContent = "";

    if (!usuario || !senha) {
        errorDiv.textContent = "Por favor, preencha todos os campos.";
        return;
    }

    loadingDiv.classList.add("show");

    try {
        const resposta = await fetch("/LoginApi/Login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: 'include',
            body: JSON.stringify({ usuario, senha, ip })
        });

        if (resposta.ok) {
            window.location.href = "/Home/Index";
        } else {
            const dadosErro = await resposta.json();
            errorDiv.textContent = dadosErro.erro || "Usuário ou senha inválidos";
        }
    } catch (erro) {
        errorDiv.textContent = "Erro ao conectar com o servidor. Tente novamente.";
        console.error("Erro no login:", erro);
    } finally {
        loadingDiv.classList.remove("show");
    }
}

async function cadastrar() {
    const ip = await obterIpPublico();
    const usuario = document.getElementById("cadUsuario").value;
    const email = document.getElementById("cadEmail").value;
    const senha = document.getElementById("cadSenha").value;

    if (!usuario || !senha || !email) {
        showNotification('Por favor preencha todos os campos', 'error');
        return;
    }

    try {
        const resposta = await fetch("/LoginApi/Cadastrar", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: 'include',
            body: JSON.stringify({ usuario, senha, email, ip })
        });

        if (resposta.ok) {
            showNotification('Solicitação de acesso enviada com sucesso!', 'success');
            showScreen('login-screen');
        } else {
            const dadosErro = await resposta.json();
            showNotification(dadosErro.erro || 'Erro ao solicitar acesso', 'error');
        }
    } catch (erro) {
        showNotification("Erro ao conectar com o servidor. Tente novamente.", 'error');
        console.error("Erro no cadastro:", erro);
    }
}

async function sendRecoveryCode() {
    const Email = document.getElementById('recover-email').value;
    if (!Email) {
        showNotification('Por favor, digite seu e-mail', 'error');
        return;
    }
    try {
        const Ip = await obterIpPublico();

        const res = await fetch(`/LoginApi/EnviarToken`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify({ Email, Ip })

        });

        if (res.ok) {
            document.getElementById('email-display').textContent = Email;
            showScreen('recover-step2');
            showNotification('Código enviado para seu e-mail!', 'success');
        } else {
            const dadosErro = await res.json();
            showNotification(dadosErro.erro || 'Erro ao enviar código', 'error');
        }
    } catch (erro) {
        showNotification('Erro ao enviar código: ' + erro, 'error');
    }
}

async function verifyRecoveryCode() {
    const code = document.getElementById('recovery-code').value;
    if (!code) {
        showNotification('Por favor, digite o código recebido', 'error');
        return;
    }
    try {
        const res = await fetch(`/LoginApi/VerificaToken/${code}`, {
            method: 'POST',
            credentials: 'include'
        });

        if (res.ok) {
            showScreen('recover-step3');
            showNotification('Código correto!', 'success');
        } else {
            const dadosErro = await res.json();
            showNotification(dadosErro.erro || 'Erro ao verificar código', 'error');
        }
    } catch (erro) {
        showNotification('Erro ao verificar código: ' + erro, 'error');
    }
}

async function updatePassword() {
    const newPassword = document.getElementById('new-password').value;
    const confirmPassword = document.getElementById('confirm-password').value;

    if (!newPassword || !confirmPassword) {
        showNotification('Por favor, preencha ambos os campos', 'error');
        return;
    }

    if (newPassword !== confirmPassword) {
        showNotification('As senhas não coincidem', 'error');
        return;
    }

    try {
        const res = await fetch(`/LoginApi/UpdateSenha/${newPassword}`, {
            method: 'PUT',
            credentials: 'include'
        });

        if (res.ok) {
            showScreen('login-screen');
            showNotification('Senha alterada com sucesso!', 'success');
        } else {
            const dadosErro = await res.json();
            showNotification(dadosErro.erro || 'Erro ao alterar senha', 'error');
        }

    } catch (erro) {
        showNotification('Erro ao alterar senha: ' + erro, 'error');
    }
}

async function obterIpPublico() {
    try {
        const response = await fetch("https://api.ipify.org?format=json");
        const data = await response.json();
        return data.ip;
    } catch (error) {
        console.error("Erro ao obter IP:", error);
        return "unknown";
    }
}

function showNotification(message, type) {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.innerHTML = `
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
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

document.addEventListener('keypress', function (e) {
    if (e.key === 'Enter') {
        if (document.getElementById('login-screen').classList.contains('active')) {
            fazerLogin();
        } else if (document.getElementById('register-screen').classList.contains('active')) {
            cadastrar();
        } else if (document.getElementById('recover-step1').classList.contains('active')) {
            sendRecoveryCode();
        } else if (document.getElementById('recover-step2').classList.contains('active')) {
            verifyRecoveryCode();
        } else if (document.getElementById('recover-step3').classList.contains('active')) {
            updatePassword();
        }
    }
});