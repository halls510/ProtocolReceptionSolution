CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Protocolos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Protocolos" PRIMARY KEY AUTOINCREMENT,
    "NumeroProtocolo" TEXT NOT NULL,
    "NumeroVia" INTEGER NOT NULL,
    "Cpf" TEXT NOT NULL,
    "Rg" TEXT NOT NULL,
    "Nome" TEXT NOT NULL,
    "NomeMae" TEXT NOT NULL,
    "NomePai" TEXT NOT NULL,
    "Foto" TEXT NOT NULL
);

CREATE UNIQUE INDEX "idx_cpf_numero_via" ON "Protocolos" ("Cpf", "NumeroVia");

CREATE UNIQUE INDEX "idx_numero_protocolo" ON "Protocolos" ("NumeroProtocolo");

CREATE UNIQUE INDEX "idx_rg_numero_via" ON "Protocolos" ("Rg", "NumeroVia");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240918013726_InitialCreate', '6.0.33');

COMMIT;

