using ProtocolPublisher.Models;
using System;

namespace ProtocolPublisher.Mocks
{
    public class MockarProtocolos
    {
        // Lista de nomes masculinos
        private static readonly List<string> NomesMasculinos = new List<string>
        {
            "Lucas", "Gabriel", "João", "Miguel", "Arthur", "Pedro", "Mateus", "Henrique", "Eduardo", "Gustavo"
        };

        // Lista de nomes femininos
        private static readonly List<string> NomesFemininos = new List<string>
        {
            "Maria", "Ana", "Isabela", "Fernanda", "Carolina", "Juliana", "Larissa", "Bruna", "Patrícia", "Gabriela"
        };

        // Lista de sobrenomes (comuns a ambos)
        private static readonly List<string> Sobrenomes = new List<string>
        {
            "Silva", "Oliveira", "Santos", "Souza", "Pereira", "Costa", "Almeida", "Ferreira", "Rodrigues", "Lima"
        };

        private static List<string> cpfsGerados = new List<string>(); // Armazena Cpfs gerados

        private static List<string> rgsGerados = new List<string>();  // Armazena RGs gerados

        private static List<string> CpfsFixos()
        {
            return new List<string>
            {
                "12345678901",
                "98765432100",
                "45678912345",
                "65432109876",
                "56789023456",
                "78901234567",
                "89012345678",
                "90123456789",
                "01234567890",
                "23456789012"
            };
        }

        private static List<string> RgsFixos()
        {
            return new List<string>
            {
                "SP100001",
                "MG100002",
                "RJ100003",
                "ES100004",
                "DF100005",
                "BA100006",
                "SC100007",
                "GO100008",
                "PR100009",
                "CE100010"
            };
        }


        public MockarProtocolos()
        {
            cpfsGerados.AddRange(CpfsFixos());
            rgsGerados.AddRange(RgsFixos());
        }

        private static readonly Random Random = new Random();

