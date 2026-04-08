# Spisekammeret — Prosjektkontekst og Plan

## Om appen
Norsk oppskriftsapp for å opprette, lagre, presentere og importere matoppskrifter.

- **Kode:** Engelsk
- **GUI og oppskrifter:** Norsk
- **GitHub:** https://github.com/TorgeirI/spisekammeret

---

## Tech Stack

| Lag               | Teknologi                          |
|-------------------|------------------------------------|
| Backend API       | ASP.NET Core 9 Web API (C#)        |
| Frontend web      | React + TypeScript (Vite)          |
| Mobil (senere)    | React Native                       |
| Database          | PostgreSQL + EF Core               |
| LLM-parsing       | Claude API (Anthropic)             |
| Web scraping      | AngleSharp                         |

---

## Prosjektstruktur

```
spisekammeret/
├── Spisekammeret.Api/            ← ASP.NET Core 9 Web API
│   └── Controllers/
├── Spisekammeret.Core/           ← Delte modeller, interfaces, DTOs
│   ├── Models/
│   ├── Interfaces/
│   └── DTOs/
├── Spisekammeret.Infrastructure/ ← EF Core, repositories, services
│   ├── Data/
│   ├── Repositories/
│   └── Services/
├── Spisekammeret.Web/            ← React + TypeScript (Vite)
└── CONTEXT.md                   ← Denne filen
```

### Avhengigheter mellom prosjekter

```
Spisekammeret.Api
  ├── Spisekammeret.Core
  └── Spisekammeret.Infrastructure
        └── Spisekammeret.Core
```

---

## Dataformat: Schema.org/Recipe med JSON-LD

Bransjestandard brukt av Google og store oppskriftssider.

### Nøkkelfelt

| Felt                  | Type              | Beskrivelse                        |
|-----------------------|-------------------|------------------------------------|
| `name`                | string            | Navn på retten                     |
| `description`         | string            | Kort beskrivelse                   |
| `image`               | string[]          | Bilde-URLer                        |
| `prepTime`            | ISO 8601 duration | Forberedelsestid (PT15M)           |
| `cookTime`            | ISO 8601 duration | Koketid                            |
| `totalTime`           | ISO 8601 duration | Total tid                          |
| `recipeYield`         | string            | Porsjoner / antall                 |
| `recipeIngredient`    | string[]          | Liste over ingredienser            |
| `recipeInstructions`  | HowToStep[]       | Steg-for-steg fremgangsmåte        |
| `recipeCategory`      | string            | Kategori (Dessert, Middag, etc.)   |
| `recipeCuisine`       | string            | Kjøkken (Norsk, Italiensk, etc.)   |
| `nutrition`           | NutritionInfo     | Næringsinnhold                     |
| `keywords`            | string            | Kommaseparerte nøkkelord / tags    |

---

## Kategorisering og filtrering

### Filter-dimensjoner

| Dimensjon           | Eksempler                                             |
|---------------------|-------------------------------------------------------|
| Måltidstype         | Frokost, Lunsj, Middag, Dessert, Snacks              |
| Kjøkken             | Norsk, Italiensk, Asiatisk, Meksikansk, Fransk       |
| Kosthold            | Vegetar, Vegansk, Glutenfri, Melkefri, Lavkarbo/Keto |
| Tilberedningsmåte   | Steking, Baking, Grilling, Damping, Råkost           |
| Vanskelighet        | Enkel, Middels, Krevende                             |
| Tid                 | Under 15 min, 15–30 min, 30–60 min, Over 60 min     |
| Sesong / Anledning  | Jul, Påske, Sommer, Hverdagsmat, Helgemat            |
| Næringsinnhold      | Kalorier (range), Protein, Karbohydrat, Fett         |
| Porsjoner           | 1–2, 2–4, 4+                                         |

### Database-implementasjon
- **Tags** (mange-til-mange) for diskrete kategorier
- **Numeriske felt** for tid, kalorier, porsjoner — range-filtrering
- **Full-tekst søk** via PostgreSQL `tsvector` på navn, ingredienser, beskrivelse

---

## Arkitektur

```
┌─────────────────────────────────────────────────────────────────┐
│                    Frontend (React + TypeScript)                │
│                                                                 │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────────┐  │
│  │ Browse + Filter│  │ Recipe Detail  │  │  Import / Legg   │  │
│  │                │  │ (JSON-LD view) │  │  til oppskrift   │  │
│  └───────┬────────┘  └───────┬────────┘  └────────┬─────────┘  │
└──────────┼────────────────────┼───────────────────┼────────────┘
           └────────────────────┼───────────────────┘
                                │ REST API / OpenAPI
┌───────────────────────────────▼──────────────────────────────────┐
│                     ASP.NET Core 9 Web API                      │
│                                                                  │
│  ┌────────────────┐  ┌─────────────────┐  ┌──────────────────┐  │
│  │ RecipeController│  │ImportController │  │ SearchController │  │
│  └───────┬────────┘  └────────┬────────┘  └────────┬─────────┘  │
│          └───────────────────┬┘                    │            │
│                              │                     │            │
│  ┌───────────────────────────▼─────────────────────▼──────────┐ │
│  │                        Services                            │ │
│  │  ┌──────────────┐  ┌──────────────┐  ┌─────────────────┐  │ │
│  │  │ RecipeService│  │ ImportService│  │  SearchService  │  │ │
│  │  └──────────────┘  └──────┬───────┘  └─────────────────┘  │ │
│  │                           │                                │ │
│  │             ┌─────────────┴──────────────┐                │ │
│  │  ┌──────────▼──────────┐  ┌──────────────▼─────────────┐  │ │
│  │  │  ClaudeAiService    │  │      ScraperService        │  │ │
│  │  │  - Parser tekst     │  │  - Henter HTML fra URL     │  │ │
│  │  │  - Strukturerer til │  │  - Ekstraherer JSON-LD     │  │ │
│  │  │    Schema.org format│  │  - Fallback til LLM        │  │ │
│  │  └─────────────────────┘  └────────────────────────────┘  │ │
│  └────────────────────────────────────────────────────────────┘ │
└──────────────────────────────┬───────────────────────────────────┘
                               │ EF Core
┌──────────────────────────────▼───────────────────────────────────┐
│                           PostgreSQL                             │
│                                                                  │
│  recipes          tags            recipe_tags                    │
│  ───────────────  ─────────────   ──────────────                 │
│  id               id              recipe_id                      │
│  name             name            tag_id                         │
│  description      category                                       │
│  json_ld (JSONB)  slug            nutrition                       │
│  prep_time                        ──────────────                 │
│  cook_time        ingredients     calories                        │
│  servings         ─────────────   protein                        │
│  cuisine          id              fat                            │
│  difficulty       recipe_id       carbs                          │
│  created_at       name                                           │
│  source_url       amount                                         │
│                   unit                                           │
└──────────────────────────────────────────────────────────────────┘
```

---

## Import-flyt

```
URL / råtekst
     │
     ▼
Er det JSON-LD på siden?
  Ja ──► Bruk direkte (Schema.org allerede strukturert)
  Nei ──► AngleSharp scraper henter ren tekst
               │
               ▼
          Claude API
          "Parse dette til Schema.org Recipe JSON"
               │
               ▼
          Validering + lagring
```

---

## Plan / Gjenstående steg

- [x] Navn valgt: **Spisekammeret**
- [x] GitHub repo opprettet: https://github.com/TorgeirI/spisekammeret
- [x] Solution-struktur initialisert og pushet
- [ ] Kjernemodeller — C# domeneklasser + Schema.org-mapping (`Spisekammeret.Core`)
- [ ] Database — EF Core schema, migrasjoner, PostgreSQL (`Spisekammeret.Infrastructure`)
- [ ] API — CRUD endpoints + OpenAPI (`Spisekammeret.Api`)
- [ ] Import + LLM — scraper + Claude API-integrasjon
- [ ] React frontend — browse, filter, oppskriftsvisning
- [ ] Mobil — React Native (senere)
