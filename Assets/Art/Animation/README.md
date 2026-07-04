# 🎞️ Art / Animation — Ctrl+Alt+Delas

Controllers e clipes de animação do fantasminha.

---

## O que fica aqui

| Arquivo | Descrição |
|---------|-----------|
| `Player.controller` | Animator Controller do personagem (já existe) |
| `idle.anim` | Clipe de animação parado (já existe) |
| `walking.anim` | Clipe de animação andando (já existe) |
| Novos clipes | Adicione aqui se criar animações extras |

---

## 🔗 Como o Animator funciona no projeto

O `PlayerController.cs` controla **um único parâmetro**:

| Parâmetro | Tipo | Quando é `true` |
|-----------|------|-----------------|
| `IsMoving` | Bool | Personagem se movendo horizontalmente |

O Animator Controller (`Player.controller`) usa esse parâmetro para alternar entre os estados:

```
idle.anim  ←──────────────────────  IsMoving = false
                                              ↕
walking.anim  ←───────────────────  IsMoving = true
```

---

## ➕ Como criar uma nova animação

1. Selecione o objeto **Player** na Hierarchy.
2. Abra `Window > Animation > Animation`.
3. Clique em **Create** e salve o novo clipe nesta pasta.
4. No **Animator** (`Window > Animation > Animator`), arraste o clipe para criar um novo estado.
5. Crie uma **Transition** com a condição desejada.

---

## ⚠️ Atenção

- O parâmetro `IsMoving` deve ter **exatamente esse nome** (case-sensitive) — o script usa `Animator.StringToHash("IsMoving")`.
- Não renomeie `Player.controller` sem atualizar a referência no componente **Animator** do Player.
- Novos parâmetros do Animator precisam ser lidos/escritos via script — peça ao Kiro para adicionar se necessário.
