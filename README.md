
# TooliRent – README


TooliRent är ett REST‑API för verktygsuthyrning byggt i .NET 8 enligt en N‑Tier‑arkitektur. Projektet består av fyra lager: WebAPI (controllers, Swagger, JWT‑autentisering), Services (affärslogik, DTO:er, AutoMapper, FluentValidation), Core (entiteter, interfaces och basmodeller) och Infrastructure (EF Core DbContext, repositories, migrationer och seed‑data). Arkitekturen följer Repository- och Unit of Work‑mönstret, med separata DTO:er för Create, Update och Dto.

Datamodellerna är uppbyggda kring en gemensam BaseEntity med Id, CreatedAt och UpdatedAt. En Customer har Name, Email, PhoneNumber och IsActive. Ett Tool innehåller bland annat Name, Price, Description, Stock, CatalogNumber, Category, Status och IsAvailable. En Rental knyter en kund till en uthyrning med StartDate, EndDate, IsReturned samt kopplade RentalDetails. Varje RentalDetail kopplar en Rental till ett specifikt Tool med en angiven Quantity. DTO:er används för att hantera in- och utdata, och valideras med FluentValidation.

API‑et erbjuder endpoints för flera områden. Under Auth finns inloggning, tokenförnyelse, registrering av admins och medlemmar, samt aktivering, avaktivering och radering av konton. Man kan även lista admins och medlemmar samt hämta aktuell användare. Customer‑delen innehåller CRUD‑operationer och aktivering/avaktivering av kunder. Tool‑delen ger möjlighet att skapa, hämta, uppdatera, ta bort samt lista verktyg, inklusive filtrering, listning av kategorier och kontroll av tillgänglighet. Rental‑delen hanterar hela uthyrningsflödet med att skapa, hämta, uppdatera, avboka, hämta ut och återlämna, samt att se egna uthyrningar och försenade uthyrningar. Slutligen innehåller AdminSummary sammanställningar av nyckeltal och de mest populära verktygen.

För att köra projektet lokalt krävs .NET och SQL Server. Efter att repot klonats anger man connection string i WebAPI/appsettings.json, kör `Update-Database` för att skapa och seeda databasen, och startar sedan WebAPI‑projektet. API‑et blir tillgängligt via Swagger på /swagger, där även detaljerade exempel för alla endpoints kan testas. Seedade data innehåller både verktyg, kunder och testanvändare med rollerna Admin och Member.

Projektet visar en komplett lösning för verktygsuthyrning med .NET, inkluderande autentisering, datavalidering, affärslogik, datalager och dokumentation via Swagger.

