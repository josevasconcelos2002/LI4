USE master;
GO
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'leiloes')
BEGIN
    ALTER DATABASE leiloes SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE leiloes;
END
GO

-- Criar o banco de dados
CREATE DATABASE leiloes;
GO

USE leiloes;
GO
CREATE TABLE Produto (
    idProduto INT PRIMARY KEY IDENTITY(1,1),
    nome VARCHAR(45) NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    imagem VARCHAR(255), 
    numDonosAnt INT NOT NULL DEFAULT 0 
);
GO

CREATE TABLE Utilizador (
    nif VARCHAR(9) PRIMARY KEY,
    nome VARCHAR(45) NOT NULL,
    username VARCHAR(45) NOT NULL,
    email VARCHAR(45) NOT NULL,
    password VARCHAR(45) NOT NULL,
    userType INT NOT NULL,
    saldo DECIMAL(10, 2) NOT NULL
);
GO

-- Criar a tabela Leilao
CREATE TABLE Leilao (
    idLeilao INT PRIMARY KEY IDENTITY(1,1),
    licitacaoAtual DECIMAL(10, 2) NOT NULL,
    precoMinLicitacao DECIMAL(10, 2) NOT NULL,
    estado NVARCHAR(50) NOT NULL,
    dataInicial DATETIME NOT NULL,
    dataFinal DATETIME NOT NULL,
    criadorId VARCHAR(9),
    produtoId INT,
	FOREIGN KEY (criadorId) REFERENCES Utilizador(nif),
	FOREIGN KEY (produtoId) REFERENCES Produto(idProduto)
);
GO

CREATE TABLE Licitacao (
    idLicitacao INT PRIMARY KEY IDENTITY(1,1),
    valor DECIMAL(10, 2) NOT NULL,
    leilao_idLeilao INT NOT NULL,
    user_NIF VARCHAR(9) ,
    dataLicitacao DATETIME NOT NULL, 
    FOREIGN KEY (leilao_idLeilao) REFERENCES Leilao(idLeilao),
    FOREIGN KEY (user_NIF) REFERENCES Utilizador(nif)
);
GO


-- Inserir dados na tabela Produto
INSERT INTO Produto (nome, descricao, imagem, numDonosAnt) VALUES 
('Mercedes EQA' , 'O Mercedes EQA é um SUV compacto elétrico, elegante e tecnologicamente avançado', 'caminho/para/imagem1.jpg', 0),
('Terreno em Vila Franca de Xira', 'Terreno espaçoso (20000m^2 em Vila Franca de Xira, pronto para construção)', 'caminho/para/imagem2.jpg', 1),
('Camisola de LeBron James usada em jogo', 'Camisola dos Miami Heat usada por Lebron James no jogo 3 das finais de 2012', 'caminho/para/imagem3.jpg', 3);
GO

-- Inserir dados na tabela Utilizador
INSERT INTO Utilizador (nif, nome, username, email, password, userType, saldo) VALUES 
(111111111, 'Luis Borges', 'luisborges', 'luisborges@gmail.com', 'pass123', 0, 0.00),
(222222222, 'Jose Vasconcelos', 'josevasconcelos', 'josevasconcelos@gmail.com', 'pass456', 0, 0.00),
(333333333, 'Flavio Silva', 'flaviosilva', 'flaviosilva@gmail.com', 'pass789', 0, 0.00),
(444444444, 'Bento Guimaraes', 'bentoguimaraes', 'bentoguimaraes@gmail.com', 'pass234', 0, 0.00),
(555555555, 'Tiago Pereira', 'tiagopereira', 'tiagopereira@gmail.com', 'pass678', 0, 0.00),
(100000000, 'Miguel Porto', 'miguelportoAdmin', 'miguelporto@gmail.com', 'pass000', 1, 0.00);
GO

-- Inserir dados na tabela Leilao
INSERT INTO Leilao (licitacaoAtual, precoMinLicitacao, estado, dataInicial, dataFinal, criadorId, produtoId) VALUES 
(100.00, 50.00, 'Aberto', '2024-01-01 10:00:00', '2024-01-10 18:00:00', 111111111, 1),
(200.00, 150.00, 'Fechado', '2024-01-05 09:00:00', '2024-01-15 20:00:00', 222222222, 2);
GO

-- Inserir dados na tabela Licitacao
INSERT INTO Licitacao (valor, leilao_idLeilao, user_NIF, dataLicitacao) VALUES
(110.00, 1, '333333333', '2024-01-10 18:01:00'),
(210.00, 2, '444444444', '2024-01-15 20:05:00'),
(115.00, 1, '555555555', '2024-01-10 18:01:15');
GO

SELECT * FROM Produto;
SELECT * FROM Utilizador;
SELECT * FROM Leilao;
SELECT * FROM Licitacao;



