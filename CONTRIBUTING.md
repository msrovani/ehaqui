# Contribuindo para EHAQUI / ISHERE

## Código de Conduta

Respeito acima de tudo. Este é um projeto para crianças e famílias.

## Como contribuir

1. Fork o repositório
2. Crie uma branch: `git checkout -b feat/nome-da-feat`
3. Commit com padrão [Conventional Commits](https://www.conventionalcommits.org/):
   - `feat:` nova funcionalidade
   - `fix:` correção de bug
   - `docs:` documentação
   - `i18n:` tradução/internacionalização
   - `refactor:` refatoração
   - `test:` testes
4. Push e abra um Pull Request para `develop`

## i18n

Sempre mantenha `pt-BR.json` e `en.json` sincronizados.
Use a chave `_note` para comentários internos.

## Engine

O código do jogo fica em `src/Ehaqui/`.
A engine (Unity ou Godot) será decidida após protótipo funcional.
