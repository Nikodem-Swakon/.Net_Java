INSERT INTO Profiles (Name) VALUES ('Default');

-- Pokaż wszystkie profile
SELECT * FROM Profiles;

-- Pokaż wszystkie videos i ich ProfileId
SELECT * FROM Videos;

-- Opcjonalnie: ustaw poprawny ProfileId na istniejący (np. 1)
UPDATE Videos SET ProfileId = 1 WHERE ProfileId NOT IN (SELECT Id FROM Profiles);
