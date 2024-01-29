USE master;
GO
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'leiloes')
BEGIN
    ALTER DATABASE leiloes SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE leiloes;
END
GO

-- Criar a base de dados
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
('Mercedes EQA' ,                  'SUV compacto elétrico e elegante', '/img/mercedes.jpg',   1),
('Terreno em Vila Franca de Xira', '20000m^2, pronto para construção', '/img/terreno.jpg',    1),
('Camisola de LeBron James',       'Camisola usada em jogo por LBJ',   '/img/jersey.jpg',     3),
('Relógio Vintage Omega',          'Relógio Omega anos 50',            '/img/relogio.jpg',    4),
('Vinil Autografado Beatles',      'Vinil dos Beatles assinado',       '/img/beatles.jpg',    5),
('Pintura Original Monet',         'Quadro impressionista Monet',      '/img/quadro.jpg',     6),
('Escultura em Bronze',            'Escultura abstrata moderna',       '/img/escultura.jpg',  7),
('Manuscrito Séc. XVIII',          'Documento histórico raro',         '/img/manuscrito.jpg', 8),
('Câmera Leica M3 Clássica',       'Camera Leica modelo M3',           '/img/camera.jpg',     9),
('Vestido de Alta Costura',        'Vestido exclusivo designer',       '/img/vestido.jpg',   10),
('Coleção de Moedas Antigas',      'Moedas raras século XIX',          '/img/moedas.jpg',    11),
('Guitarra Fender Autografada',    'Guitarra assinada por Clapton',    '/img/guitarra.jpg',  12),
('Livro Primeira Edição',          'Livro clássico ed. original',      '/img/livro.jpg',     13),
('Camisola Miami Dolphins',        'QB1 Tagovailoa',                   '/img/tua.jpg',        1),
('Computador NASA',                'Usado na missão à Lua 1969',       '/img/pc.jpg',         3),
('Colunas Outkast',                'Usadas pelo duo em 1999',          '/img/colunas.jpg',    2),
('Telemóvel Samsung',              'Em excelente estado',              '/img/telemovel.jpg',  1),
('Caneta Saramago',                'Usada por José Saramago',          '/img/caneta.jpg',     3),
('Ténis Tim Duncan',               'Usados jogo 4 das finais de 03',   '/img/tenis.jpg',     1),
('Quadro DaVinci',                 'Quadro raro',                      '/img/davinci.jpg',    6),
('Quadro Warhol',                  'Quadro de Andy Warhol',            '/img/warhol.jpg',     4),
('Quadro Picasso',                 'Quadro de Pablo Picasso',          '/img/picasso.jpg',    2),
('Quadro Dali',                    'Quadro de Salvador Dali',          '/img/dali.jpg',      14),
('Quadro Pomar',                   'Quadro de Júlio Pomar',            '/img/pomar.jpg',      2);

GO

-- Inserir dados na tabela Utilizador
INSERT INTO Utilizador (nif, nome, username, email, password, userType, saldo) VALUES 
(111111111, 'Luis Borges',      'luisborges',       'luisborges@gmail.com',      '9b8769a4a742959a2d0298c36fb70623f2dfacda84362', 0, 0.00),
(222222222, 'Jose Vasconcelos', 'josevasconcelos',  'josevasconcelos@gmail.com', '9b8769a4a742959a2d0298c36fb70623f2dfacda84362', 0, 0.00),
(333333333, 'Flavio Silva',     'flaviosilva',      'flaviosilva@gmail.com',     '9b8769a4a742959a2d0298c36fb70623f2dfacda84362', 0, 0.00),
(444444444, 'Bento Guimaraes',  'bentoguimaraes',   'bentoguimaraes@gmail.com',  '9b8769a4a742959a2d0298c36fb70623f2dfacda84362', 0, 0.00),
(555555555, 'Tiago Pereira',    'tiagopereira',     'tiagopereira@gmail.com',    '9b8769a4a742959a2d0298c36fb70623f2dfacda84362', 0, 0.00),
(100000000, 'Miguel Porto',     'miguelportoAdmin', 'miguelporto@gmail.com',     '9b8769a4a742959a2d0298c36fb70623f2dfacda84362', 1, 0.00);
GO

-- Inserir dados na tabela Leilao
INSERT INTO Leilao (licitacaoAtual, precoMinLicitacao, estado, dataInicial, dataFinal, criadorId, produtoId) VALUES 
(56000.00,  40000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-01 18:00:00', 111111111,  1), -- 12 leilões terminados, para consultar nas estatísticas
(5100.00,  5000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-02 18:00:00', 111111111,  2), 
(700.00,  700.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-03 18:00:00', 111111111,  3),
(1400.00,  6500.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-04 18:00:00', 111111111,  4),
(11522.00,  11000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-05 18:00:00', 111111111,  5),
(600000.00,  500000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-06 18:00:00', 111111111,  6),
(450.00,  400.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-07 18:00:00', 111111111,  7),
(1200.00,  1200.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-08 18:00:00', 111111111,  8),
(140.00,  125.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-09 18:00:00', 111111111,  9),
(2100.00,  2000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-10 18:00:00', 111111111,  10),
(5000.00,  4000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-11 18:00:00', 111111111,  11),
(20000.00,  16000.00,  'terminado',     '2024-01-01 10:00:00', '2024-01-11 18:00:00', 111111111,  12),
(0.00,  120.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222, 13), -- mais 12 leilões ativos ou pendentes
(0.00,  150.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222, 14), 
(0.00,  16000.00,  'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222, 15),
(0.00,  2600.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 333333333, 16),
(0.00,  350.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 444444444, 17),
(0.00,  380.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 444444444, 18),
(0.00,  2000.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 444444444, 19),
(0.00,  300000.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 555555555, 20),
(0.00,  80000.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 555555555, 21),
(0.00,  560000.00, 'ativo',    '2024-01-01 10:00:00', '2024-02-01 18:00:00', 333333333, 22),
(0.00,  620000.00, 'pendente', '2024-01-01 10:00:00', '2024-02-01 18:00:00', 444444444, 23),
(0.00,  120000.00, 'pendente', '2024-01-01 10:00:00', '2024-02-01 18:00:00', 222222222, 24);

GO

-- Inserir dados na tabela Licitacao
INSERT INTO Licitacao (valor, leilao_idLeilao, user_NIF, dataLicitacao) VALUES
(50000.00, 1,  '333333333', '2024-01-10 18:00:00'),-- Licitações nos leilões terminados
(55555.00, 1,  '333333333', '2024-01-10 18:00:00'),
(56000.00, 1,  '333333333', '2024-01-10 18:00:00'),
(5100.00, 2,  '333333333', '2024-01-10 18:00:00'),
(700.00, 3,  '333333333', '2024-01-10 18:00:00'),
(1400.00, 4,  '333333333', '2024-01-10 18:00:00'),
(11522.00, 5,  '333333333', '2024-01-10 18:00:00'),
(600000.00, 6,  '333333333', '2024-01-10 18:00:00'),
(450.00, 7,  '333333333', '2024-01-10 18:00:00'),
(1200.00, 8,  '333333333', '2024-01-10 18:00:00'),
(140.00, 9,  '333333333', '2024-01-10 18:00:00'),
(2100.00, 10, '333333333', '2024-01-10 18:00:00'),
(5000.00, 11, '333333333', '2024-01-10 18:00:00'),
(20000.00, 12, '333333333', '2024-01-10 18:00:00');




GO


SELECT * FROM Produto;
SELECT * FROM Utilizador;
SELECT * FROM Leilao;
SELECT * FROM Licitacao;



