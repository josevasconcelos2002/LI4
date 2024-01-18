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
('Mercedes EQA' ,                  'SUV compacto elétrico e elegante', 'caminho/para/imagem1.jpg',   0),
('Terreno em Vila Franca de Xira', '20000m^2, pronto para construção', 'caminho/para/imagem2.jpg',   1),
('Camisola de LeBron James',       'Camisola usada em jogo por LBJ',   'caminho/para/imagem3.jpg',   3),
('Relógio Vintage Omega',          'Relógio Omega anos 50',            'caminho/para/imagem4.jpg',   4),
('Vinil Autografado Beatles',      'Disco dos Beatles assinado',       'caminho/para/imagem5.jpg',   5),
('Pintura Original Monet',         'Quadro impressionista Monet',      'caminho/para/imagem6.jpg',   6),
('Escultura em Bronze',            'Escultura abstrata moderna',       'caminho/para/imagem7.jpg',   7),
('Manuscrito Séc. XVIII',          'Documento histórico raro',         'caminho/para/imagem8.jpg',   8),
('Câmera Leica M3 Clássica',       'Camera Leica modelo M3',           'caminho/para/imagem9.jpg',   9),
('Vestido de Alta Costura',        'Vestido exclusivo designer',       'caminho/para/imagem10.jpg', 10),
('Coleção de Moedas Antigas',      'Moedas raras século XIX',          'caminho/para/imagem11.jpg', 11),
('Guitarra Fender Autografada',    'Guitarra assinada por Clapton',    'caminho/para/imagem12.jpg', 12),
('Livro Primeira Edição',          'Livro clássico ed. original',      'caminho/para/imagem13.jpg', 13);
GO

-- Inserir dados na tabela Utilizador
INSERT INTO Utilizador (nif, nome, username, email, password, userType, saldo) VALUES 
(111111111, 'Luis Borges',      'luisborges',       'luisborges@gmail.com',      'pass123', 0, 0.00),
(222222222, 'Jose Vasconcelos', 'josevasconcelos',  'josevasconcelos@gmail.com', 'pass456', 0, 0.00),
(333333333, 'Flavio Silva',     'flaviosilva',      'flaviosilva@gmail.com',     'pass789', 0, 0.00),
(444444444, 'Bento Guimaraes',  'bentoguimaraes',   'bentoguimaraes@gmail.com',  'pass234', 0, 0.00),
(555555555, 'Tiago Pereira',    'tiagopereira',     'tiagopereira@gmail.com',    'pass678', 0, 0.00),
(100000000, 'Miguel Porto',     'miguelportoAdmin', 'miguelporto@gmail.com',     'pass000', 1, 0.00),
(200000000, 'Ana Silva',        'anasilvaUser',     'anasilva@gmail.com',        'pass001', 0, 0.00),
(300000000, 'João Santos',      'joaosantosUser',   'joaosantos@gmail.com',      'pass002', 0, 0.00),
(400000000, 'Maria Costa',      'mariacostaUser',   'mariacosta@gmail.com',      'pass003', 0, 0.00),
(500000000, 'Lucas Pereira',    'lucaspereiraUser', 'lucaspereira@gmail.com',    'pass004', 0, 0.00),
(600000000, 'Teresa Gomes',     'teresagomesUser',  'teresagomes@gmail.com',     'pass005', 0, 0.00);
GO

-- Inserir dados na tabela Leilao
INSERT INTO Leilao (licitacaoAtual, precoMinLicitacao, estado, dataInicial, dataFinal, criadorId, produtoId) VALUES 
(100.00,  50.00,   'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 111111111,  1),
(200.00,  150.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222,  2),
(100.00,  50.00,   'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222,  3),
(200.00,  150.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 300000000,  4),
(300.00,  250.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 444444444,  5),
(400.00,  350.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 400000000,  6),
(500.00,  450.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 555555555,  7),
(600.00,  550.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 500000000,  8),
(700.00,  650.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 111111111,  9),
(800.00,  750.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 600000000, 10),
(900.00,  850.00,  'ativo',     '2024-01-01 10:00:00', '2024-02-01 18:00:00', 333333333, 11),
(1000.00, 950.00,  'terminado', '2024-01-01 10:00:00', '2024-02-01 18:00:00', 200000000, 12),
(1100.00, 1050.00, 'pendente',  '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222, 13);

GO

-- Inserir dados na tabela Licitacao
INSERT INTO Licitacao (valor, leilao_idLeilao, user_NIF, dataLicitacao) VALUES
(110.00, 1, '333333333', '2024-01-10 18:00:00'),
(115.00, 1, '555555555', '2024-01-10 19:00:00'),
(210.00, 2, '444444444', '2024-01-10 18:00:00'),
(250.00, 2, '555555555', '2024-01-10 19:00:00'),
(255.00, 2, '111111111', '2024-01-10 20:00:00'),
(260.00, 2, '100000000', '2024-01-10 21:00:00'),
(300.00, 3, '100000000', '2024-01-10 18:00:00'),
(310.00, 3, '600000000', '2024-01-10 18:01:00'),
(320.00, 3, '100000000', '2024-01-10 19:00:00'),
(390.05, 3, '600000000', '2024-01-10 20:32:05');



GO

SELECT * FROM Produto;
SELECT * FROM Utilizador;
SELECT * FROM Leilao;
SELECT * FROM Licitacao;



