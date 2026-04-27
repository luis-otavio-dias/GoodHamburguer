# Good Hamburguer API

O Good Hamburguer API é um sistema para registro e gerenciamento de pedidos de lanchonete. Desenvolvido como solução para o desafio técnico de Desenvolvedor C#, o projeto expõe uma API REST robusta que calcula automaticamente regras de negócio e descontos baseados nas combinações do cardápio.

## O que foi desenvolvido (Requisitos)

- **API REST**: Construída com .NET 10 e Minimal APIs.
- **CRUD Completo de Pedidos**: Rotas para criação, listagem, busca por ID, atualização e exclusão.
- **Endpoint de Cardápio**: Rota dedicada (/api/Cardapio) para consultar sanduíches e acompanhamentos disponíveis simultaneamente.
- **Regras de Negócio Aplicadas**:
    - Cálculo transparente de subtotal, descontos progressivos (10%, 15% ou 20%) e total final do pedido no momento da resposta
    - Validação contra itens duplicados (ex: tentativa de adicionar dois refrigerantes lança uma exceção tratada)
    - Limite máximo de itens respeitado por pedido (exatamente 1 sanduíche e até 2 acompanhamentos distintos).

## Pré-requisitos

- .NET 10.0 SDK instalado no ambiente.

## Como executar o projeto

O projeto foi configurado para ser facilmente executável em qualquer ambiente local. A base de dados utiliza o SQLite de forma embutida, eliminando a necessidade de configurar servidores SQL externos ou gerenciar containers Docker durante a avaliação.

### 1. Clonar e restaurar pacotes
```bash
git clone <seu-repositorio>
cd GoodHamburguer/GoodHamburguer
dotnet restore
```

### 2. Configurar o Banco de Dados

O projeto utiliza o Entity Framework Core. Para criar o banco de dados local (`app.db`) e popular os dados iniciais do cardápio automaticamente, execute a aplicação das migrations:

```bash
dotnet ef database update
```
**(Certifique-se de ter a ferramenta dotnet-ef instalada globalmente)**

### 3. Executar a aplicação
```bash
dotnet run
```

## Testar as requisições

Para validar os endpoints, você pode utilizar:

- O arquivo `GoodHamburguer.http` já incluído na raiz do projeto. Ele contém exemplos prontos de requisições (GET, POST, PUT, DELETE) e pode ser executado diretamente pelo Visual Studio 2022 ou pelo VS Code usando a extensão REST Client.

- A documentação interativa OpenAPI gerada automaticamente pela aplicação em ambiente de desenvolvimento (acessível no navegador conforme a porta gerada no terminal).

### Exemplos de Uso (cURL)
Testando os endpoints da API com `cURL`:

- Consultar o Cardápio (Sanduíches e Acompanhamentos)
```bash
curl -X GET http://localhost:5176/api/Cardapio -H "Accept: application/json"
```

- Criar um novo Pedido  
(Exemplo: Pedindo um X Egg (ID 2) com Refrigerante (ID 2))
```bash
curl -X POST http://localhost:5176/api/Pedido \
  -H "Content-Type: application/json" \
  -d '{
        "sanduicheId": 2,
        "acompanhamentoIds": [2]
      }'
```

- Listar todos os Pedidos
```bash
curl -X GET http://localhost:5176/api/Pedido -H "Accept: application/json"
```

- Consultar um Pedido específico por ID
```bash
curl -X GET http://localhost:5176/api/Pedido/1 -H "Accept: application/json"
```

- Atualizar um Pedido  
(Exemplo: Alterando o pedido 1 para ter apenas Batata Frita (ID 1))
```bash
curl -X PUT http://localhost:5176/api/Pedido/1 \
  -H "Content-Type: application/json" \
  -d '{
        "sanduicheId": 2,
        "acompanhamentoIds": [1]
      }'
```

- Eliminar um Pedido
```bash
curl -X DELETE http://localhost:5176/api/Pedido/1 -H "Accept: application/json"
```

## Decisões Técnicas

A arquitetura foi pensada para entregar um projeto limpo, performático e modular:
- **Minimal APIs** no lugar de Controllers: Optei pelo uso de Minimal APIs para reduzir a verbosidade tradicional do MVC, o que garantiu um código mais conciso.

- **Separação por Domínios**: Mesmo utilizando Minimal APIs, dividi o projeto em camadas lógicas (Pastas Models, DTOs, Endpoints, Data). As rotas foram isoladas em métodos de extensão (MapPedidoEndpoints, etc.), evitando que o Program.cs se tornasse um arquivo gigantesco e difícil de dar manutenção.

- **Padrão DTO** (Data Transfer Objects) via records: Utilizei records do C# para as respostas da API e requisições de criação. Essa decisão garante a imutabilidade dos dados em trânsito, separando completamente as Entidades do Banco de Dados daquilo que é exposto ao cliente final.

- **Entity Framework Core + SQLite**: Escolha focada na facilidade de desenvolvimento (zero configuração prévia exigida) e manutenção. A modelagem no AppDbContext tratou adequadamente as restrições de deleção (ex: impedir a exclusão de um sanduíche atrelado a um pedido) e a relação N:N entre Pedidos e Acompanhamentos.

## O que deixei de fora (Próximos passos)

Para garantir uma entrega focada e de alta qualidade nos requisitos obrigatórios, algumas melhorias sugeridas como diferenciais ficaram mapeadas para evolução futura do sistema:

- **Testes Automatizados (xUnit)**: Em um passo seguinte, o foco seria a criação de testes de unidade cobrindo exaustivamente o método CalcularTotal() da entidade Pedido, garantindo que futuras mudanças nas porcentagens de desconto não quebrem as regras estabelecidas.

- **Frontend em Blazor**: A prioridade foi consolidar um backend à prova de falhas com regras de negócios bem definidas. A interface visual de usuário não foi incluída nesta primeira versão.

- **Isolamento de Service Layer**: Atualmente, a lógica de montagem do Pedido via DTO (ex: validação de IDs de acompanhamentos inválidos) divide espaço no arquivo de Endpoints. Para a escalabilidade do sistema, essa orquestração seria refatorada para dentro de uma camada de Serviços.