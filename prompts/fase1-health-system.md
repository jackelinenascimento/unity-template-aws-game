# Prompt — Fase 1: Sistema de Vida com Polvinas

Você é um desenvolvedor Unity C# trabalhando no projeto "Ecto-Escape", um platformer 2D com integração ESP32 via serial USB. O personagem se chama Blink (fantasma digital).

## CONTEXTO DO PROJETO

O projeto já possui estes scripts funcionando em Assets/Scripts/:
- PlayerController.cs (movimento + pulo via ESP32 ou teclado)
- ESP32SerialReader.cs (singleton, comunicação serial bidirecional)
- Collectible.cs (item genérico com trigger)
- GameManager.cs (singleton, conta coletáveis, mostra painel fim de jogo)
- LevelEnd.cs (trigger de final de fase)
- MovingPlatform.cs (plataforma que sobe/desce)

Comunicação serial existente:
- Unity → ESP32: 'I' (idle/neutro), 'T' (feliz/tesouro), 'D' (dano/assustado)
- ESP32 → Unity: "L1/L0", "R1/R0", "D1/D0" (botões), "U" (pulo)

## O QUE PRECISO QUE VOCÊ FAÇA

Implemente o sistema de vida baseado em "Polvinas" (um polvo fofo que funciona como coração/vida):

### Regras do sistema:
- O jogador COMEÇA com 2 Polvinas (2 vidas)
- Máximo configurável no Inspector (default: 5)
- Coletar uma Polvina no mapa = +1 vida (se não estiver no máximo)
- Tomar dano de obstáculo = -1 Polvina
- Se chegar a 0 Polvinas = Game Over (morte)
- Após tomar dano, 1.5s de invencibilidade com sprite piscando (alpha oscilando)

### Existem 3 tipos de coletáveis no jogo:
1. **PenDrive** — obrigatório para desbloquear o portal final
2. **DataCore** — obrigatório para desbloquear o portal final
3. **Polvina** — restaura/adiciona 1 vida (NÃO conta para desbloquear portal)

## Arquivos a CRIAR

### 1. Assets/Scripts/HealthSystem.cs
- Componente que vai no Player
- Campos: startingLives = 2, maxLives = 5
- Métodos públicos: TakeDamage(), AddLife(), GetCurrentLives()
- Evento/Action: OnLivesChanged(int currentLives), OnDeath()
- Lógica de invencibilidade: bool isInvincible, duração 1.5s, coroutine que pisca o SpriteRenderer
- Ao tomar dano: chama ESP32SerialReader.Instance?.SendDamage()
- Ao morrer: avisa GameManager.Instance?.OnPlayerDeath()

### 2. Assets/Scripts/DamageZone.cs
- Script genérico para qualquer obstáculo que causa dano
- Collider2D trigger — ao detectar "Player", chama HealthSystem.TakeDamage()
- Campo opcional: int damageAmount = 1
- Respeita a invencibilidade (se HealthSystem.IsInvincible, ignora)

### 3. Assets/Scripts/HUDManager.cs
- Singleton, referência ao Canvas de UI
- Mostra as Polvinas como ícones de coração (usa polvina.png como sprite dos corações)
- Campo: Sprite polvinaIcon (arrastar polvina.png no Inspector)
- Campo: Transform heartsContainer (parent dos ícones)
- Instancia/destroi ícones dinamicamente conforme OnLivesChanged
- Mostra também contador de coletáveis obrigatórios: "X / Y" (TextMeshPro)
- Escuta eventos do HealthSystem e GameManager para atualizar

## Arquivos a MODIFICAR

### 4. Assets/Scripts/Collectible.cs
- Adicionar enum CollectibleType { PenDrive, DataCore, Polvina }
- Adicionar campo [SerializeField] CollectibleType type no Inspector
- Propriedade pública: Type (getter)
- No OnTriggerEnter2D:
  - Se PenDrive ou DataCore: chama GameManager.Instance?.OnItemCollected(type) + SendTreasure()
  - Se Polvina: chama other.GetComponent<HealthSystem>()?.AddLife() + SendPolvina()
- Manter campos de collectEffect e collectSound existentes

### 5. Assets/Scripts/GameManager.cs
- Alterar OnItemCollected() para receber CollectibleType como parâmetro: OnItemCollected(CollectibleType type)
- Contar apenas PenDrive + DataCore para a condição de vitória (Polvina não conta)
- Adicionar método OnPlayerDeath() que mostra painel de Game Over com texto diferente da vitória
- Expor propriedade: bool AllRequiredCollected (true quando pendrives + datacores foram todos coletados)
- Contar _totalCollectibles apenas dos tipos PenDrive e DataCore (filtrar Polvina no Awake)

### 6. Assets/Scripts/ESP32SerialReader.cs
- Adicionar novo método público: SendPolvina() => WriteChar('P')
- Documentar no header: 'P' → carinha especial polvina (~ u ~)

### 7. Assets/Scripts/PlayerController.cs
- Não precisa de grandes mudanças, mas garantir que o HealthSystem está no mesmo GameObject
- Remover os métodos TriggerDamageReaction() e TriggerTreasureReaction() (agora o HealthSystem e Collectible cuidam disso diretamente)
- Manter TriggerIdleReaction() caso algo precise resetar a expressão

## Estilo de código
- Seguir o mesmo padrão já usado no projeto: headers com [Header("")], tooltips, summaries XML, regiões com comentários "// ----"
- Português nos comentários e tooltips
- Nomes de classes/métodos/variáveis em inglês
- Usar SerializeField private (nunca public fields)
- Singletons com pattern Instance já usado no projeto

## Importante
- NÃO criar arquivos .meta (Unity gera automaticamente)
- NÃO modificar MovingPlatform.cs ou LevelEnd.cs
- Manter compatibilidade com Unity 2022+ e o New Input System
- O sprite polvina.png já existe em Assets/Art/ (será referenciado no Inspector, não hardcode path)
