# Protocolo de Recepção de Protocolos - README

Este projeto é um sistema de recepção de protocolos para emissão de documentos, utilizando RabbitMQ, uma API de consulta e armazenamento em banco de dados. A aplicação está estruturada em três componentes principais:

1. **ProtocolPublisher**: Um projeto console para envio de protocolos (JSON) para uma fila no RabbitMQ.
2. **ProtocolConsumer**: Um projeto que consome os protocolos da fila, valida os dados recebidos, gera logs de dados inválidos e armazena os protocolos válidos no banco de dados.
3. **ProtocolAPI**: Uma API que permite consultar os protocolos por número, CPF, RG e listagem paginada de todos os protocolos.

## Tecnologias Utilizadas

- **.NET 6**
- **RabbitMQ** (com Docker)
- **SQLite** (como banco de dados)
- **XUnit** (para testes unitários)
- **Docker** (para deploy e execução)

## Pré-requisitos

Antes de rodar a aplicação localmente, certifique-se de ter os seguintes componentes instalados:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

## Clonando o Repositório

Para clonar o repositório, execute o seguinte comando:

```bash
git clone https://github.com/halls510/ProtocolReceptionSolution.git
cd ProtocolReceptionSolution
```

## Configuração do Ambiente

Para facilitar o processo de configuração das variáveis de ambiente exigidas pela aplicação, utilizamos um arquivo `.env` na raiz do projeto. Este arquivo contém as definições das variáveis necessárias para a aplicação funcionar corretamente, como as credenciais do RabbitMQ e a chave secreta do JWT.

### Estrutura do Arquivo `.env`

O arquivo `.env` deve ser colocado na raiz da solução e ter o seguinte conteúdo:

```bash
RABBITMQ_HOSTNAME=localhost
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest
QUEUE_NAME=protocolo_queue
DB_NAME=database_name
JWT_SECRET_KEY=sua_chave_secreta
```

### Instruções para Configurar e Utilizar

1. **Criar o arquivo `.env`**: Na raiz da solução, crie um arquivo chamado `.env` e copie o conteúdo acima.
   
2. **Configuração automática das variáveis de ambiente**: A aplicação está configurada para carregar automaticamente as variáveis definidas no arquivo `.env`, simplificando o processo de setup.

3. **Utilização no Docker Compose**: O arquivo `.env` também é utilizado pelo Docker Compose para definir as variáveis de ambiente dentro dos contêineres. O Docker Compose lerá automaticamente o arquivo quando for executado.

4. **Certifique-se de que o arquivo `.env` não seja commitado**: O arquivo `.env` está incluído no `.gitignore` para garantir que ele não seja enviado ao controle de versão, protegendo as informações sensíveis como credenciais e chaves.


### Configurando o Docker

A aplicação utiliza o **RabbitMQ** e o banco de dados SQLite. Para facilitar a execução local, utilizamos Docker e Docker Compose. Para subir os serviços, execute o comando:

```bash
docker-compose up --build
```

Isso criará os containers para o RabbitMQ e rodará o **ProtocolPublisher**, **ProtocolConsumer** e a **ProtocolAPI**.

### Executando a Aplicação

1. **Subindo RabbitMQ e API**: Utilize o Docker Compose para iniciar os containers:

```bash
docker-compose up
```

2. **Publisher**: O projeto `ProtocolPublisher` irá mockar aproximadamente 100 protocolos e enviá-los para a fila do RabbitMQ.

3. **Consumer**: O projeto `ProtocolConsumer` consumirá os protocolos da fila, fará a validação dos dados e os armazenará no banco de dados SQLite.

4. **API de Consulta**: A API estará disponível para consultas dos protocolos através de diferentes endpoints protegidos por autenticação JWT.

## Endpoints da API

A API disponibiliza os seguintes endpoints para consulta:

### Autenticação

- **POST /api/Auth/login**  
  Endpoint de login para obter o token JWT.  
  **Exemplo de body**:
  ```json
  {
    "username": "admin",
    "password": "senha123"
  }
  ```
  **Resposta**:
  ```json
  {
    "token": "jwt_token_aqui"
  }
  ```

### Consultar Protocolo por Número

