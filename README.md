# Protocolo de Recep��o de Protocolos - README

Este projeto � um sistema de recep��o de protocolos para emiss�o de documentos, utilizando RabbitMQ, uma API de consulta e armazenamento em banco de dados. A aplica��o est� estruturada em tr�s componentes principais:

1. **ProtocolPublisher**: Um projeto console para envio de protocolos (JSON) para uma fila no RabbitMQ.
2. **ProtocolConsumer**: Um projeto que consome os protocolos da fila, valida os dados recebidos, gera logs de dados inv�lidos e armazena os protocolos v�lidos no banco de dados.
3. **ProtocolAPI**: Uma API que permite consultar os protocolos por n�mero, CPF, RG e listagem paginada de todos os protocolos.

## Tecnologias Utilizadas

- **.NET 6**
- **RabbitMQ** (com Docker)
- **SQLite** (como banco de dados)
- **XUnit** (para testes unit�rios)
- **Docker** (para deploy e execu��o)

## Pr�-requisitos

Antes de rodar a aplica��o localmente, certifique-se de ter os seguintes componentes instalados:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

## Clonando o Reposit�rio

Para clonar o reposit�rio, execute o seguinte comando:

```bash
git clone https://github.com/halls510/ProtocolReceptionSolution.git
cd ProtocolReceptionSolution
```

## Configura��o do Ambiente

Para facilitar o processo de configura��o das vari�veis de ambiente exigidas pela aplica��o, utilizamos um arquivo `.env` na raiz do projeto. Este arquivo cont�m as defini��es das vari�veis necess�rias para a aplica��o funcionar corretamente, como as credenciais do RabbitMQ e a chave secreta do JWT.

### Estrutura do Arquivo `.env`

O arquivo `.env` deve ser colocado na raiz da solu��o e ter o seguinte conte�do:

```bash
RABBITMQ_HOSTNAME=localhost
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest
QUEUE_NAME=protocolo_queue
DB_NAME=database_name
JWT_SECRET_KEY=sua_chave_secreta
```

### Instru��es para Configurar e Utilizar

1. **Criar o arquivo `.env`**: Na raiz da solu��o, crie um arquivo chamado `.env` e copie o conte�do acima.
   
2. **Configura��o autom�tica das vari�veis de ambiente**: A aplica��o est� configurada para carregar automaticamente as vari�veis definidas no arquivo `.env`, simplificando o processo de setup.

3. **Utiliza��o no Docker Compose**: O arquivo `.env` tamb�m � utilizado pelo Docker Compose para definir as vari�veis de ambiente dentro dos cont�ineres. O Docker Compose ler� automaticamente o arquivo quando for executado.

4. **Certifique-se de que o arquivo `.env` n�o seja commitado**: O arquivo `.env` est� inclu�do no `.gitignore` para garantir que ele n�o seja enviado ao controle de vers�o, protegendo as informa��es sens�veis como credenciais e chaves.


### Configurando o Docker

A aplica��o utiliza o **RabbitMQ** e o banco de dados SQLite. Para facilitar a execu��o local, utilizamos Docker e Docker Compose. Para subir os servi�os, execute o comando:

```bash
docker-compose up --build
```

Isso criar� os containers para o RabbitMQ e rodar� o **ProtocolPublisher**, **ProtocolConsumer** e a **ProtocolAPI**.

### Executando a Aplica��o

1. **Subindo RabbitMQ e API**: Utilize o Docker Compose para iniciar os containers:

```bash
docker-compose up
```

2. **Publisher**: O projeto `ProtocolPublisher` ir� mockar aproximadamente 100 protocolos e envi�-los para a fila do RabbitMQ.

3. **Consumer**: O projeto `ProtocolConsumer` consumir� os protocolos da fila, far� a valida��o dos dados e os armazenar� no banco de dados SQLite.

4. **API de Consulta**: A API estar� dispon�vel para consultas dos protocolos atrav�s de diferentes endpoints protegidos por autentica��o JWT.

## Endpoints da API

A API disponibiliza os seguintes endpoints para consulta:

### Autentica��o

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

### Consultar Protocolo por N�mero

- **GET /api/Protocolos/numero/{numeroProtocolo}**  
  Consulta protocolo por n�mero.  
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
    "nome": "Jo�o Silva",
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
      "nome": "Jo�o Silva",
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
      "nome": "Jo�o Silva",
      ...
    }
  ]
  ```

### Consultar Todos os Protocolos (com pagina��o)

- **GET /api/Protocolos/paginados?page=1&pageSize=10**  
  Consulta todos os protocolos com pagina��o.  
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
        "nome": "Jo�o Silva",
        ...
      },
      ...
    ]
  }
  ```

## Logs da Aplica��o

A aplica��o gera logs para:

- **Sucesso no armazenamento de protocolos v�lidos**
- **Falha de valida��o com detalhes do erro**
- **Erros de conex�o com RabbitMQ (com tentativas de reconex�o)**

Os logs s�o armazenados em arquivos gerados no diret�rio `/logs` de cada container.

---

## Execu��o de Testes Unit�rios (XUnit)

O projeto inclui testes unit�rios usando o framework **XUnit** para garantir a qualidade e cobertura do c�digo. Os testes est�o localizados no projeto `ProtocolAPI.Tests`.

### Pr�-requisitos

- Certifique-se de que o **.NET SDK** esteja instalado na m�quina de execu��o.
- O projeto de testes deve estar restaurado corretamente com todas as depend�ncias.

### Executando os Testes via Terminal

Para executar os testes unit�rios do projeto `ProtocolAPI.Tests` via terminal, siga os passos abaixo:

1. **Abrir o terminal**: Navegue at� a raiz do projeto `ProtocolAPI.Tests`.

2. **Restaurar as depend�ncias**:
   
   Antes de executar os testes, � importante restaurar as depend�ncias do projeto. Execute o seguinte comando no terminal:

   ```bash
   dotnet restore
   ```

3. **Executar os testes**:

   Para rodar os testes unit�rios, utilize o seguinte comando:

   ```bash
   dotnet test ProtocolAPI.Tests.csproj
   ```

   Esse comando ir� compilar o projeto de testes, executar todos os testes e mostrar os resultados no terminal.

### Demo

- A API est� dispon�vel em: [https://api.hallison.com.br/swagger/index.html](https://api.hallison.com.br/swagger/index.html)
- A interface gr�fica do RabbitMQ est� dispon�vel em: [https://rabbitmq.hallison.com.br](https://rabbitmq.hallison.com.br)

### Credenciais da API e RabbitMQ

- **Usu�rio da API**: `admin`
- **Senha da API**: `senha123`

- **Usu�rio RabbitMQ**: `rabbitmq`
- **Senha RabbitMQ**: `G5Hs4asd4Hhk63nNdfhkdfdAsf2fhghj45fns`

## Scripts de Banco de Dados

Os scripts de banco de dados para a cria��o das tabelas est�o dispon�veis no diret�rio `sqlscripts/`. Eles podem ser utilizados para recriar o banco de dados manualmente, se necess�rio.

---

� 2024 Protocolo de Recep��o de Protocolos