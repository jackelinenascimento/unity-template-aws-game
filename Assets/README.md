# 📦 Assets

Tudo que compõe o jogo fica aqui. O Unity gerencia esta pasta automaticamente — **nunca mova ou renomeie arquivos fora do Unity**, ou os `.meta` ficam órfãos e o projeto quebra.

---

## 📂 Subpastas

| Pasta | Conteúdo | Quem cuida |
|-------|----------|------------|
| `Art/` | Sprites, animações, cenários, chão, decoração | 🎨 Artista · 🏞️ Cenário |
| `Scenes/` | Fases do jogo (`.unity`) | 🕹️ Unity |
| `Scripts/` | Código C# de gameplay | ⚙️ Gameplay |
| `Settings/` | Configurações de render URP | não mexa sem necessidade |
| `TextMesh Pro/` | Assets do sistema de texto — gerado automaticamente | não mexa |

---

## ⚠️ Regras de ouro

1. **Sempre importe arquivos pelo Unity** (arraste para o painel Project) — nunca cole direto no explorador de arquivos.
2. **Não delete `.meta`** — cada arquivo tem um par `.meta` que guarda o GUID. Deletar o `.meta` quebra referências na cena.
3. **Prefabs antes de duplicar** — se for usar o mesmo objeto várias vezes (item, obstáculo), salve-o como Prefab em `Art/` antes de duplicar.
4. **Commits** — ao commitar, inclua sempre o `.meta` junto com o arquivo original.
