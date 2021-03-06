SELECT        
	Clients.Id,
	PersonNames.FirstName, PersonNames.LastName, PersonNames.MiddleName,
	Practices.Name AS Facility,
	Encounters.EncounterDate, Encounters.Id AS EncounterId, EncounterTypes.Name EncounterType  
FROM            
	Clients INNER JOIN	
	PersonNames ON Clients.PersonId = PersonNames.PersonId INNER JOIN
    Encounters ON Clients.Id = Encounters.ClientId INNER JOIN
    EncounterTypes ON Encounters.EncounterTypeId = EncounterTypes.Id INNER JOIN
    Practices ON Clients.PracticeId = Practices.Id
ORDER BY 
	Encounters.EncounterDate DESC