- **GET /api/Protocolos/numero/{numeroProtocolo}**  
  Consulta protocolo por número.  
  **Exemplo**:
  ```
  GET /api/Protocolos/numero/00001
  ```
  **Resposta**:
  ```json
  {
    "numeroProtocolo": "00001",
    "cpf": "12345678901",
    "rg": "SP100001",
    "nome": "João Silva",
    ...
  }
  ```

### Consultar Protocolo por CPF

- **GET /api/Protocolos/cpf/{cpf}**  
  Consulta protocolos por CPF.  
  **Exemplo**:
  ```
  GET /api/Protocolos/cpf/12345678901
  ```
  **Resposta**:
  ```json
  [
    {
      "numeroProtocolo": "00001",
      "cpf": "12345678901",
      "rg": "SP100001",
      "nome": "João Silva",
      ...
    },
    {
      "numeroProtocolo": "00002",
      "cpf": "12345678901",
      "rg": "SP100002",
      "nome": "Maria Silva",
      ...
    }
  ]
  ```

### Consultar Protocolo por RG

- **GET /api/Protocolos/rg/{rg}**  
  Consulta protocolos por RG.  
  **Exemplo**:
  ```
  GET /api/Protocolos/rg/SP100001
  ```
  **Resposta**:
  ```json
  [
    {
      "numeroProtocolo": "00001",
      "cpf": "12345678901",
      "rg": "SP100001",
      "nome": "João Silva",
      ...
    }
  ]
  ```

### Consultar Todos os Protocolos (com paginação)

- **GET /api/Protocolos/paginados?page=1&pageSize=10**  
  Consulta todos os protocolos com paginação.  
  **Exemplo**:
  ```
  GET /api/Protocolos/paginados?page=1&pageSize=10
  ```
  **Resposta**:
  ```json
  {
    "paginaAtual": 1,
    "tamanhoPagina": 10,
    "totalItens": 100,
    "protocolos": [
      {
        "numeroProtocolo": "00001",
        "cpf": "12345678901",
        "rg": "SP100001",
        "nome": "João Silva",
        ...
      },
      ...
    ]
  }
  ```

## Logs da Aplicação

A aplicação gera logs para:

- **Sucesso no armazenamento de protocolos válidos**
- **Falha de validação com detalhes do erro**
- **Erros de conexão com RabbitMQ (com tentativas de reconexão)**

Os logs são armazenados em arquivos gerados no diretório `/logs` de cada container.

---

## Execução de Testes Unitários (XUnit)

O projeto inclui testes unitários usando o framework **XUnit** para garantir a qualidade e cobertura do código. Os testes estão localizados no projeto `ProtocolAPI.Tests`.

### Pré-requisitos

- Certifique-se de que o **.NET SDK** esteja instalado na máquina de execução.
- O projeto de testes deve estar restaurado corretamente com todas as dependências.

### Executando os Testes via Terminal

Para executar os testes unitários do projeto `ProtocolAPI.Tests` via terminal, siga os passos abaixo:

1. **Abrir o terminal**: Navegue até a raiz do projeto `ProtocolAPI.Tests`.

2. **Restaurar as dependências**:
   
   Antes de executar os testes, é importante restaurar as dependências do projeto. Execute o seguinte comando no terminal:

   ```bash
   dotnet restore
   ```

3. **Executar os testes**:

   Para rodar os testes unitários, utilize o seguinte comando:

   ```bash
   dotnet test ProtocolAPI.Tests.csproj
   ```

   Esse comando irá compilar o projeto de testes, executar todos os testes e mostrar os resultados no terminal.

### Demo

- A API está disponível em: [https://api.hallison.com.br/swagger/index.html](https://api.hallison.com.br/swagger/index.html)
- A interface gráfica do RabbitMQ está disponível em: [https://rabbitmq.hallison.com.br](https://rabbitmq.hallison.com.br)

### Credenciais da API e RabbitMQ

- **Usuário da API**: `admin`
- **Senha da API**: `senha123`

- **Usuário RabbitMQ**: `rabbitmq`
- **Senha RabbitMQ**: `G5Hs4asd4Hhk63nNdfhkdfdAsf2fhghj45fns`

## Scripts de Banco de Dados

Os scripts de banco de dados para a criação das tabelas estão disponíveis no diretório `sqlscripts/`. Eles podem ser utilizados para recriar o banco de dados manualmente, se necessário.

---

© 2024 Protocolo de Recepção de Protocolos