        // Método para gerar uma lista de protocolos mock
        public static List<Protocolo> GerarMockDeProtocolos()
        {
            var protocolos = new List<Protocolo>();

            // Caminho base onde as fotos reais estão armazenadas
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos");


            // Fotos válidas (formatos .jpg e .png)
            List<string> fotosValidas = new List<string>
            {
                Path.Combine(basePath, "foto1.jpg"),
                Path.Combine(basePath, "foto2.jpg"),
                Path.Combine(basePath, "foto3.jpg"),
                Path.Combine(basePath, "foto4.jpg"),
                Path.Combine(basePath, "foto5.jpg"),
                Path.Combine(basePath, "foto6.png"),
                Path.Combine(basePath, "foto7.png"),
                Path.Combine(basePath, "foto8.png"),
                Path.Combine(basePath, "foto9.png"),
                Path.Combine(basePath, "foto10.png")
            };

            // Fotos inválidas (formatos .bmp e .webp)
            List<string> fotosInvalidas = new List<string>
            {
                Path.Combine(basePath, "foto11.bmp"),
                Path.Combine(basePath, "foto12.bmp"),
                Path.Combine(basePath, "foto13.bmp"),
                Path.Combine(basePath, "foto14.bmp"),
                Path.Combine(basePath, "foto15.bmp")
            };

            // Gerar 10 protocolos válidos com cpfs fixados e rgs fixados
            for (int i = 1; i <= 10; i++)
            {
                // Gerar o nome do pai e da mãe
                string sobrenomePai, sobrenomeMae;
                string nomePai = GerarNomePai(out sobrenomePai);
                string nomeMae = GerarNomeMae(out sobrenomeMae);

                // Gerar o nome completo, herdando um sobrenome de cada
                string nomeCompleto = GerarNomeCompleto(sobrenomePai, sobrenomeMae);

                // gerar cpf
                string cpf = CpfsFixos()[i-1];

                // gerar rg
                string rg = RgsFixos()[i-1];

                var protocolo = new Protocolo
                {
                    NumeroProtocolo = i.ToString("D5"), // Números de protocolo como "00001", "00002", etc.
                    NumeroVia =  1,            // Número da via 1
                    Cpf = cpf,          // Gera CPFs com 11 dígitos
                    Rg = rg,           // Gera RG com variação de estados brasileiros
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                Console.WriteLine($"CPF gerado: {cpf}");  // Exibe o CPF gerado no console
                Console.WriteLine($"Rg gerado: {rg}");  // Exibe o Rg gerado no console

                protocolos.Add(protocolo);

                var protocolo2 = new Protocolo
                {
                    NumeroProtocolo = (105 + i).ToString("D5"), // Números de protocolo como "00001", "00002", etc.
                    NumeroVia = 2,            // Número da via 2
                    Cpf = cpf,          // Gera CPFs com 11 dígitos
                    Rg = rg,           // Gera RG com variação de estados brasileiros
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                protocolos.Add(protocolo2);


                var protocolo3 = new Protocolo
                {
                    NumeroProtocolo = (115 + i).ToString("D5"), // Números de protocolo como "00001", "00002", etc.
                    NumeroVia = 3,            // Número da via 3
                    Cpf = cpf,          // Gera CPFs com 11 dígitos
                    Rg = rg,           // Gera RG com variação de estados brasileiros
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                protocolos.Add(protocolo3);
            }        

            // Gerar 80 protocolos válidos
            for (int i = 11; i <= 90; i++)
            {
                // Gerar o nome do pai e da mãe
                string sobrenomePai, sobrenomeMae;
                string nomePai = GerarNomePai(out sobrenomePai);
                string nomeMae = GerarNomeMae(out sobrenomeMae);

                // Gerar o nome completo, herdando um sobrenome de cada
                string nomeCompleto = GerarNomeCompleto(sobrenomePai, sobrenomeMae);

                // gerar cpf
                string cpf = GerarCpfAleatorio();

                // gerar rg
                string rg = GerarRgAleatorio(i);

                var protocolo = new Protocolo
                {
                    NumeroProtocolo = i.ToString("D5"), // Números de protocolo como "00001", "00002", etc.
                    NumeroVia =  1,            // Número da via 1
                    Cpf = cpf,          // Gera CPFs com 11 dígitos
                    Rg = rg,           // Gera RG com variação de estados brasileiros
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                Console.WriteLine($"CPF gerado: {cpf}");  // Exibe o CPF gerado no console
                Console.WriteLine($"Rg gerado: {rg}");  // Exibe o Rg gerado no console

                protocolos.Add(protocolo);
            }

            // Gerar 5 protocolos inválidos para duplicidade de número de protocolo
            for (int i = 91; i <= 95; i++)
            {
                // Gerar o nome do pai e da mãe
                string sobrenomePai, sobrenomeMae;
                string nomePai = GerarNomePai(out sobrenomePai);
                string nomeMae = GerarNomeMae(out sobrenomeMae);

                // Gerar o nome completo, herdando um sobrenome de cada
                string nomeCompleto = GerarNomeCompleto(sobrenomePai, sobrenomeMae);

                // gerar cpf
                string cpf = GerarCpfAleatorio();

                // gerar rg
                string rg = GerarRgAleatorio(i);

                var protocolo = new Protocolo
                {
                    NumeroProtocolo = "00001",          // Número de protocolo duplicado para testar validação
                    NumeroVia =  1,            // Número da via 1
                    Cpf = cpf,          // Gera CPFs com 11 dígitos
                    Rg = rg,           // Gera RG com variação de estados brasileiros
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                protocolos.Add(protocolo);
            }

            // Gerar 3 protocolos inválidos para CPF com via repetida          
            for (int i = 96; i <= 98; i++)
            {
                // Gerar o nome do pai e da mãe
                string sobrenomePai, sobrenomeMae;
                string nomePai = GerarNomePai(out sobrenomePai);
                string nomeMae = GerarNomeMae(out sobrenomeMae);

                // Gerar o nome completo, herdando um sobrenome de cada
                string nomeCompleto = GerarNomeCompleto(sobrenomePai, sobrenomeMae);

                // gerar cpf duplicado
                string cpfDuplicado = GerarCpfAleatorio(permitirDuplicidade: true);

                // gerar rg
                string rg = GerarRgAleatorio(i);

                var protocolo = new Protocolo
                {
                    NumeroProtocolo = i.ToString("D5"),
                    NumeroVia = 1,                     // Mesma via (1) para o mesmo CPF
                    Cpf = cpfDuplicado,                // CPF duplicado para testar validação de via
                    Rg = rg,          // RG com estados variados
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                protocolos.Add(protocolo);
            }

            // Gerar 5 protocolos inválidos com fotos de formatos inválidos (webp, bmp)
            for (int i = 99; i <= 103; i++)
            {
                // Gerar o nome do pai e da mãe
                string sobrenomePai, sobrenomeMae;
                string nomePai = GerarNomePai(out sobrenomePai);
                string nomeMae = GerarNomeMae(out sobrenomeMae);

                // Gerar o nome completo, herdando um sobrenome de cada
                string nomeCompleto = GerarNomeCompleto(sobrenomePai, sobrenomeMae);

                // gerar cpf
                string cpf = GerarCpfAleatorio();

                // gerar rg
                string rg = GerarRgAleatorio(i);

                var protocolo = new Protocolo
                {
                    NumeroProtocolo = i.ToString("D5"),
                    NumeroVia = (i % 5) + 1,
                    Cpf = cpf,          // Gera CPFs com 11 dígitos
                    Rg = rg,           // Gera RG com variação de estados brasileiros
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosInvalidas[i % fotosInvalidas.Count]  // Alterna entre fotos inválidas
                };

                protocolos.Add(protocolo);
            }

            // Gerar 3 protocolos inválidos para RG com via repetida          
            for (int i = 103; i <= 105; i++)
            {
                // Gerar o nome do pai e da mãe
                string sobrenomePai, sobrenomeMae;
                string nomePai = GerarNomePai(out sobrenomePai);
                string nomeMae = GerarNomeMae(out sobrenomeMae);

                // Gerar o nome completo, herdando um sobrenome de cada
                string nomeCompleto = GerarNomeCompleto(sobrenomePai, sobrenomeMae);

                // gerar cpf duplicado
                string cpf = GerarCpfAleatorio();

                // gerar rg
                string rgDuplicado = GerarRgAleatorio(i, permitirDuplicidade: true);

                var protocolo = new Protocolo
                {
                    NumeroProtocolo = i.ToString("D5"),
                    NumeroVia = 1,                     // Mesma via (1) para o mesmo CPF
                    Cpf = cpf,                // CPF duplicado para testar validação de via
                    Rg = rgDuplicado,          // RG com estados variados
                    Nome = nomeCompleto,           // Nome genérico
                    NomeMae = nomeMae,       // Nome da mãe genérico
                    NomePai = nomePai,       // Nome do pai genérico
                    Foto = fotosValidas[i % fotosValidas.Count]  // Alterna entre as fotos válidas disponíveis
                };

                protocolos.Add(protocolo);
            }


            return protocolos;
        }

        // Método para gerar um nome completo aleatório
        public static string GerarNomeCompleto(string sobrenomePai, string sobrenomeMae)
        {
            string nome = NomesMasculinos[Random.Next(NomesMasculinos.Count)];

            // A criança herda um sobrenome do pai e outro da mãe
            string sobrenome = $"{sobrenomePai} {sobrenomeMae}";

            return $"{nome} {sobrenome}";
        }

        // Método para gerar um nome feminino (para a mãe)
        public static string GerarNomeMae(out string sobrenome)
        {
            string nome = NomesFemininos[Random.Next(NomesFemininos.Count)];
            sobrenome = Sobrenomes[Random.Next(Sobrenomes.Count)];
            return $"{nome} {sobrenome}";
        }

        // Método para gerar um nome masculino (para o pai)
        public static string GerarNomePai(out string sobrenome)
        {
            string nome = NomesMasculinos[Random.Next(NomesMasculinos.Count)];
            sobrenome = Sobrenomes[Random.Next(Sobrenomes.Count)];
            return $"{nome} {sobrenome}";
        }

        // Função para gerar CPFs aleatórios com exatamente 11 dígitos, com possibilidade de duplicar alguns
        public static string GerarCpfAleatorio(bool permitirDuplicidade = false)
        {
            // Se permitir duplicidade e já houver CPFs gerados, escolher aleatoriamente um existente
            if (permitirDuplicidade && cpfsGerados.Count > 0 && Random.Next(0, 10) < 3) // 30% de chance de duplicar
            {
                return cpfsGerados[Random.Next(cpfsGerados.Count)];  // Retorna um CPF já gerado (duplicado)
            }

            // Gerar um CPF novo (único)
            string novoCpf = $"{Random.Next(100000000, 999999999)}{Random.Next(10, 99)}";  // Gera 9 dígitos + 2 dígitos finais

            // Armazenar o CPF gerado para futuras duplicações
            cpfsGerados.Add(novoCpf);

            return novoCpf;
        }

        // Função para gerar RGs aleatórios, com possibilidade de duplicar alguns
        public static string GerarRgAleatorio(int index, bool permitirDuplicidade = false)
        {
            List<string> estadosBrasileiros = new List<string>
            {
                "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
            };

            // Se permitir duplicidade e já houver RGs gerados, escolher aleatoriamente um existente
            if (permitirDuplicidade && rgsGerados.Count > 0 && Random.Next(0, 10) < 3) // 30% de chance de duplicar
            {
                return rgsGerados[Random.Next(rgsGerados.Count)];  // Retorna um RG já gerado (duplicado)
            }

            // Gera um RG novo
            string estado = estadosBrasileiros[Random.Next(estadosBrasileiros.Count)];
            string novoRg = $"{estado}{100000 + index}";  // Ex: "SP100001", "MG100002"

            // Armazena o RG gerado para futuras duplicações
            rgsGerados.Add(novoRg);

            return novoRg;
        }
    }
}
