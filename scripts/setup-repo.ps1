param(
    [string]$RepoUrl = "https://github.com/msrovani/ehaqui.git",
    [string]$Branch = "main"
)

$ErrorActionPreference = "Stop"
$Root = Resolve-Path (Join-Path $PSScriptRoot "..")

Write-Host "=== Configurando repositório EHAQUI / ISHERE ===" -ForegroundColor Cyan
Write-Host ""

# Verificar git
$git = Get-Command git -ErrorAction SilentlyContinue
if (-not $git) {
    Write-Host "Git não encontrado. Instale o Git for Windows em:" -ForegroundColor Yellow
    Write-Host "  https://git-scm.com/download/win" -ForegroundColor Yellow
    exit 1
}

# Inicializar git
Set-Location -LiteralPath $Root
git init
if ($LASTEXITCODE -ne 0) { throw "git init falhou" }

# Configurar user
$name = git config user.name
if (-not $name) {
    Write-Host "Configurando identidade git (global)..." -ForegroundColor Yellow
    git config --global user.name "Seu Nome"
    git config --global user.email "seu@email.com"
    Write-Host "ATENÇÃO: edite user.name e user.email no script ou execute:" -ForegroundColor Yellow
    Write-Host "  git config user.name 'Seu Nome'" -ForegroundColor Yellow
    Write-Host "  git config user.email 'seu@email.com'" -ForegroundColor Yellow
}

# Adicionar tudo
git add .
if ($LASTEXITCODE -ne 0) { throw "git add falhou" }

# Commit
git commit -m "feat: estrutura inicial do projeto EHAQUI / ISHERE

- i18n completo pt-BR (EHAQUI) e en (ISHERE)
- Documentos de design, arquitetura e roadmap
- CI/CD via GitHub Actions
- Estrutura de diretórios para engine (Unity/Godot)
- Scripts de setup e desenvolvimento"
if ($LASTEXITCODE -ne 0) { throw "git commit falhou" }

# Remote
git remote add origin $RepoUrl
Write-Host ""
Write-Host "Remote 'origin' adicionado: $RepoUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para enviar ao GitHub:" -ForegroundColor Green
Write-Host "  git push -u origin $Branch" -ForegroundColor Green
Write-Host ""
Write-Host "Se precisar forçar (primeiro push):" -ForegroundColor Yellow
Write-Host "  git push -u origin $Branch --force" -ForegroundColor Yellow
